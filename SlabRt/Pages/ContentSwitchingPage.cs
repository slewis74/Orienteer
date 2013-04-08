using System;
using System.Collections.Generic;
using Slab.Pages;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SlabRt.Pages
{
    public class ContentSwitchingPage : UserControl
    {
        private readonly Dictionary<ApplicationViewState, FrameworkElement> _viewCache;

        public ContentSwitchingPage()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;

            _viewCache = new Dictionary<ApplicationViewState, FrameworkElement>();

            Loaded += StartLayoutUpdates;

            Unloaded += StopLayoutUpdates;
        }

        public IViewLocator ViewLocator { get; set; }

        public static readonly DependencyProperty PageCommandsPanelProperty =
            DependencyProperty.Register("PageCommandsPanel", typeof(Panel), typeof(ContentSwitchingPage), new PropertyMetadata(default(Panel)));

        public Panel PageCommandsPanel
        {
            get { return (Panel)GetValue(PageCommandsPanelProperty); }
            set { SetValue(PageCommandsPanelProperty, value); }
        }

        private void StartLayoutUpdates(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += WindowSizeChanged;
            WindowSizeChanged(this, null);
        }

        private void StopLayoutUpdates(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= WindowSizeChanged;
        }

        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            var pageViewModel = DataContext;
            if (pageViewModel == null) return;

            FrameworkElement frameworkElement;
            if (_viewCache.ContainsKey(ApplicationView.Value))
                frameworkElement = _viewCache[ApplicationView.Value];
            else
            {
                frameworkElement = ViewLocator.Resolve(pageViewModel, ApplicationView.Value);
                _viewCache.Add(ApplicationView.Value, frameworkElement);
            }

            Content = frameworkElement;
            ResetAppBarContentForView(frameworkElement);
        }

        private void ResetAppBarContentForView(FrameworkElement view)
        {
            PageCommandsPanel.Children.Clear();
            var hasAppBarContent = view as IHaveBottomAppBar;
            if (hasAppBarContent != null)
            {
                var frameworkElement = (FrameworkElement)Activator.CreateInstance(hasAppBarContent.BottomAppBarContentType);
                frameworkElement.DataContext = view.DataContext;

                PageCommandsPanel.Children.Add(frameworkElement);
            }
        }
    }
}