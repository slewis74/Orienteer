using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Slab.WinStore.Controls
{
    public class LabelledContent : ContentControl
    {
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabelledContent), new PropertyMetadata(default(string)));

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(double), typeof(LabelledContent), new PropertyMetadata(default(double)));

        public double LabelWidth
        {
            get { return (double)GetValue(LabelWidthProperty); }
            set { SetValue(LabelWidthProperty, value); }
        }

        public static readonly DependencyProperty LabelTextStyleProperty =
            DependencyProperty.Register("LabelTextStyle", typeof (Style), typeof (LabelledContent), new PropertyMetadata(default(Style)));

        public Style LabelTextStyle
        {
            get { return (Style) GetValue(LabelTextStyleProperty); }
            set { SetValue(LabelTextStyleProperty, value); }
        }
    }
}