using Slew.PresentationBus;

namespace Orienteer.Requests
{
    public class ViewModelNavigationRequest : PresentationRequest
    {
        public ViewModelNavigationRequest(string route, ViewModelNavigationRequestEventArgs args)
        {
            Route = route;
            Args = args;
        }

        public ViewModelNavigationRequestEventArgs Args { get; set; }
        public string Route { get; set; }
    }
}