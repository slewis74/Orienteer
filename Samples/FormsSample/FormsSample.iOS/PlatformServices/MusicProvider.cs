using System.Threading.Tasks;
using Orienteer.Data;
using Sample.Shared;
using Sample.Shared.Model;

namespace FormsSample.iOS.PlatformServices
{
    public class MusicProvider : IMusicProvider
    {
        public DistinctAsyncObservableCollection<Artist> Artists { get; private set; }

        public Task LoadContent()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ReScanMusicLibrary()
        {
            throw new System.NotImplementedException();
        }
    }
}