using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Orienteer.Pages.Navigation;
using Orienteer.Requests;
using Orienteer.WinPhone.Pages;
using Orienteer.Xaml;
using Slew.PresentationBus;

namespace Orienteer.WinPhone
{
    public class PhoneApplicationFrameAdapter : 
        IPhoneApplicationFrameAdapter,
        IHandlePresentationEvent<ViewModelNavigationRequest>
    {
        private readonly IViewLocator _viewLocator;
        private readonly INavigator _navigator;
        private PhoneApplicationFrame _phoneApplicationFrame;
        private readonly string _viewRootNamespace;
        private readonly string _baseFeatureFolder;

        private object _dataContext;

        public PhoneApplicationFrameAdapter(
            string viewRootNamespace,
            string baseFeatureFolder,
            IViewLocator viewLocator,
            INavigator navigator)
        {
            _viewRootNamespace = viewRootNamespace;
            _baseFeatureFolder = baseFeatureFolder;
            _viewLocator = viewLocator;
            _navigator = navigator;
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

        private void PhoneApplicationFrameOnNavigating(object sender, NavigatingCancelEventArgs navigatingCancelEventArgs)
        {
            if (navigatingCancelEventArgs.Uri.OriginalString.Contains(".xaml"))
            {
                return;
            }
            
            navigatingCancelEventArgs.Cancel = true;

            var context = SynchronizationContext.Current;

            Task.Run(() => context.Post(_ =>
            {
                var route = navigatingCancelEventArgs.Uri.ToRoute();
                _navigator.NavigateAsync(route);
            }, null));
        }

        private void PhoneApplicationFrameOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            // The data context is set prior to the Handle method triggering the navigate to the page URI.
            if (navigationEventArgs.NavigationMode == NavigationMode.New)
            {
                ((FrameworkElement) navigationEventArgs.Content).DataContext = _dataContext;
            }
            _dataContext = null;
        }

        public void Handle(ViewModelNavigationRequest presentationEvent)
        {
            var viewType = _viewLocator.DetermineViewType(presentationEvent.Args.ViewModel.GetType());
            var typeInfo = viewType.FullName;
            var viewWithoutRootNamespace = typeInfo.Replace(_viewRootNamespace, string.Empty);
            var viewAsPath = "/" + _baseFeatureFolder + viewWithoutRootNamespace.Replace(".", "/") + ".xaml";

            // store the data context, because we can't control the creation of the page, we have to assign the ViewModel
            // when the OnNavigated fires, meaning the reached the page.
            _dataContext = presentationEvent.Args.ViewModel;

            PhoneApplicationFrame.Navigate(new Uri(viewAsPath, UriKind.Relative));

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