using Orienteer.Data;
using Sample.Shared.Model;

namespace FormsSample.Features.Home
{
    public class HomeViewModel
    {
        public delegate HomeViewModel Factory(DistinctAsyncObservableCollection<Artist> artists);

        public HomeViewModel(
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