using Slew.PresentationBus;

namespace Slab.Requests
{
    public class PageNavigationRequest : PresentationRequest
    {
        public PageNavigationRequest(string route, PageNavigationRequestEventArgs args)
        {
            Args = args;
            Route = route;
        }

        public PageNavigationRequestEventArgs Args { get; set; }
        public string Route { get; set; }
    }
}