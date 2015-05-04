using Xamarin.Forms;

namespace FormsSample.Home
{
    public class HomeView : ContentPage
    {
        public HomeView()
        {
            var layout = new Grid();

            var label = new Label {Text = "Hello Forms Sample"};
            layout.Children.Add(label);

            Content = layout;
        }
    }
}