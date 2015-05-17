using System.Threading.Tasks;

namespace Sample.Shared
{
    public interface IAlbumArtStorage
    {
        string AlbumArtFileName(string albumFolder, uint size);
        Task SaveBitmapAsync(string albumFolder, uint size, string songPath);

        Task<bool> AlbumFolderExists(string albumFolder);
    }
}