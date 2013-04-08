using System;
using Slab.Pages;
using Slab.Pages.Navigation;
using Slab.PresentationBus;
using SlabRt.Pages.Settings;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace SlabRt.Pages.Navigation
{
    public class RtNavigator : Navigator, IRtNavigator
    {
        private Popup _settingsPopup;
        private readonly int _settingsWidth;

        public RtNavigator(IPresentationBus presentationBus, IControllerInvoker controllerInvoker) : base(presentationBus, controllerInvoker)
        {
            _settingsWidth = 346;
        }

        protected override void DoNavigate(ControllerInvokerResult controllerResult)
        {
            var result = controllerResult.Result;
            var settingsResult = result as ISettingsPageActionResult;
            if (settingsResult != null)
            {
                DoPopup(settingsResult);
                return;
            }

            base.DoNavigate(controllerResult);
        }

        public void SettingsNavigateBack()
        {
            if (_settingsPopup != null)
            {
                _settingsPopup.IsOpen = false;
            }

            // If the app is not snapped, then the back button shows the Settings pane again.
            if (Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                SettingsPane.Show();
            }
        }

        private void DoPopup(ISettingsPageActionResult settingsResult)
        {
            var windowBounds = Window.Current.Bounds;

            // Create a Popup window which will contain our flyout.
            _settingsPopup = new Popup();
            _settingsPopup.Closed += OnPopupClosed;

            Window.Current.Activated += OnWindowActivated;

            _settingsPopup.IsLightDismissEnabled = true;

            _settingsPopup.Width = _settingsWidth;
            _settingsPopup.Height = windowBounds.Height;

            // Add the proper animation for the panel.
            _settingsPopup.ChildTransitions = new TransitionCollection
                                                  {
                                                      new PaneThemeTransition
                                                          {
                                                              Edge = (SettingsPane.Edge == SettingsEdgeLocation.Right)
                                                                         ? EdgeTransitionLocation.Right
                                                                         : EdgeTransitionLocation.Left
                                                          }
                                                  };

            // Create a SettingsFlyout the same dimenssions as the Popup.
            var view = (FrameworkElement)Activator.CreateInstance(settingsResult.PageType);
            view.DataContext = settingsResult.Parameter;

            view.Width = _settingsWidth;
            view.Height = windowBounds.Height;

            // Place the SettingsFlyout inside our Popup window.
            var settingsPopupViewModel = new SettingsPopupViewModel(new SettingsBackCommand(this));

            _settingsPopup.Child = new SettingsPopupView
            {
                DataContext = settingsPopupViewModel,
                HeaderBackground = new SolidColorBrush(Colors.Green),
                Background = new SolidColorBrush(Colors.LightGreen),
                Content = view
            };

            // Let's define the location of our Popup.
            _settingsPopup.SetValue(Canvas.LeftProperty, SettingsPane.Edge == SettingsEdgeLocation.Right ? (windowBounds.Width - _settingsWidth) : 0);
            _settingsPopup.SetValue(Canvas.TopProperty, 0);
            _settingsPopup.IsOpen = true;
        }

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                _settingsPopup.IsOpen = false;
            }
        }

        void OnPopupClosed(object sender, object e)
        {
            Window.Current.Activated -= OnWindowActivated;
        }

    }
}