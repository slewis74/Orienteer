using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Orienteer.Pages.Navigation;
using Orienteer.Requests;
using Orienteer.WinPhone.Pages;
using Slew.PresentationBus;

namespace Orienteer.WinPhone
{
    public class PhoneApplicationFrameAdapter : 
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

        public async Task DoStartup()
        {
            if (_hasStarted)
                return;

            var routes = _navigationStack.RetrieveRoutes();
            foreach (var route in routes)
            {
                Debug.WriteLine("Restoring route {0}", route);
                await _navigator.NavigateAsync(route, false);
            }

            _hasStarted = true;
        }

        private void PhoneApplicationFrameOnNavigating(object sender, NavigatingCancelEventArgs navigatingCancelEventArgs)
        {
            var originalString = navigatingCancelEventArgs.Uri.OriginalString;
            Debug.WriteLine("Navigating to {0}", originalString);

            // ignore this event if we're navigating to a page or an external app, rather than a route.
            if (originalString.Contains(".xaml") || originalString.Contains("external"))
            {
                return;
            }
            
            navigatingCancelEventArgs.Cancel = true;

            var route = navigatingCancelEventArgs.Uri.ToRoute();
            _navigator.NavigateAsync(route);
        }

        private void PhoneApplicationFrameOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            var uri = navigationEventArgs.Uri.ToString();
            Debug.WriteLine("Navigated to {0}", uri);

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
                _navigationStack.StoreRoutes(_navigationStackCache.ToArray());
            }

            ClearDataContextForViewPath(uri);
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

            if (PhoneApplicationFrame.Navigate(new Uri(viewAsPath, UriKind.Relative)))
                Debug.WriteLine("PhoneApplicationFrame.Navigate(\"{0}\")", viewAsPath);
            else
                Debug.WriteLine("ERROR PhoneApplicationFrame.Navigate(\"{0}\") FAILED", viewAsPath);

            _navigationStackCache.Push(presentationEvent.Route);
            if (_hasStarted)
            {
                _navigationStack.StoreRoutes(_navigationStackCache.ToArray());
            }

            Debug.WriteLine("Handled route {0}", presentationEvent.Route);
            presentationEvent.IsHandled = true;
        }
    }

    public static class UriExtensions
    {
        public static string ToRoute(this Uri @this)
        {
            var route = @this.ToString();
            return route[0] == '/' ? route.Substring(1) : route;
        }
    }
}