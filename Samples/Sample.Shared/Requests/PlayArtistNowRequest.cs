using Sample.Shared.Model;

namespace Sample.Shared.Requests
{
    public class PlayArtistNowRequest : PlayNowRequest<Artist>
    {
        public PlayArtistNowRequest(Artist artist)
            : base(artist.Name, null)
        {
            Scope = artist;
        }
    }
}