using Microsoft.Phone.Controls;

namespace FormsSample.WinPhone
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new FormsSample.App(typeof(MainPage).Assembly));
        }
    }
}
