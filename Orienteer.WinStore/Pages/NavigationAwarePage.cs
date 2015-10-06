using System;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Orienteer.Messages;
using Orienteer.WinStore.Host;

namespace Orienteer.WinStore.Pages
{
    public class NavigationAwarePage : Page
    {
        public event EventHandler<FrameworkElement> ProvideAppBarContent;

        public NavigationAwarePage()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;

            // When this page is part of the visual tree make two changes:
            // 1) Map application view state to visual state for the page
            // 2) Handle keyboard and mouse navigation requests
            Loaded += (sender, e) =>
                          {
                              // Keyboard and mouse navigation only apply when occupying the entire window
                              // ReSharper disable CompareOfFloatsByEqualityOperator
                              if (ActualHeight != Window.Current.Bounds.Height || ActualWidth != Window.Current.Bounds.Width) return;
                              // ReSharper restore CompareOfFloatsByEqualityOperator

                              // Listen to the window directly so focus isn't required
                              Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CoreDispatcherAcceleratorKeyActivated;
                              Window.Current.CoreWindow.PointerPressed += CoreWindowPointerPressed;

                          };

            // Undo the same changes when the page is no longer visible
            Unloaded += (sender, e) =>
                            {
                                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= CoreDispatcherAcceleratorKeyActivated;
                                Window.Current.CoreWindow.PointerPressed -= CoreWindowPointerPressed;
                            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                DataContext = e.Parameter;
            }

            FrameworkElement content = null;
            // ReSharper disable SuspiciousTypeConversion.Global
            var haveBottomAppBar = this as IHaveBottomAppBar;
            // ReSharper restore SuspiciousTypeConversion.Global
            if (haveBottomAppBar != null)
            {
                var contentType = haveBottomAppBar.BottomAppBarContentType;
                content = (FrameworkElement)Activator.CreateInstance(contentType);
                content.DataContext = DataContext;
            }
            OnProvideAppBarContent(content);
        }

        private void OnProvideAppBarContent(FrameworkElement frameworkElement)
        {
            if (ProvideAppBarContent == null) return;
            ProvideAppBarContent(this, frameworkElement);
        }


        public virtual void GoHome(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as HostViewModel;
            if (viewModel == null)
                return;

            viewModel.PresentationBus.Send(new GoHomeCommand());
        }

        public virtual void GoBack(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as HostViewModel;
            if (viewModel == null)
                return;

            viewModel.PresentationBus.Send(new GoBackCommand());
        }

        /// <summary>
        /// Invoked on every keystroke, including system keys such as Alt key combinations, when
        /// this page is active and occupies the entire window.  Used to detect keyboard navigation
        /// between pages even when the page itself doesn't have focus.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="args">Event data describing the conditions that led to the event.</param>
        private void CoreDispatcherAcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            var virtualKey = args.VirtualKey;

            // Only investigate further when Left, Right, or the dedicated Previous or Next keys
            // are pressed
            if ((args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                 args.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                 (int)virtualKey == 166 || (int)virtualKey == 167))
            {
                var coreWindow = Window.Current.CoreWindow;
                const CoreVirtualKeyStates downState = CoreVirtualKeyStates.Down;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    // When the previous key or Alt+Left are pressed navigate back
                    args.Handled = true;
                    GoBack(this, new RoutedEventArgs());
                }
            }
        }

        /// <summary>
        /// Invoked on every mouse click, touch screen tap, or equivalent interaction when this
        /// page is active and occupies the entire window.  Used to detect browser-style next and
        /// previous mouse button clicks to navigate between pages.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="args">Event data describing the conditions that led to the event.</param>
        private void CoreWindowPointerPressed(CoreWindow sender,
                                              PointerEventArgs args)
        {
            var properties = args.CurrentPoint.Properties;

            // Ignore button chords with the left, right, and middle buttons
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            // If back or foward are pressed (but not both) navigate appropriately
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                args.Handled = true;
                if (backPressed) GoBack(this, new RoutedEventArgs());
            }
        }
        
    }
}