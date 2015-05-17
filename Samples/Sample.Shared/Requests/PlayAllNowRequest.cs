using System.Collections.Generic;
using Sample.Shared.Model;
using Slew.PresentationBus;

namespace Sample.Shared.Requests
{
    public class PlayAllNowRequest : PresentationRequest
    {
        public PlayAllNowRequest(IEnumerable<Artist> artists)
        {
            Artists = artists;
        }

        public IEnumerable<Artist> Artists { get; private set; }
    }
}