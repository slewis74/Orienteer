using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slab.Pages.Navigation;
using Slab.Requests;
using Slew.PresentationBus;
using Xamarin.Forms;

namespace Slab.Forms.Pages
{
    public class SlabNavigationPage :
        NavigationPage,
        IHandlePresentationEventAsync<ViewModelNavigationRequest>,
        IHandlePresentationEventAsync<GoBackRequest>
    {
        private readonly INavigator _navigator;
        private readonly IViewLocator _viewLocator;
        private readonly INavigationStack _navigationStack;
        private bool _hasAppearedBefore;

        private readonly Stack<NavigationFrameStackItem> _navigationStackCache;

        public SlabNavigationPage(
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

            if (_hasAppearedBefore == false)
            {
                DoStartup();
            }
        }

        private void OnPopped(object sender, NavigationEventArgs navigationEventArgs)
        {
            _navigationStackCache.Pop();
            if (_navigationStack != null)
            {
                _navigationStack.StoreRoutes(_navigationStackCache.Select(i => i.Route).ToArray());
            }
        }

        public async Task DoStartup()
        {
            var routes = _navigationStack.RetrieveRoutes();
            foreach (var route in routes)
            {
                await _navigator.NavigateAsync(route);
            }
            _hasAppearedBefore = true;
        }

        public async Task HandleAsync(ViewModelNavigationRequest presentationEvent)
        {
            await NavigateToViewModelAndAddToStack(presentationEvent.Route, presentationEvent.Args.ViewModel);
        }

        public async Task HandleAsync(GoBackRequest presentationEvent)
        {
            if (Navigation.NavigationStack.Any())
            {
                await GoBack();
            }
            presentationEvent.IsHandled = true;
        }

        private async Task NavigateToViewModelAndAddToStack(string route, object viewModel)
        {
            var page = NavigateToViewModel(viewModel);

            await GoForward(route, page);
        }

        private Page NavigateToViewModel(object viewModel)
        {
            var page = _viewLocator.Resolve(viewModel);
            page.BindingContext = viewModel;
            return page;
        }

        private async Task GoForward(string route, Page newPage)
        {
            _navigationStackCache.Push(new NavigationFrameStackItem(route, newPage));

            await Navigation.PushAsync(newPage);

            // Only store this if the app is already running, i.e. not doing startup restore of the stack
            if (_hasAppearedBefore && _navigationStack != null)
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

            public string Route { get; private set; }
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