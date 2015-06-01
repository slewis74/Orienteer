using System.Threading.Tasks;
using Orienteer.Data;
using Sample.Shared.Model;

namespace Sample.Shared
{
    public interface IMusicProvider
    {
        Task<DistinctAsyncObservableCollection<Artist>> GetArtists();

        Task<bool> ReScanMusicLibrary();
    }
}