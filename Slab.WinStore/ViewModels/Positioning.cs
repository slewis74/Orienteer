using Windows.Foundation;
using Windows.UI.Xaml;

namespace Slab.WinStore.ViewModels
{
    public static class Positioning
    {
        public static Rect GetElementRect(this FrameworkElement element, int hOffset, int vOffset)
        {
            var rect = GetElementRect(element);
            rect.Y += vOffset;
            rect.X += hOffset;
            return rect;
        }

        public static Rect GetElementRect(this FrameworkElement element)
        {
            var buttonTransform = element.TransformToVisual(null);
            var point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }
    }
}