using Orienteer.Pages;
using Orienteer.Pages.Navigation;
using Orienteer.Requests;
using Orienteer.WinStore.Events;
using Orienteer.WinStore.Pages;
using Orienteer.WinStore.Requests;
using Orienteer.Xaml.ViewModels;
using PresentationBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Orienteer.WinStore.Controls
{
    public class NavigationFrame : 
        ContentControl,
        IHandlePresentationCommand<PageNavigationCommand>,
        IHandlePresentationCommandAsync<ViewModelNavigationCommand>,
        IHandlePresentationCommandAsync<GoBackCommand>
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
            var cmd = new DisplaySettingsCommand(viewType, args.Request);
            PresentationBus.Send(cmd);
        }

        public static readonly DependencyProperty TargetNameProperty =
            DependencyProperty.Register("TargetName", typeof (string), typeof (NavigationFrame), new PropertyMetadata(default(string)));

        public string TargetName
        {
            get { return (string) GetValue(TargetNameProperty); }
            set { SetValue(TargetNameProperty, value); }
        }

        public static readonly DependencyProperty ViewLocatorProperty =
            DependencyProperty.Register("ViewLocator", typeof(object), typeof(NavigationFrame), new PropertyMetadata(default(IViewLocator)));

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

            ((IPresentationBusConfiguration)frame.PresentationBus).Subscribe(frame);
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

            ((IPresentationBusConfiguration)PresentationBus).UnSubscribe(this);
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

            await SwitchContent(navigationFrameStackItem.Content);
        }

        public async Task HandleAsync(GoBackCommand command)
        {
            if (CanGoBack)
            {
                await GoBack();
            }
        }

        public void Handle(PageNavigationCommand command)
        {
            if (command.Args.Target != TargetName)
                return;

            NavigateToPage(command.Route, command.Args.ViewType, command.Args.Parameter);
        }

        private async Task NavigateToPage(string route, Type viewType, object parameter)
        {
            // create the view instance.
            var view = (FrameworkElement)Activator.CreateInstance(viewType);
            view.DataContext = parameter;

            await GoForward(route, view);
        }

        public async Task HandleAsync(ViewModelNavigationCommand command)
        {
            if (command.Args.Target != TargetName)
                return;

            await NavigateToViewModelAndAddToStack(command.Route, command.Args.ViewModel);
        }

        private async Task NavigateToViewModelAndAddToStack(string route, object viewModel)
        {
            var contentSwitchingPage = NavigateToViewModel(viewModel);

            await GoForward(route, contentSwitchingPage);
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

        private async Task GoForward(string route, FrameworkElement newContent)
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

            await SwitchContent(newContent);
            
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
            await SwitchContent(item.Content);

            UpdateCurrentPageTitle(item.Content);

            if (NavigationStack != null)
            {
                NavigationStack.StoreRoutes(_navigationStack.Select(i => i.Route).ToArray());
            }
        }

        private async Task SwitchContent(FrameworkElement content)
        {
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            if (Content != null)
            {
                dataTransferManager.DataRequested -= DataTransferManagerOnDataRequested;
            }

            Content = content;
            await SetCanGoBack();

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

        private async Task SetCanGoBack()
        {
            CanGoBack = _navigationStack.Count() > 1;
            await PresentationBus.Publish(new CanGoBackChanged(CanGoBack));
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