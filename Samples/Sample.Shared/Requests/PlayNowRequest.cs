using Slew.PresentationBus;

namespace Sample.Shared.Requests
{
    public class PlayNowRequest<T> : PresentationRequest
    {
        public PlayNowRequest(string artistName, string albumTitle)
        {
            ArtistName = artistName;
            AlbumTitle = albumTitle;
        }

        public string ArtistName { get; set; }
        public string AlbumTitle { get; set; }

        public T Scope { get; protected set; }
    }
}