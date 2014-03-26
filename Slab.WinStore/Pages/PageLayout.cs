using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Slab.WinStore.Pages
{
    public enum PageLayout
    {
        Portrait,
        Landscape,
        Narrow
    }

    public static class PageLayoutProvider
    {
        public static PageLayout DetermineVisualState(int narrowWidth = 500)
        {
            var pageLayout = PageLayout.Landscape;

            var applicationView = ApplicationView.GetForCurrentView();

            if (applicationView.IsFullScreen)
            {
                if (applicationView.Orientation == ApplicationViewOrientation.Portrait)
                {
                    pageLayout = PageLayout.Portrait;
                }
            }
            else
            {
                var pageWidth = Window.Current.Bounds.Width;

                if (pageWidth < narrowWidth)
                {
                    pageLayout = PageLayout.Narrow;
                }
            }

            return pageLayout;
        }
        
    }
}