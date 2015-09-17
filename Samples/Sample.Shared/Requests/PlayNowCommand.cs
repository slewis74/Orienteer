using PresentationBus;

namespace Sample.Shared.Requests
{
    public class PlayNowCommand<T> : IPresentationCommand
    {
        public PlayNowCommand(string artistName, string albumTitle)
        {
            ArtistName = artistName;
            AlbumTitle = albumTitle;
        }

        public string ArtistName { get; set; }
        public string AlbumTitle { get; set; }

        public T Scope { get; protected set; }
    }
}