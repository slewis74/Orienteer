using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace SlabRt.Data.Navigation
{
    public class NavigationStackStorage : INavigationStackStorage
    {
        public void StoreRoutes(string[] routes)
        {
            ApplicationData.Current.LocalSettings.DeleteContainer("Navigation");
            var navContainer = ApplicationData.Current.LocalSettings.CreateContainer("Navigation", ApplicationDataCreateDisposition.Always);

            var count = routes.Count();

            navContainer.Values["RouteCount"] = count;
            
            for (var i = 0; i < count; i++)
            {
                navContainer.Values["Route" + i] = routes[i];
            }
        }

        public string[] RetrieveRoutes()
        {
            if (ApplicationData.Current.LocalSettings.Containers.ContainsKey("Navigation") == false)
                return null;

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