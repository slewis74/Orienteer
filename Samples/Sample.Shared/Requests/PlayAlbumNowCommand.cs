using Sample.Shared.Model;

namespace Sample.Shared.Requests
{
    public class PlayAlbumNowCommand : PlayNowCommand<Album>
    {
        public PlayAlbumNowCommand(string artistName, Album album)
            : base(artistName, album.Title)
        {
            Scope = album;
        }
    }
}