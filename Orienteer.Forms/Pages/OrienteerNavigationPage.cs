using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orienteer.Messages;
using Orienteer.Pages.Navigation;
using PresentationBus;
using Xamarin.Forms;

namespace Orienteer.Forms.Pages
{
    public class OrienteerNavigationPage :
        NavigationPage,
        IHandlePresentationCommandAsync<ViewModelNavigationCommand>,
        IHandlePresentationCommandAsync<GoBackCommand>
    {
        private readonly INavigator _navigator;
        private readonly IViewLocator _viewLocator;
        private readonly INavigationStack _navigationStack;
        private bool _hasStarted;

        private readonly Stack<NavigationFrameStackItem> _navigationStackCache;

        public OrienteerNavigationPage(
            INavigator navigator,
            IViewLocator viewLocator,
            INavigationStack navigationStack)
        {
            _navigator = navigator;
            _viewLocator = viewLocator;
            _navigationStack = navigationStack;

            _navigationStackCache = new Stack<NavigationFrameStackItem>();

            Popped += OnPopped;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_hasStarted == false)
            {
                Task.Run(async () => await DoStartup());
            }
        }

        private void OnPopped(object sender, NavigationEventArgs navigationEventArgs)
        {
            _navigationStackCache.Pop();
            _navigationStack?.StoreRoutes(_navigationStackCache.Select(i => i.Route).ToArray());
        }

        public async Task DoStartup()
        {
            if (_hasStarted)
                return;

            _hasStarted = true;

            var routes = _navigationStack.RetrieveRoutes();
            foreach (var route in routes)
            {
                await _navigator.NavigateAsync(route, false);
            }
        }

        public async Task HandleAsync(ViewModelNavigationCommand command)
        {
            await NavigateToViewModelAndAddToStack(command.Route, command.Args.ViewModel, command.Animated);
        }

        public async Task HandleAsync(GoBackCommand command)
        {
            if (Navigation.NavigationStack.Any())
            {
                await GoBack();
            }
        }

        private async Task NavigateToViewModelAndAddToStack(string route, object viewModel, bool animated)
        {
            var page = NavigateToViewModel(viewModel);

            await GoForward(route, page, animated);
        }

        private Page NavigateToViewModel(object viewModel)
        {
            var page = _viewLocator.Resolve(viewModel);
            page.BindingContext = viewModel;
            return page;
        }

        private async Task GoForward(string route, Page newPage, bool animated)
        {
            _navigationStackCache.Push(new NavigationFrameStackItem(route, newPage));

            await Navigation.PushAsync(newPage, animated);

            // Only store this if the app is already running, i.e. not doing startup restore of the stack
            if (_hasStarted && _navigationStack != null)
            {
                _navigationStack.StoreRoutes(_navigationStackCache.Select(i => i.Route).ToArray());
            }
        }

        public async Task GoBack()
        {
            await Navigation.PopAsync();
        }

        private class NavigationFrameStackItem
        {
            public NavigationFrameStackItem(string route, Page content)
            {
                Route = route;
                Content = content;
            }

            public string Route { get; }
            public Page Content { get; set; }

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