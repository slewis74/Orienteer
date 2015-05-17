using System.Linq;
using Orienteer.Pages;
using Orienteer.Pages.Navigation;
using Sample.Shared;

namespace FormsSample.Features.Albums
{
    public class AlbumController : Controller
    {
        private readonly IMusicProvider _musicProvider;
        private readonly AlbumViewModel.Factory _albumViewModelFactory;

        public AlbumController(
            IMusicProvider musicProvider,
            AlbumViewModel.Factory albumViewModelFactory)
        {
            _musicProvider = musicProvider;
            _albumViewModelFactory = albumViewModelFactory;
        }

        public ActionResult ShowAlbum(string artistName, string albumTitle)
        {
            var artist = _musicProvider.Artists.Single(a => a.Name == artistName);

            return new ViewModelActionResult(() => _albumViewModelFactory(artist, artist.Albums.Single(a => a.Title == albumTitle)));
        }
    }
}