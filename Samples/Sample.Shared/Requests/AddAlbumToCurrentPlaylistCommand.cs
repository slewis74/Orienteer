using Sample.Shared.Model;

namespace Sample.Shared.Requests
{
    public class AddAlbumToCurrentPlaylistCommand : AddToCurrentPlaylistCommand
    {
        public Album Album { get; set; }
    }
}