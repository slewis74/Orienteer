using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Orienteer.Xaml;

namespace Orienteer.WinStore.Pages
{
    public class ContentSwitchingPage : UserControl
    {
        private readonly Dictionary<PageLayout, FrameworkElement> _viewCache;

        public ContentSwitchingPage()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;

            _viewCache = new Dictionary<PageLayout, FrameworkElement>();

            Loaded += StartLayoutUpdates;

            Unloaded += StopLayoutUpdates;
        }

        public IViewLocator<FrameworkElement> ViewLocator { get; set; }

        public static readonly DependencyProperty NarrowWidthProperty =
            DependencyProperty.Register("NarrowWidth", typeof(int), typeof(ContentSwitchingPage), new PropertyMetadata(500));

        public int NarrowWidth
        {
            get { return (int)GetValue(NarrowWidthProperty); }
            set { SetValue(NarrowWidthProperty, value); }
        }

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
            var pageLayout = PageLayoutProvider.DetermineVisualState(NarrowWidth);
            if (_viewCache.ContainsKey(pageLayout))
                frameworkElement = _viewCache[pageLayout];
            else
            {
                frameworkElement = ViewLocator.Resolve(pageViewModel);
                _viewCache.Add(pageLayout, frameworkElement);
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