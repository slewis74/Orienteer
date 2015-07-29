using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Orienteer.Data;
using Orienteer.Pages.Navigation;
using Orienteer.Requests;
using Orienteer.WinPhone.Pages;
using Slew.PresentationBus;

namespace Orienteer.WinPhone
{
    public class PhoneApplicationFrameAdapter : 
        DispatchesToOriginalThreadBase,
        IPhoneApplicationFrameAdapter,
        IHandlePresentationEvent<ViewModelNavigationRequest>
    {
        private readonly IViewLocator _viewLocator;
        private readonly INavigator _navigator;
        private readonly INavigationStack _navigationStack;
        private PhoneApplicationFrame _phoneApplicationFrame;
        private readonly string _viewRootNamespace;
        private readonly string _baseFeatureFolder;
        private readonly Stack<string> _navigationStackCache;
        private readonly Dictionary<string, object> _dataContextCache; 

        private bool _hasStarted;

        public PhoneApplicationFrameAdapter(
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
            _dataContextCache = new Dictionary<string, object>();

            _manifestNavigationPage = "/Orienteer";
        }

        private string _manifestNavigationPage;
        public string ManifestNavigationPage
        {
            get { return _manifestNavigationPage; }
            set { _manifestNavigationPage = "/" + value; }
        }

        public PhoneApplicationFrame PhoneApplicationFrame
        {
            get { return _phoneApplicationFrame; }
            set
            {
                if (_phoneApplicationFrame != null)
                {
                    _phoneApplicationFrame.Navigating -= PhoneApplicationFrameOnNavigating;
                    _phoneApplicationFrame.Navigated -= PhoneApplicationFrameOnNavigated;
                }

                _phoneApplicationFrame = value;

                if (_phoneApplicationFrame != null)
                {
                    _phoneApplicationFrame.Navigating += PhoneApplicationFrameOnNavigating;
                    _phoneApplicationFrame.Navigated += PhoneApplicationFrameOnNavigated;
                }
            }
        }

        private ManualResetEvent _resetEvent;

        public async Task DoStartup()
        {
            if (_hasStarted)
                return;

            var routes = _navigationStack.RetrieveRoutes();

            // restore the routes from a background thread, otherwise the WP Frame will only do the first nav.
            Task.Run(() =>
            {
                using (_resetEvent = new ManualResetEvent(false))
                {
                    foreach (var r in routes)
                    {
                        _resetEvent.Reset();
                        var route = r;
                        // Dispatch the actual nav call back onto the UI thread.
                        DispatchCall(async c =>
                        {
                            await _navigator.NavigateAsync(route, false);
                        });   

                        // wait for this nav to complete before we continue
                        _resetEvent.WaitOne();
                    }
                }
                _resetEvent = null;
                _hasStarted = true;
            });
        }

        private void PhoneApplicationFrameOnNavigating(object sender, NavigatingCancelEventArgs navigatingCancelEventArgs)
        {
            var originalString = navigatingCancelEventArgs.Uri.OriginalString;

            if (originalString == ManifestNavigationPage)
            {
                DoStartup();
                navigatingCancelEventArgs.Cancel = true;
                return;
            }

            // ignore this event if we're navigating to a page or an external app, rather than a route.
            if (originalString.Contains(".xaml") || originalString.Contains("external"))
            {
                return;
            }
            
            navigatingCancelEventArgs.Cancel = true;

            if (originalString == ManifestNavigationPage)
            {
                return;
            }

            // NOTE: the Uri will have Url encoding if it was sourced from a secondary tile.
            var route = navigatingCancelEventArgs.Uri.ToRoute().WithoutUrlEncoding();
            _navigator.NavigateAsync(route);
        }

        private void PhoneApplicationFrameOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            var uri = navigationEventArgs.Uri.ToString();

            if (uri.Contains("external"))
            {
                return;
            }
            
            // The data context is set prior to the Handle method triggering the navigate to the page URI.
            if (navigationEventArgs.NavigationMode == NavigationMode.New)
            {
                ((FrameworkElement)navigationEventArgs.Content).DataContext = _dataContextCache[uri];
            }
            else if (navigationEventArgs.NavigationMode == NavigationMode.Back)
            {
                _navigationStackCache.Pop();
                _navigationStack.StoreRoutes(_navigationStackCache.ToArray());
            }

            ClearDataContextForViewPath(uri);

            // during startup we need to signal that each nav has completed, because the background thread has to WaitOne
            // before trying to restore the next route.
            if (_resetEvent != null)
                _resetEvent.Set();
        }

        private void ClearDataContextForViewPath(string viewPath)
        {
            if (_dataContextCache.ContainsKey(viewPath))
            {
                _dataContextCache.Remove(viewPath);
            }
        }

        public void Handle(ViewModelNavigationRequest presentationEvent)
        {
            var viewType = _viewLocator.DetermineViewType(presentationEvent.Args.ViewModel.GetType());
            var typeInfo = viewType.FullName;
            var viewWithoutRootNamespace = typeInfo.Replace(_viewRootNamespace, string.Empty);
            var viewAsPath = "/" + _baseFeatureFolder + viewWithoutRootNamespace.Replace(".", "/") + ".xaml";

            // store the data context, because we can't control the creation of the page, we have to assign the ViewModel
            // when the OnNavigated fires, meaning the reached the page.
            ClearDataContextForViewPath(viewAsPath);
            _dataContextCache.Add(viewAsPath, presentationEvent.Args.ViewModel);

            PhoneApplicationFrame.Navigate(new Uri(viewAsPath, UriKind.Relative));

            _navigationStackCache.Push(presentationEvent.Route);
            if (_hasStarted)
            {
                _navigationStack.StoreRoutes(_navigationStackCache.ToArray());
            }

            presentationEvent.IsHandled = true;
        }
    }

    public static class UriExtensions
    {
        public static string ToRoute(this Uri @this)
        {
            return @this.ToString().ToRoute();
        }
    }
}