using Xamarin.Forms;

namespace FormsSample.Features.Artists.All
{
    public class ArtistsView : ContentPage
    {
        public ArtistsView()
        {
            var layout = new Grid { Padding = 24 };
            layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            layout.RowDefinitions.Add(new RowDefinition());

            var label = new Label
            {
                Text = "Jukebox",
                FontSize = 24
            };
            layout.Children.Add(label, 0 , 0);

            var listView = new ListView();
            listView.SetBinding(ListView.ItemsSourceProperty, new Binding("Artists"));
            listView.ItemTemplate = new DataTemplate(typeof(ArtistCell));

            listView.ItemSelected += (sender, args) => ((ArtistsViewModel) BindingContext).DisplayArtist.Execute(args.SelectedItem);

            layout.Children.Add(listView, 0, 1);

            Content = layout;
        }
    }

    public class ArtistCell : ViewCell
    {
        public ArtistCell()
        {
            var layout = new Grid { HeightRequest = 40 };
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            layout.ColumnDefinitions.Add(new ColumnDefinition());

            var image = new Image();
            layout.Children.Add(image, 0, 0);

            var name = new Label();
            name.SetBinding(Label.TextProperty, new Binding("Name"));
            layout.Children.Add(name, 1, 0);

            View = layout;
        }
    }
}