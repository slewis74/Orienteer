using Sample.Shared.Model;

namespace Sample.Shared.Requests
{
    public class AddSongToCurrentPlaylistRequest : AddToCurrentPlaylistRequest
    {
        public Song Song { get; set; }
    }
}