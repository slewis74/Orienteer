using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SlabRt.Controls
{
    public class HeaderedContent : ContentControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(HeaderedContent), new PropertyMetadata(default(string)));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}