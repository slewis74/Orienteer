using Slew.PresentationBus;

namespace Orienteer.Requests
{
    public class ViewModelNavigationRequest : PresentationRequest
    {
        public ViewModelNavigationRequest(string route, ViewModelNavigationRequestEventArgs args, bool animated)
        {
            Route = route;
            Args = args;
            Animated = animated;
        }

        public ViewModelNavigationRequestEventArgs Args { get; set; }
        public string Route { get; set; }
        public bool Animated { get; set; }
    }
}