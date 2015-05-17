using System.Collections.Generic;
using Sample.Shared.Model;
using Slew.PresentationBus;

namespace Sample.Shared.Events
{
    public class AlbumDataLoaded : PresentationEvent
    {
        public AlbumDataLoaded(IEnumerable<Artist> artists)
        {
            Artists = artists;
        }

        public IEnumerable<Artist> Artists { get; set; }
    }
}