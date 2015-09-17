using Sample.Shared.Model;

namespace Sample.Shared.Requests
{
    public class AddArtistToCurrentPlaylistCommand : AddToCurrentPlaylistCommand
    {
        public Artist Artist { get; set; }
    }
}