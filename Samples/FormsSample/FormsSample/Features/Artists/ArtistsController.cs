using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FormsSample.Features.Albums;
using FormsSample.Features.Artists.All;
using FormsSample.Features.Artists.Single;
using Orienteer.Pages;
using Orienteer.Pages.Navigation;
using Sample.Shared;

namespace FormsSample.Features.Artists
{
    public class ArtistsController : Controller
    {
        private readonly IMusicProvider _musicProvider;
        private readonly ArtistsViewModel.Factory _artistsViewModelFactory;
        private readonly AlbumViewModel.Factory _albumViewModelFactory;
        private readonly ArtistViewModel.Factory _artistViewModelFactory;

        public ArtistsController(
            IMusicProvider musicProvider,
            ArtistsViewModel.Factory artistsViewModelFactory,
            AlbumViewModel.Factory albumViewModelFactory,
            ArtistViewModel.Factory artistViewModelFactory)
        {
            _musicProvider = musicProvider;
            _artistsViewModelFactory = artistsViewModelFactory;
            _albumViewModelFactory = albumViewModelFactory;
            _artistViewModelFactory = artistViewModelFactory;
        }

        public async Task<ActionResult> ShowAll()
        {
            var artists = await _musicProvider.GetArtists();
            return new ViewModelActionResult(() => _artistsViewModelFactory(artists));
        }

        public async Task<ActionResult> ShowArtist(string name)
        {
            var artists = await _musicProvider.GetArtists();
            var artist = artists.SingleOrDefault(a => a.Name == name);

            if (artist == null)
            {
                Debug.WriteLine("Artist {0} not found, total artists {1}", name, artists.Count);
                return await ShowAll();
            }
            if (artist.Albums.Count == 1)
                return new ViewModelActionResult(() => _albumViewModelFactory(artist, artist.Albums.Single()));
            return new ViewModelActionResult(() => _artistViewModelFactory(artist));
        } 
    }
}