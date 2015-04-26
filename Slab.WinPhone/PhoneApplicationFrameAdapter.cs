using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Slab.Pages.Navigation;
using Slab.Requests;
using Slab.Xaml;
using Slew.PresentationBus;

namespace Slab.WinPhone
{
    public class PhoneApplicationFrameAdapter<TDefaultViewModel> : 
        IPhoneApplicationFrameAdapter,
        IHandlePresentationEvent<ViewModelNavigationRequest>
    {
        private readonly IViewLocator<FrameworkElement> _viewLocator;
        private readonly INavigator _navigator;
        private PhoneApplicationFrame _phoneApplicationFrame;
        private object _dataContext;
        private bool _redirecting;
        private readonly string _baseFeatureNamespace;
        private readonly Func<TDefaultViewModel> _defaultViewModelFactory;

        public PhoneApplicationFrameAdapter(
            string baseFeatureNamespace,
            Func<TDefaultViewModel> defaultViewModelFactory,
            IViewLocator<FrameworkElement> viewLocator,
            INavigator navigator)
        {
            _baseFeatureNamespace = baseFeatureNamespace;
            _defaultViewModelFactory = defaultViewModelFactory;
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
                _redirecting = false;
                return;
            }
            
            _redirecting = true;
            navigatingCancelEventArgs.Cancel = true;

            var context = SynchronizationContext.Current;

            Task.Run(() => context.Post(_ =>
            {
                var route = navigatingCancelEventArgs.Uri.ToString().Substring(1);
                _navigator.NavigateAsync(route);
            }, null));
        }

        private void PhoneApplicationFrameOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            if (navigationEventArgs.NavigationMode == NavigationMode.New)
            {
                ((FrameworkElement) navigationEventArgs.Content).DataContext = _dataContext ?? _defaultViewModelFactory();
            }
            _dataContext = null;
        }

        public void Handle(ViewModelNavigationRequest presentationEvent)
        {
            var viewType = _viewLocator.DetermineViewType(presentationEvent.Args.ViewModel.GetType());
            var typeInfo = viewType.FullName;
            var viewWithoutRootNamespace = typeInfo.Replace(_baseFeatureNamespace, string.Empty);
            var viewAsPath = viewWithoutRootNamespace.Replace(".", "/") + ".xaml";

            _dataContext = presentationEvent.Args.ViewModel;

            PhoneApplicationFrame.Navigate(new Uri(viewAsPath, UriKind.Relative));
        }
    }
}