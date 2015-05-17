using Sample.Shared.Model;

namespace Sample.Shared.Requests
{
    public class AddArtistToCurrentPlaylistRequest : AddToCurrentPlaylistRequest
    {
        public Artist Artist { get; set; }
    }
}