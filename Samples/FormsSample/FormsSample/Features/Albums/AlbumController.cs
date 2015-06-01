using System.Linq;
using System.Threading.Tasks;
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

        public async Task<ActionResult> ShowAlbum(string artistName, string albumTitle)
        {
            var artist = (await _musicProvider.GetArtists()).Single(a => a.Name == artistName);

            return new ViewModelActionResult(() => _albumViewModelFactory(artist, artist.Albums.Single(a => a.Title == albumTitle)));
        }
    }
}