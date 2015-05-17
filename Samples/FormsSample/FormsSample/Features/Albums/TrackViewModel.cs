using System;
using Sample.Shared.Model;

namespace FormsSample.Features.Albums
{
    public class TrackViewModel
    {
        private readonly Song _song;

        public TrackViewModel(string artistName, string albumTitle, Song song)
        {
            _song = song;
            ArtistName = artistName;
            AlbumTitle = albumTitle;
        }

        public string ArtistName { get; private set; }
        public string AlbumTitle { get; private set; }

        public uint TrackNumber { get { return _song.TrackNumber; } }
        public string Title { get { return _song.Title; } }
        public uint DiscNumber { get { return _song.DiscNumber; } }
        public TimeSpan Duration { get { return _song.Duration; } }

        public Song GetSong()
        {
            return _song;
        }
    }
}