using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orienteer.Pages.Navigation;
using Orienteer.Requests;
using Slew.PresentationBus;
using Xamarin.Forms;

namespace Orienteer.Forms.Pages
{
    public class OrienteerNavigationPage :
        NavigationPage,
        IHandlePresentationEventAsync<ViewModelNavigationRequest>,
        IHandlePresentationEventAsync<GoBackRequest>
    {
        private readonly INavigator _navigator;
        private readonly IViewLocator _viewLocator;
        private readonly INavigationStack _navigationStack;
        private bool _hasAppearedBefore;

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
                await _navigator.NavigateAsync(route, false);
            }
            _hasAppearedBefore = true;
        }

        public async Task HandleAsync(ViewModelNavigationRequest presentationEvent)
        {
            await NavigateToViewModelAndAddToStack(presentationEvent.Route, presentationEvent.Args.ViewModel, presentationEvent.Animated);
            presentationEvent.IsHandled = true;
        }

        public async Task HandleAsync(GoBackRequest presentationEvent)
        {
            if (Navigation.NavigationStack.Any())
            {
                await GoBack();
            }
            presentationEvent.IsHandled = true;
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