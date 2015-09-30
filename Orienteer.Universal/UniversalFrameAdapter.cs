using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Orienteer.Data;
using Orienteer.Pages.Navigation;
using Orienteer.Requests;
using Orienteer.Universal.Pages;
using PresentationBus;

namespace Orienteer.Universal
{
    public class UniversalFrameAdapter :
        DispatchesToOriginalThreadBase,
        IUniversalFrameAdapter,
        IHandlePresentationCommand<ViewModelNavigationCommand>
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

        public void DoStartup()
        {
            if (_hasStarted)
                return;

            var routes = _navigationStack.RetrieveRoutes();

            // restore the routes from a background thread, otherwise the WP Frame will only do the first nav.
            Task.Run(() =>
            {
                foreach (var r in routes)
                {
                    var route = r;
                    // Dispatch the actual nav call back onto the UI thread.
                    DispatchCall(async c =>
                    {
                        await _navigator.NavigateAsync(route, false);
                    });
                }
                _hasStarted = true;
            });
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
    }

    public interface IUniversalFrameAdapter
    {
        Frame ApplicationFrame { get; set; }

        void DoStartup();
    }
}