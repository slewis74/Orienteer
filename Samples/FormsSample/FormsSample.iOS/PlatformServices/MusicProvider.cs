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
using Orienteer;
using Orienteer.Data;
using Sample.Shared;
using Sample.Shared.Model;

namespace FormsSample.iOS.PlatformServices
{
    public class MusicProvider : IMusicProvider
    {
        private bool _hasLoaded;

        public MusicProvider()
        {
            Artists = new DistinctAsyncObservableCollection<Artist>();
        }

        private DistinctAsyncObservableCollection<Artist> Artists { get; set; }

        private readonly AsyncLock _asyncLock = new AsyncLock();
        public async Task<DistinctAsyncObservableCollection<Artist>> GetArtists()
        {
            using (var releaser = await _asyncLock.LockAsync())
            {
                if (_hasLoaded == false)
                {
                    await LoadContent();
                    _hasLoaded = true;
                }
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
        }

        /// <summary>
        /// Loads the existing content from the application data
        /// </summary>
        private async Task<bool> LoadData(List<Artist> artists)
        {
            var storageFile = IsolatedStorageFile.GetUserStoreForApplication();
            if (storageFile.FileExists("Artists") == false)
            {
                return false;
            }

            try
            {
                IEnumerable<Artist> artistsData;
                using (var artistsFile = new IsolatedStorageFileStream("Artists", FileMode.Open))
                {
                    using (var textStream = new StreamReader(artistsFile))
                    {
                        // WARNING: Do not use the async method here, if you do the method returns and the nav tries to finish
                        // initialising before the data is available.
                        var data = textStream.ReadToEnd();
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
            if (artists.Any())
                return false;

            var artist1 = new Artist
            {
                Name = "Black Eyed Peas"
            };
            var album1 = new Album{ Title = "Monkey Business"};
            album1.Songs.Add(new Song { TrackNumber = 1, Title = "Boom, Boom, Pow"});
            artist1.Albums.Add(album1);
            artists.Add(artist1);

            var artist2 = new Artist
            {
                Name = "Hilltop Hoods"
            };
            var album2 = new Album { Title = "Drinking from the sun" };
            album2.Songs.Add(new Song { TrackNumber = 3, Title = "I love it" });
            artist2.Albums.Add(album2);

            var album3 = new Album { Title = "State of the art" };
            album3.Songs.Add(new Song { TrackNumber = 3, Title = "Chase that feeling" });
            artist2.Albums.Add(album3);

            artists.Add(artist2);

            return true;

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
                    artists.Add(artist);
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