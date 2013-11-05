using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slab.Messages;
using Slab.Pages;
using Slab.Pages.Navigation;
using Slab.PresentationBus;
using Slab.Requests;
using Slab.ViewModels;
using SlabRt.Data.Navigation;
using SlabRt.Pages;
using SlabRt.Requests;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SlabRt.Controls
{
    public class NavigationFrame : 
        ContentControl,
        IHandlePresentationEvent<PageNavigationRequest>,
        IHandlePresentationEvent<ViewModelNavigationRequest>,
        IHandlePresentationEvent<GoBackRequest>
    {
        private readonly Stack<NavigationFrameStackItem> _navigationStack;

        public NavigationFrame()
        {
            Unloaded += OnUnloaded;

            // Note: the top of the navigation stack is always the currently displayed page
            _navigationStack = new Stack<NavigationFrameStackItem>();

            SettingsPane.GetForCurrentView().CommandsRequested += HostCommandsRequested;
        }

        void HostCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            if (Content == null)
                return;

            var viewType = Content.GetType();
            var request = new DisplaySettingsRequest(viewType, args.Request);
            PresentationBus.Publish(request);
        }

        public static readonly DependencyProperty TargetNameProperty =
            DependencyProperty.Register("TargetName", typeof (string), typeof (NavigationFrame), new PropertyMetadata(default(string)));

        public string TargetName
        {
            get { return (string) GetValue(TargetNameProperty); }
            set { SetValue(TargetNameProperty, value); }
        }

        public static readonly DependencyProperty ViewLocatorProperty =
            DependencyProperty.Register("ViewLocator", typeof (object), typeof (NavigationFrame), new PropertyMetadata(default(IViewLocator)));

        public IViewLocator ViewLocator
        {
            get { return (IViewLocator)GetValue(ViewLocatorProperty); }
            set { SetValue(ViewLocatorProperty, value); }
        }

        public static readonly DependencyProperty PageCommandsPanelProperty =
            DependencyProperty.Register("PageCommandsPanel", typeof (Panel), typeof (NavigationFrame), new PropertyMetadata(default(Panel), TryToRestoreNavigationStack));

        public Panel PageCommandsPanel
        {
            get { return (Panel) GetValue(PageCommandsPanelProperty); }
            set { SetValue(PageCommandsPanelProperty, value); }
        }

        public static readonly DependencyProperty CanGoBackProperty =
            DependencyProperty.Register("CanGoBack", typeof (bool), typeof (NavigationFrame), new PropertyMetadata(default(bool)));

        public bool CanGoBack
        {
            get { return (bool) GetValue(CanGoBackProperty); }
            set { SetValue(CanGoBackProperty, value); }
        }

        public static readonly DependencyProperty NavigationStackStorageProperty =
            DependencyProperty.Register("NavigationStackStorage", typeof(object), typeof(NavigationFrame), new PropertyMetadata(default(INavigationStackStorage), TryToRestoreNavigationStack));

        public INavigationStackStorage NavigationStackStorage
        {
            get { return (INavigationStackStorage)GetValue(NavigationStackStorageProperty); }
            set { SetValue(NavigationStackStorageProperty, value); }
        }

        public static readonly DependencyProperty DefaultRouteProperty =
            DependencyProperty.Register("DefaultRoute", typeof (string), typeof (NavigationFrame), new PropertyMetadata(default(string), TryToRestoreNavigationStack));

        public string DefaultRoute
        {
            get { return (string) GetValue(DefaultRouteProperty); }
            set { SetValue(DefaultRouteProperty, value); }
        }

        public static readonly DependencyProperty ControllerInvokerProperty =
            DependencyProperty.Register("ControllerInvoker", typeof(object), typeof(NavigationFrame), new PropertyMetadata(default(IControllerInvoker), TryToRestoreNavigationStack));

        public IControllerInvoker ControllerInvoker
        {
            get { return (IControllerInvoker)GetValue(ControllerInvokerProperty); }
            set { SetValue(ControllerInvokerProperty, value); }
        }

        public static readonly DependencyProperty PresentationBusProperty =
            DependencyProperty.Register("PresentationBus", typeof (object), typeof (NavigationFrame), new PropertyMetadata(default(IPresentationBus), OnPresentationBusChanged));

        private static void OnPresentationBusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;

            var frame = (NavigationFrame) d;

            frame.PresentationBus.Subscribe(frame);
        }

        public IPresentationBus PresentationBus
        {
            get { return (IPresentationBus) GetValue(PresentationBusProperty); }
            set { SetValue(PresentationBusProperty, value); }
        }

        public static readonly DependencyProperty CurrentPageTitleProperty =
            DependencyProperty.Register("CurrentPageTitle", typeof (string), typeof (NavigationFrame), new PropertyMetadata(default(string)));

        public string CurrentPageTitle
        {
            get { return (string) GetValue(CurrentPageTitleProperty); }
            set { SetValue(CurrentPageTitleProperty, value); }
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (PresentationBus == null) return;

            PresentationBus.UnSubscribe(this);
        }

        private static void TryToRestoreNavigationStack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;

            var frame = (NavigationFrame)d;
            frame.RestoreNavigationStack();
        }

        public async void RestoreNavigationStack()
        {
            if (NavigationStackStorage == null ||
                string.IsNullOrWhiteSpace(DefaultRoute) ||
                ControllerInvoker == null ||
                PageCommandsPanel == null)
            {
                return;
            }

            var routes = NavigationStackStorage.RetrieveRoutes();

            NavigationFrameStackItem navigationFrameStackItem;
            if (routes == null || routes.Any() == false)
            {
                navigationFrameStackItem = new NavigationFrameStackItem(DefaultRoute, null);
                _navigationStack.Push(navigationFrameStackItem);
            }
            else
            {
                foreach (var route in routes)
                {
                    _navigationStack.Push(new NavigationFrameStackItem(route, null));
                }
                navigationFrameStackItem = _navigationStack.Peek();
            }

            await CheckItemContent(navigationFrameStackItem);
            Content = navigationFrameStackItem.Content;

            SetCanGoBack();
        }

        public void Handle(GoBackRequest request)
        {
            if (CanGoBack)
            {
                GoBack();
            }
            request.IsHandled = true;
        }

        public void Handle(PageNavigationRequest request)
        {
            if (request.Args.Target != TargetName)
                return;

            NavigateToPage(request.Route, request.Args.ViewType, request.Args.Parameter);
        }

        private void NavigateToPage(string route, Type viewType, object parameter)
        {
            // create the view instance.
            var view = (FrameworkElement)Activator.CreateInstance(viewType);
            view.DataContext = parameter;

            GoForward(route, view);
        }

        public void Handle(ViewModelNavigationRequest request)
        {
            if (request.Args.Target != TargetName)
                return;

            NavigateToViewModelAndAddToStack(request.Route, request.Args.ViewModel);
        }

        private void NavigateToViewModelAndAddToStack(string route, object viewModel)
        {
            var contentSwitchingPage = NavigateToViewModel(viewModel);

            GoForward(route, contentSwitchingPage);
        }

        private FrameworkElement NavigateToViewModel(object viewModel)
        {
            ViewLocator.Resolve(viewModel, PageLayoutProvider.DetermineVisualState());

            var contentSwitchingPage = new ContentSwitchingPage
                                           {
                                               DataContext = viewModel,
                                               ViewLocator = ViewLocator,
                                               PageCommandsPanel = PageCommandsPanel
                                           };

            return contentSwitchingPage;
        }

        private void GoForward(string route, FrameworkElement newContent)
        {
            // We only want 1 SearchViewModel on the top of the stack, so if the top item and the new content
            // are both SearchViewModels, we pop the top one off and discard it.
            var topItem = _navigationStack.Peek();
            if (topItem != null &&
                topItem.Content.DataContext is ISearchViewModelBase &&
                newContent.DataContext is ISearchViewModelBase)
            {
                _navigationStack.Pop();
            }

            _navigationStack.Push(new NavigationFrameStackItem(route, newContent));

            Content = newContent;
            SetCanGoBack();

            UpdateCurrentPageTitle(newContent);

            if (NavigationStackStorage != null)
            {
                NavigationStackStorage.StoreRoutes(_navigationStack.Select(i => i.Route).ToArray());
            }
        }

        public async void GoBack()
        {
            if (CanGoBack == false)
                return;
            
            // pop the current page off the stack
            _navigationStack.Pop();
            
            var item = _navigationStack.Peek();
            await CheckItemContent(item);
            Content = item.Content;
            SetCanGoBack();

            UpdateCurrentPageTitle(item.Content);

            if (NavigationStackStorage != null)
            {
                NavigationStackStorage.StoreRoutes(_navigationStack.Select(i => i.Route).ToArray());
            }
        }

        private void SetCanGoBack()
        {
            CanGoBack = _navigationStack.Count() > 1;
        }

        private void UpdateCurrentPageTitle(FrameworkElement newContent)
        {
            var hasPageTitle = newContent.DataContext as HasPageTitleBase;
            if (hasPageTitle != null)
            {
                CurrentPageTitle = hasPageTitle.PageTitle;
            }
        }

        private async Task<bool> CheckItemContent(NavigationFrameStackItem item)
        {
            if (item.Content != null) 
                return false;

            var controllerResult = await ControllerInvoker.CallAsync(item.Route);
            if (controllerResult.Result is IPageActionResult)
            {
                //NavigateToPage();
            }
            else if (controllerResult.Result is ViewModelActionResult)
            {
                var view = NavigateToViewModel(((ViewModelActionResult)controllerResult.Result).ViewModelInstance);
                item.Content = view;
                UpdateCurrentPageTitle(item.Content);
            }

            return true;
        }

        private class NavigationFrameStackItem
        {
            public NavigationFrameStackItem(string route, FrameworkElement content)
            {
                Route = route;
                Content = content;
            }

            public string Route { get; private set; }
            public FrameworkElement Content { get; set; }

            public override bool Equals(object obj)
            {
                return obj is NavigationFrameStackItem && ((NavigationFrameStackItem)obj).Route == Route;
            }
            public override int GetHashCode()
            {
                return Route.GetHashCode();
            }
        }
    }
}