using Xamarin.Forms;

namespace FormsSample.Features.Albums
{
    public class AlbumView : ContentPage
    {
        public AlbumView()
        {
            var layout = new Grid { Padding = 24 };
            layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            layout.RowDefinitions.Add(new RowDefinition());

            var label = new Label
            {
                FontSize = 24
            };
            label.SetBinding(Label.TextProperty, new Binding("Title"));
            layout.Children.Add(label, 0, 0);

            var listView = new ListView();
            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("Tracks"));
            listView.ItemTemplate = new DataTemplate(typeof(TrackCell));
            layout.Children.Add(listView, 0, 1);

            Content = layout;
        }
    }
    
    public class TrackCell : ViewCell
    {
        public TrackCell()
        {
            var layout = new Grid { HeightRequest = 40 };
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            layout.ColumnDefinitions.Add(new ColumnDefinition());

            var image = new Image();
            layout.Children.Add(image, 0, 0);

            var title = new Label();
            title.SetBinding(Label.TextProperty, new Binding("Title"));
            layout.Children.Add(title, 1, 0);

            View = layout;
        }
    }
}