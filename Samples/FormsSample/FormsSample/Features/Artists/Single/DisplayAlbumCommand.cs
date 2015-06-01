using FormsSample.Features.Albums;
using Orienteer.Pages.Navigation;
using Orienteer.Xaml.ViewModels;
using Sample.Shared.Model;

namespace FormsSample.Features.Artists.Single
{
    public class DisplayAlbumCommand : NavigationCommand<Album>
    {
        private readonly Artist _artist;

        public DisplayAlbumCommand(INavigator navigator, Artist artist)
            : base(navigator)
        {
            _artist = artist;
        }

        public override void Execute(Album album)
        {
            Navigator.NavigateAsync<AlbumController>(c => c.ShowAlbum(_artist.Name, album.Title));
        }
    }
}