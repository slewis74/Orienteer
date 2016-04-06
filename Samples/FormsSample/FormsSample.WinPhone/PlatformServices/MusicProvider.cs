using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using Orienteer;
using Orienteer.Data;
using Sample.Shared;
using Sample.Shared.Events;
using PresentationBus;
using Album = Sample.Shared.Model.Album;
using Artist = Sample.Shared.Model.Artist;
using Song = Sample.Shared.Model.Song;

namespace FormsSample.WinPhone.PlatformServices
{
    public class MusicProvider : IMusicProvider
    {
        private readonly IPresentationBus _presentationBus;
        private bool _hasLoaded;

        public MusicProvider(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;

            Artists = new DistinctAsyncObservableCollection<Artist>();
        }

        private DistinctAsyncObservableCollection<Artist> Artists { get; set; }

        private readonly AsyncLock _asyncLock = new AsyncLock();
        public async Task<DistinctAsyncObservableCollection<Artist>> GetArtists()
        {
            using (var releaser = await _asyncLock.LockAsync())
            {
                if (_hasLoaded)
                    return Artists;

                await LoadContent();
                _hasLoaded = true;
            }

            return Artists;
        }

        private async Task LoadContent()
        {
            var artists = new List<Artist>();
            await LoadData(artists);
            
            Artists.StartLargeUpdate();
            Artists.AddRange(artists);
            Artists.CompleteLargeUpdate();

            await _presentationBus.PublishAsync(new AlbumDataLoaded(Artists));
        }

        /// <summary>
        /// Loads the existing content from the application data
        /// </summary>
        private async Task<bool> LoadData(List<Artist> artists)
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            if (files.Any(f => f.Name == "Artists") == false)
                return false;

            try
            {
                var artistsFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Artists", CreationCollisionOption.OpenIfExists);

                var file = await artistsFile.OpenStreamForReadAsync();

                // Read the data.
                string data;
                using (var streamReader = new StreamReader(file))
                {
                    data = streamReader.ReadToEnd();
                }

                var artistsData = JsonConvert.DeserializeObject<IEnumerable<Artist>>(data);

                foreach (var artist in artistsData)
                {
                    artists.Add(artist);
                }

                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public async Task<bool> ReScanMusicLibrary()
        {
            // copy to a separate list while loaded, to stop the UI flickering when reading lots of new data
            var artists = (await GetArtists()).ToList();
            var newTracks = await ScanMusicLibraryFolder(artists);

            if (newTracks)
            {
                Artists.Replace(artists);
                await SaveData(Artists);
            }

            return true;
        }

        private async Task<bool> ScanMusicLibraryFolder(IList<Artist> artists)
        {
            var newData = false;

            using (var library = new MediaLibrary())
            {
                foreach (var libArtist in library.Artists)
                {
                    var artist = artists.FirstOrDefault(x => string.Compare(x.Name, libArtist.Name, StringComparison.CurrentCultureIgnoreCase) == 0);
                    if (artist == null)
                    {
                        artist = new Artist
                        {
                            Name = libArtist.Name
                        };
                        newData = true;
                        artists.Add(artist);
                    }

                    foreach (var libAlbum in libArtist.Albums)
                    {
                        var album = artist.Albums.FirstOrDefault(x =>
                                string.Compare(x.Title, libAlbum.Name, StringComparison.CurrentCultureIgnoreCase) == 0);
                        if (album == null)
                        {
                            album = new Album
                            {
                                Title = libAlbum.Name
                                //Folder = Path.Combine(relativeFolder, fileProps.Album.RemoveIllegalChars())
                            };
                            artist.Albums.Add(album);
                            newData = true;
                        }

                        foreach (var libSong in libAlbum.Songs)
                        {
                            var song = album.Songs.FirstOrDefault(s => s.TrackNumber == libSong.TrackNumber);
                            if (song == null)
                            {
                                song = new Song
                                {
                                    Title = libSong.Name,
                                    TrackNumber = (uint)libSong.TrackNumber,
                                    Duration = libSong.Duration
                                };
                                album.Songs.Add(song);
                                newData = true;
                                await _presentationBus.PublishAsync(new SongLoadedEvent(album, song));
                                // save new entry to app storage
                            }
                            
                        }
                    }
                    
                }
            }
            return newData;
        }

        private async Task SaveData(IEnumerable<Artist> artists)
        {
            var artistsData = JsonConvert.SerializeObject(artists);

            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(artistsData.ToCharArray());

            var artistsFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Artists", CreationCollisionOption.ReplaceExisting);

            // Write the data from the textbox.
            using (var s = await artistsFile.OpenStreamForWriteAsync())
            {
                s.Write(fileBytes, 0, fileBytes.Length);
            }

            Debug.WriteLine("Save completed");
        }
    }

    public static class FolderPathStringExtensions
    {
        public static string RemoveIllegalChars(this string @this)
        {
            return
                @this.Replace("<", string.Empty)
                    .Replace(">", string.Empty)
                    .Replace(":", string.Empty)
                    .Replace(".", string.Empty)
                    .Replace("/", string.Empty)
                    .Replace("?", string.Empty)
                    .Replace("|", string.Empty)
                    .Replace("*", string.Empty)
                    .Trim();
        }
    }
}