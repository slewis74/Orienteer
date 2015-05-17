using Sample.Shared.Model;

namespace Sample.Shared.Requests
{
    public class PlaySongNowRequest : PlayNowRequest<Song>
    {
        public PlaySongNowRequest(string artistName, string albumTitle, Song song)
            : base(artistName, albumTitle)
        {
            Scope = song;
        }
    }
}