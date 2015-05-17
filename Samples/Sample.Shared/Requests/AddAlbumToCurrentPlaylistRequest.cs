using Sample.Shared.Model;

namespace Sample.Shared.Requests
{
    public class AddAlbumToCurrentPlaylistRequest : AddToCurrentPlaylistRequest
    {
        public Album Album { get; set; }
    }
}