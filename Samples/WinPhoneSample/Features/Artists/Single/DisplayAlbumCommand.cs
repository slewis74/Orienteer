using Orienteer.Pages.Navigation;
using Orienteer.Xaml.ViewModels;
using Sample.Shared.Model;
using WinPhoneSample.Features.Albums;

namespace WinPhoneSample.Features.Artists.Single
{
    public class DisplayAlbumCommand : NavigationCommand<Album>
    {
        private readonly Artist _artist;

        public DisplayAlbumCommand(INavigator navigator, Artist artist)
            : base(navigator)
        {
            _artist = artist;
        }

        public override async void Execute(Album album)
        {
            await Navigator.NavigateAsync<AlbumController>(c => c.ShowAlbum(_artist.Name, album.Title));
        }
    }
}