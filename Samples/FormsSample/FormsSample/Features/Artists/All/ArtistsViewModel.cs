using Orienteer.Data;
using Sample.Shared.Model;

namespace FormsSample.Features.Artists.All
{
    public class ArtistsViewModel
    {
        public delegate ArtistsViewModel Factory(DistinctAsyncObservableCollection<Artist> artists);

        public ArtistsViewModel(
            DistinctAsyncObservableCollection<Artist> artists,
            DisplayArtistCommand displayArtist)
        {
            Artists = artists;
            DisplayArtist = displayArtist;
        }

        public DistinctAsyncObservableCollection<Artist> Artists { get; private set; }
        public DisplayArtistCommand DisplayArtist { get; set; }
    }
}