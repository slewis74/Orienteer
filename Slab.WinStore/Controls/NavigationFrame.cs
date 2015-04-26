using Slab.Pages;
using Slab.Pages.Navigation;
using Slab.Requests;
using Slab.WinStore.Data.Navigation;
using Slab.WinStore.Events;
using Slab.WinStore.Pages;
using Slab.WinStore.Requests;
using Slab.Xaml;
using Slab.Xaml.ViewModels;
using Slew.PresentationBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Slab.WinStore.Controls
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
            PresentationBus.PublishAsync(request);
        }

        public static readonly DependencyProperty TargetNameProperty =
            DependencyProperty.Register("TargetName", typeof (string), typeof (NavigationFrame), new PropertyMetadata(default(string)));

        public string TargetName
        {
            get { return (string) GetValue(TargetNameProperty); }
            set { SetValue(TargetNameProperty, value); }
        }

        public static readonly DependencyProperty ViewLocatorProperty =
            DependencyProperty.Register("ViewLocator", typeof(object), typeof(NavigationFrame), new PropertyMetadata(default(IViewLocator<FrameworkElement>)));

        public IViewLocator<FrameworkElement> ViewLocator
        {
            get { return (IViewLocator<FrameworkElement>)GetValue(ViewLocatorProperty); }
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

        public static readonly DependencyProperty NavigationStackProperty =
            DependencyProperty.Register("NavigationStack", typeof(object), typeof(NavigationFrame), new PropertyMetadata(default(INavigationStack), TryToRestoreNavigationStack));

        public INavigationStack NavigationStack
        {
            get { return (INavigationStack)GetValue(NavigationStackProperty); }
            set { SetValue(NavigationStackProperty, value); }
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
            if (NavigationStack == null ||
                ControllerInvoker == null ||
                PageCommandsPanel == null)
            {
                return;
            }

            var routes = NavigationStack.RetrieveRoutes();

            foreach (var route in routes)
            {
                _navigationStack.Push(new NavigationFrameStackItem(route, null));
            }
            
            var navigationFrameStackItem = _navigationStack.Peek();
            
            await CheckItemContent(navigationFrameStackItem);

            SwitchContent(navigationFrameStackItem.Content);
        }

        public async void Handle(GoBackRequest request)
        {
            if (CanGoBack)
            {
                await GoBack();
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
            ViewLocator.Resolve(viewModel);

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

            SwitchContent(newContent);
            
            UpdateCurrentPageTitle(newContent);

            if (NavigationStack != null)
            {
                NavigationStack.StoreRoutes(_navigationStack.Select(i => i.Route).ToArray());
            }
        }

        public async Task GoBack()
        {
            if (CanGoBack == false)
                return;
            
            // pop the current page off the stack
            _navigationStack.Pop();
            
            var item = _navigationStack.Peek();
            await CheckItemContent(item);
            SwitchContent(item.Content);

            UpdateCurrentPageTitle(item.Content);

            if (NavigationStack != null)
            {
                NavigationStack.StoreRoutes(_navigationStack.Select(i => i.Route).ToArray());
            }
        }

        private void SwitchContent(FrameworkElement content)
        {
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            if (Content != null)
            {
                dataTransferManager.DataRequested -= DataTransferManagerOnDataRequested;
            }

            Content = content;
            SetCanGoBack();

            if (Content != null)
            {
                dataTransferManager.DataRequested += DataTransferManagerOnDataRequested;
            }
        }

        private void DataTransferManagerOnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var currentPage = Content as FrameworkElement;
            if (currentPage == null) return;

            var viewModel = currentPage.DataContext as IShare;
            if (viewModel == null) return;

            if (viewModel.GetShareContent(args.Request))
            {
                // Out of the datapackage properties, the title is required. If the scenario completed successfully, we need
                // to make sure the title is valid since the sample scenario gets the title from the user.
                if (String.IsNullOrEmpty(args.Request.Data.Properties.Title))
                {
                    args.Request.FailWithDisplayText("Title is required, share cannot continue.");
                }
            }
        }

        private void SetCanGoBack()
        {
            CanGoBack = _navigationStack.Count() > 1;
            PresentationBus.PublishAsync(new CanGoBackChanged(CanGoBack));
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