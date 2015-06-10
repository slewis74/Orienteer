using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Orienteer.Pages;
using Orienteer.Pages.Navigation;
using Sample.Shared;
using WinPhoneSample.Features.Albums;
using WinPhoneSample.Features.Artists.All;
using WinPhoneSample.Features.Artists.Single;

namespace WinPhoneSample.Features.Artists
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
            try
            {
                var artists = await _musicProvider.GetArtists();
                return new ViewModelActionResult(() => _artistsViewModelFactory(artists));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: {0} {1}", ex.Message, ex.StackTrace);
                throw;
            }
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