using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MediaPlayer;
using Newtonsoft.Json;
using Orienteer.Data;
using Sample.Shared;
using Sample.Shared.Model;

namespace FormsSample.iOS.PlatformServices
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
            var mq = new MPMediaQuery();
            var value = NSNumber.FromInt32((int)MPMediaType.Music);
            var type = MPMediaItem.MediaTypeProperty;
            var predicate = MPMediaPropertyPredicate.PredicateWithValue(value, type);
            mq.AddFilterPredicate(predicate);

            foreach (var item in mq.Items)
            {
                var artist = artists.FirstOrDefault(a => a.Name == item.Artist);
                if (artist == null)
                {
                    artist = new Artist
                    {
                        Name = item.Artist
                    };
                    foundNewItems = true;
                }

                var album = artist.Albums.FirstOrDefault(a => a.Title == item.AlbumTitle);
                if (album == null)
                {
                    album = new Album
                    {
                        Title = item.AlbumTitle
                    };
                    artist.Albums.Add(album);
                    foundNewItems = true;
                }

                var song = album.Songs.FirstOrDefault(s => s.Title == item.Title);
                if (song == null)
                {
                    song = new Song
                    {
                        Title = item.Title,
                        DiscNumber = (uint)item.DiscNumber,
                        Duration = TimeSpan.FromSeconds(item.PlaybackDuration)
                    };
                    album.Songs.Add(song);
                    foundNewItems = true;
                }
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