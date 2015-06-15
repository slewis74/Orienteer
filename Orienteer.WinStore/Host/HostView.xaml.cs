using Windows.UI.Xaml;

namespace Orienteer.WinStore.Host
{
    public sealed partial class HostView
    {
        public HostView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.Register("HeaderContent", typeof(object), typeof(HostView), new PropertyMetadata(default(object)));

        public object HeaderContent
        {
            get { return (object)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(object), typeof(HostView), new PropertyMetadata(default(object)));

        public object Watermark
        {
            get { return (object)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }
    }
}
