using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace Slab.WinStore.Data.Navigation
{
    public class NavigationStackStorage : INavigationStackStorage
    {
        private string _deepLinkRoute;

        public NavigationStackStorage(string defaultRoute, bool alwaysStartFromDefaultRoute)
        {
            DefaultRoute = defaultRoute;
            AlwaysStartFromDefaultRoute = alwaysStartFromDefaultRoute;
        }

        public string DefaultRoute { get; private set; }
        public bool AlwaysStartFromDefaultRoute { get; private set; }

        public void StoreRoutes(string[] routes)
        {
            if (AlwaysStartFromDefaultRoute)
                return;

            ApplicationData.Current.LocalSettings.DeleteContainer("Navigation");
            var navContainer = ApplicationData.Current.LocalSettings.CreateContainer("Navigation", ApplicationDataCreateDisposition.Always);

            var count = routes.Count();

            navContainer.Values["RouteCount"] = count;
            
            for (var i = 0; i < count; i++)
            {
                navContainer.Values["Route" + i] = routes[i];
            }
        }

        public void LaunchingDeepLink(string route)
        {
            _deepLinkRoute = route;
        }

        public string[] RetrieveRoutes()
        {
            if (AlwaysStartFromDefaultRoute ||
                ApplicationData.Current.LocalSettings.Containers.ContainsKey("Navigation") == false)
            {
                if (string.IsNullOrWhiteSpace(_deepLinkRoute))
                    return new [] { DefaultRoute };
                else
                    return new[] { DefaultRoute, _deepLinkRoute };
            }

            if (string.IsNullOrWhiteSpace(_deepLinkRoute) == false)
            {
                return new[] { DefaultRoute, _deepLinkRoute };
            }

            var navContainer = ApplicationData.Current.LocalSettings.CreateContainer("Navigation", ApplicationDataCreateDisposition.Always);

            var count = Convert.ToInt32(navContainer.Values["RouteCount"]);
            var results = new List<string>();

            // read the entries back in reverse index order, so the stack comes out the right
            // way around.
            for (var i = 0; i < count; i++)
            {
                results.Add(navContainer.Values["Route" + (count - 1 - i)].ToString());
            }
            return results.ToArray();
        }
    }
}