using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Provider;
using Newtonsoft.Json;
using Orienteer.Data;
using Sample.Shared;
using Sample.Shared.Model;

namespace FormsSample.Droid.PlatformServices
{
    public class MusicProvider : IMusicProvider
    {
     public MusicProvider()
        {
            Artists = new DistinctAsyncObservableCollection<Artist>();
        }

        public DistinctAsyncObservableCollection<Artist> Artists { get; private set; }

        public async Task LoadContent()
        {
            var artists = new List<Artist>();
            await LoadData(artists);

            Artists.StartLargeUpdate();
            Artists.AddRange(artists);
            Artists.CompleteLargeUpdate();
        }

        /// <summary>
        /// Loads the existing content from the application data
        /// </summary>
        private async Task<bool> LoadData(List<Artist> artists)
        {
            var storageFile = IsolatedStorageFile.GetUserStoreForApplication();
            if (storageFile.FileExists("Artists") == false)
                return false;

            try
            {
                IEnumerable<Artist> artistsData;
                using (var artistsFile = new IsolatedStorageFileStream("Artists", FileMode.Open))
                {
                    using (var textStream = new StreamReader(artistsFile))
                    {
                        var data = await textStream.ReadToEndAsync();
                        artistsData = JsonConvert.DeserializeObject<IEnumerable<Artist>>(data);
                    }
                }

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
            var artists = Artists.ToList();
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
            var foundNewItems = false;

            var uri = MediaStore.Audio.Media.ExternalContentUri;
            string[] projection = {
                    MediaStore.Audio.Media.InterfaceConsts.Id,
                    MediaStore.Audio.Media.InterfaceConsts.AlbumId,
                    MediaStore.Audio.Media.InterfaceConsts.Title,
                    MediaStore.Audio.Media.InterfaceConsts.Artist,
                    MediaStore.Audio.Media.InterfaceConsts.Album,
                    MediaStore.Audio.Media.InterfaceConsts.Duration,
                    MediaStore.Audio.Media.InterfaceConsts.Track
                };

            var cursor = Application.Context.ContentResolver.Query(uri, projection, null, null, null);
            if (cursor.MoveToFirst())
            {
                do
                {
                    var name = cursor.GetString(3);
                    var artist = artists.FirstOrDefault(a => a.Name == name);
                    if (artist == null)
                    {
                        artist = new Artist
                        {
                            Name = name
                        };
                        artists.Add(artist);
                        foundNewItems = true;
                    }

                    var title = cursor.GetString(4);
                    var album = artist.Albums.FirstOrDefault(a => a.Title == title);
                    if (album == null)
                    {
                        album = new Album
                        {
                            Title = title
                        };
                        artist.Albums.Add(album);
                        foundNewItems = true;
                    }

                    var songTitle = cursor.GetString(2);
                    var song = album.Songs.FirstOrDefault(s => s.Title == songTitle);
                    if (song == null)
                    {
                        song = new Song
                        {
                            Title = songTitle,
                            TrackNumber = uint.Parse(cursor.GetString(6)),
                            Duration = TimeSpan.FromMilliseconds(int.Parse(cursor.GetString(5)))
                        };
                        album.Songs.Add(song);
                        foundNewItems = true;
                    }
                } while (cursor.MoveToNext());
            }

            return foundNewItems;
        }

        private async Task SaveData(IEnumerable<Artist> artists)
        {
            var artistsData = JsonConvert.SerializeObject(artists);

            using (var artistsFile = new IsolatedStorageFileStream("Artists", FileMode.Create))
            {
                using (var textStream = new StreamWriter(artistsFile))
                {
                    await textStream.WriteAsync(artistsData);
                }
            }

            Debug.WriteLine("Save completed");
        }    
    }
}