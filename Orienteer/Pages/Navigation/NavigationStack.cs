namespace Orienteer.Pages.Navigation
{
    public class NavigationStack : INavigationStack
    {
        private string _deepLinkRoute;

        public NavigationStack(string defaultRoute, bool alwaysStartFromDefaultRoute)
        {
            DefaultRoute = defaultRoute;
            AlwaysStartFromDefaultRoute = alwaysStartFromDefaultRoute;
        }

        public string DefaultRoute { get; private set; }
        public bool AlwaysStartFromDefaultRoute { get; private set; }

        public INavigationStackStorage NavigationStackStorage { get; set; }

        public void StoreRoutes(string[] routes)
        {
            if (AlwaysStartFromDefaultRoute || NavigationStackStorage == null)
                return;

            NavigationStackStorage.StoreRoutes(routes);
        }

        public void LaunchingDeepLink(string route)
        {
            _deepLinkRoute = route;
        }

        public string[] RetrieveRoutes()
        {
            if (AlwaysStartFromDefaultRoute || NavigationStackStorage == null)
            {
                if (string.IsNullOrWhiteSpace(_deepLinkRoute))
                    return new[] { DefaultRoute };
                else
                    return new[] { DefaultRoute, _deepLinkRoute };
            }

            // if there we're launching a deep link then pust the default route onto the stack
            // first, so there is somewhere to go "back" to.
            if (string.IsNullOrWhiteSpace(_deepLinkRoute) == false)
            {
                return new[] { DefaultRoute, _deepLinkRoute };
            }

            return NavigationStackStorage.RetrieveRoutes(DefaultRoute);
        }
    }
}