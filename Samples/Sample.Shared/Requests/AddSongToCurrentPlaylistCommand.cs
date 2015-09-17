using Sample.Shared.Model;

namespace Sample.Shared.Requests
{
    public class AddSongToCurrentPlaylistCommand : AddToCurrentPlaylistCommand
    {
        public Song Song { get; set; }
    }
}