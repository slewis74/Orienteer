using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Orienteer.Data;
using Orienteer.Messages;
using Orienteer.Pages.Navigation;
using Orienteer.Universal.Messages;
using Orienteer.Universal.Pages;
using PresentationBus;

namespace Orienteer.Universal
{
    public class UniversalFrameAdapter :
        DispatchesToUIThread,
        IUniversalFrameAdapter,
        IHandlePresentationCommand<ViewModelNavigationCommand>,
        IHandlePresentationCommand<GoBackCommand>,
        IHandlePresentationRequest<CanGoBackRequest, CanGoBackResponse>
    {
        private readonly IViewLocator _viewLocator;
        private readonly INavigator _navigator;
        private readonly INavigationStack _navigationStack;
        private Frame _applicationFrame;
        private readonly string _viewRootNamespace;
        private readonly string _baseFeatureFolder;
        private readonly Stack<string> _navigationStackCache;

        private bool _hasStarted;

        public UniversalFrameAdapter(
            string viewRootNamespace,
            string baseFeatureFolder,
            IViewLocator viewLocator,
            INavigator navigator,
            INavigationStack navigationStack)
        {
            _viewRootNamespace = viewRootNamespace;
            _baseFeatureFolder = baseFeatureFolder;
            _viewLocator = viewLocator;
            _navigator = navigator;
            _navigationStack = navigationStack;

            _navigationStackCache = new Stack<string>();
        }

        public Frame ApplicationFrame
        {
            get { return _applicationFrame; }
            set
            {
                if (_applicationFrame != null)
                {
                    _applicationFrame.Navigated -= ApplicationFrameOnNavigated;
                }

                _applicationFrame = value;

                if (_applicationFrame != null)
                {
                    _applicationFrame.Navigated += ApplicationFrameOnNavigated;
                }
            }
        }

        public async Task DoStartup()
        {
            if (_hasStarted)
                return;

            var routes = _navigationStack.RetrieveRoutes();

            // restore the routes from a background thread, otherwise the WP Frame will only do the first nav.
            foreach (var r in routes)
            {
                var route = r;
                await _navigator.NavigateAsync(route, false);
            }
            _hasStarted = true;
        }

        private void ApplicationFrameOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            var page = navigationEventArgs.Content as FrameworkElement;
            if (page != null)
            {
                page.DataContext = navigationEventArgs.Parameter;
            }
        }

        public void Handle(ViewModelNavigationCommand command)
        {
            var viewType = _viewLocator.DetermineViewType(command.Args.ViewModel.GetType());
            var typeInfo = viewType.FullName;
            var viewWithoutRootNamespace = typeInfo.Replace(_viewRootNamespace, string.Empty);
            var viewAsPath = "/" + _baseFeatureFolder + viewWithoutRootNamespace.Replace(".", "/") + ".xaml";

            ApplicationFrame.Navigate(viewType, command.Args.ViewModel);

            _navigationStackCache.Push(command.Route);
            if (_hasStarted)
            {
                _navigationStack.StoreRoutes(_navigationStackCache.ToArray());
            }
        }

        public void Handle(GoBackCommand presentationCommand)
        {
            ApplicationFrame.GoBack();
            _navigationStackCache.Pop();
            _navigationStack.StoreRoutes(_navigationStackCache.ToArray());
        }

        public CanGoBackResponse Handle(CanGoBackRequest request)
        {
            return new CanGoBackResponse { CanGoBack = ApplicationFrame.CanGoBack };
        }
    }

    public interface IUniversalFrameAdapter
    {
        Frame ApplicationFrame { get; set; }

        Task DoStartup();
    }
}