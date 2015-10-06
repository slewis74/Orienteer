using PresentationBus;

namespace Orienteer.Messages
{
    public class PageNavigationCommand : IPresentationCommand
    {
        public PageNavigationCommand(string route, PageNavigationRequestEventArgs args)
        {
            Args = args;
            Route = route;
        }

        public PageNavigationRequestEventArgs Args { get; set; }
        public string Route { get; set; }
    }
}