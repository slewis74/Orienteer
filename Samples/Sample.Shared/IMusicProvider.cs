using System.Threading.Tasks;
using Orienteer.Data;
using Sample.Shared.Model;

namespace Sample.Shared
{
    public interface IMusicProvider
    {
        DistinctAsyncObservableCollection<Artist> Artists { get; }

        Task LoadContent();

        Task<bool> ReScanMusicLibrary();
    }
}