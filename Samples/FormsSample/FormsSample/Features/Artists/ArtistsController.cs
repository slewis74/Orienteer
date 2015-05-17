using System.Linq;
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

        public ActionResult ShowAll()
        {
            return new ViewModelActionResult(() => _artistsViewModelFactory());
        }

        public ActionResult ShowArtist(string name)
        {
            var artist = _musicProvider.Artists.SingleOrDefault(a => a.Name == name);

            if (artist == null)
                return ShowAll();
            if (artist.Albums.Count == 1)
                return new ViewModelActionResult(() => _albumViewModelFactory(artist, artist.Albums.Single()));
            return new ViewModelActionResult(() => _artistViewModelFactory(artist));
        } 
    }
}