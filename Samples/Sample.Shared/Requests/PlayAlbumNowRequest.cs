using Sample.Shared.Model;

namespace Sample.Shared.Requests
{
    public class PlayAlbumNowRequest : PlayNowRequest<Album>
    {
        public PlayAlbumNowRequest(string artistName, Album album)
            : base(artistName, album.Title)
        {
            Scope = album;
        }
    }
}