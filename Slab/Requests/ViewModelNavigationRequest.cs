using Slab.PresentationBus;

namespace Slab.Requests
{
    public class ViewModelNavigationRequest : PresentationRequest<ViewModelNavigationRequestEventArgs>
    {
        public ViewModelNavigationRequest(string route, ViewModelNavigationRequestEventArgs args) : base(args)
        {
            Route = route;
        }

        public string Route { get; set; }
    }
}