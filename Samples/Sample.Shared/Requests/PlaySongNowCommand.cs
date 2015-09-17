using Sample.Shared.Model;

namespace Sample.Shared.Requests
{
    public class PlaySongNowCommand : PlayNowCommand<Song>
    {
        public PlaySongNowCommand(string artistName, string albumTitle, Song song)
            : base(artistName, albumTitle)
        {
            Scope = song;
        }
    }
}