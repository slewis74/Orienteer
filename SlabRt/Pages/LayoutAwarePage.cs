using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SlabRt.Pages
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public class LayoutAwarePage : NavigationAwarePage
    {
        private List<Control> _layoutAwareControls;

        public LayoutAwarePage()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;

            Loaded += StartLayoutUpdates;

            Unloaded += StopLayoutUpdates;
        }

        public static readonly DependencyProperty NarrowWidthProperty =
            DependencyProperty.Register("NarrowWidth", typeof (int), typeof (LayoutAwarePage), new PropertyMetadata(500));

        public int NarrowWidth
        {
            get { return (int) GetValue(NarrowWidthProperty); }
            set { SetValue(NarrowWidthProperty, value); }
        }
       
        public void StartLayoutUpdates(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            if (_layoutAwareControls == null)
            {
                // Start listening to view state changes when there are controls interested in updates
                Window.Current.SizeChanged += WindowSizeChanged;
                _layoutAwareControls = new List<Control>();
            }
            _layoutAwareControls.Add(control);

            // Set the initial visual state of the control
            VisualStateManager.GoToState(control, DetermineVisualState(), false);
        }

        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            InvalidatePageLayout();
        }

        public void StopLayoutUpdates(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null || _layoutAwareControls == null) return;
            _layoutAwareControls.Remove(control);
            if (_layoutAwareControls.Count == 0)
            {
                // Stop listening to view state changes when no controls are interested in updates
                _layoutAwareControls = null;
                Window.Current.SizeChanged -= WindowSizeChanged;
            }
        }

        protected virtual string DetermineVisualState()
        {
            return PageLayoutProvider.DetermineVisualState(NarrowWidth).ToString();
        }

        public void InvalidatePageLayout()
        {
            if (_layoutAwareControls == null) return;
            
            var visualState = DetermineVisualState();
            foreach (var layoutAwareControl in _layoutAwareControls)
            {
                VisualStateManager.GoToState(layoutAwareControl, visualState, false);
            }
        }
    }
}
