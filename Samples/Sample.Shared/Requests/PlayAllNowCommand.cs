using System.Collections.Generic;
using Sample.Shared.Model;
using PresentationBus;

namespace Sample.Shared.Requests
{
    public class PlayAllNowCommand : IPresentationCommand
    {
        public PlayAllNowCommand(IEnumerable<Artist> artists)
        {
            Artists = artists;
        }

        public IEnumerable<Artist> Artists { get; private set; }
    }
}