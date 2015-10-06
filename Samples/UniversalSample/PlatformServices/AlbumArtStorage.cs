using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Sample.Shared;

namespace UniversalSample.PlatformServices
{
    public class AlbumArtStorage : IAlbumArtStorage
    {
        public async Task SaveBitmapAsync(string albumFolder, uint size, string songPath)
        {
            var songFile = await StorageFile.GetFileFromPathAsync(songPath);

            using (var thumbnail = await songFile.GetThumbnailAsync(ThumbnailMode.MusicView, size) ??
                                   await songFile.GetThumbnailAsync(ThumbnailMode.VideosView, size))
            {
                if (thumbnail == null)
                    return;

                var reader = new DataReader(thumbnail);
                var fileLength = (uint)thumbnail.Size;
                await reader.LoadAsync(fileLength);

                var buffer = reader.ReadBuffer(fileLength);

                var memStream = new InMemoryRandomAccessStream();

                await memStream.WriteAsync(buffer);
                await memStream.FlushAsync();
                memStream.Seek(0);

                await ApplicationData.Current.LocalFolder.CreateFolderAsync(AlbumArtFolderName(albumFolder), CreationCollisionOption.OpenIfExists);

                var albumArtFileName = AlbumArtFileName(albumFolder, size);
                var outputFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(albumArtFileName, CreationCollisionOption.ReplaceExisting);

                // http://social.msdn.microsoft.com/Forums/windowsapps/en-US/1dda3a15-d299-40e0-b668-ec690a683f6e/how-to-resize-an-image-as-storagefile?forum=winappswithcsharp
                var decoder = await BitmapDecoder.CreateAsync(memStream);
                var transform = new BitmapTransform { ScaledHeight = size, ScaledWidth = size };
                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Rgba8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.RespectExifOrientation,
                    ColorManagementMode.DoNotColorManage);

                using (var destinationStream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, destinationStream);
                    encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, size, size, 96, 96, pixelData.DetachPixelData());
                    await encoder.FlushAsync();
                }
            }
        }

        public async Task<bool> AlbumFolderExists(string albumFolder)
        {
            var albumArtFolderName = AlbumArtFolderName(albumFolder);
            var folders = await ApplicationData.Current.LocalFolder.GetFoldersAsync();
            return folders.Any(f => f.Name == albumArtFolderName);
        }

        private static string AlbumArtFolderName(string albumFolder)
        {
            return Path.Combine("AlbumArt", albumFolder);
        }

        public string AlbumArtFileName(string albumFolder, uint size)
        {
            return Path.Combine(AlbumArtFolderName(albumFolder), "AlbumArt" + size + "x" + size + ".jpg");
        }
    }
}