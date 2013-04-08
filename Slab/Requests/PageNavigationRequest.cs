using Slab.PresentationBus;

namespace Slab.Requests
{
    public class PageNavigationRequest : PresentationRequest<PageNavigationRequestEventArgs>
    {
        public PageNavigationRequest(string route, PageNavigationRequestEventArgs args) : base(args)
        {
            Route = route;
        }

        public string Route { get; set; }
    }
}