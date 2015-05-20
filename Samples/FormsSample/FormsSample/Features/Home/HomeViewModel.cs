using Orienteer.Data;
using Sample.Shared.Model;

namespace FormsSample.Features.Home
{
    public class HomeViewModel
    {
        public delegate HomeViewModel Factory(DistinctAsyncObservableCollection<Artist> artists);

        public HomeViewModel(DistinctAsyncObservableCollection<Artist> artists)
        {
        }
    }
}