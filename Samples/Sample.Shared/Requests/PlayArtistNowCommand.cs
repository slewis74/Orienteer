using Sample.Shared.Model;

namespace Sample.Shared.Requests
{
    public class PlayArtistNowCommand : PlayNowCommand<Artist>
    {
        public PlayArtistNowCommand(Artist artist)
            : base(artist.Name, null)
        {
            Scope = artist;
        }
    }
}