using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Orienteer.Pages.Navigation;

namespace Orienteer.Universal.Data.Navigation
{
    public class UniversalNavigationStackStorage : NavigationStackStorage
    {
        protected override void WriteRoutes(string[] routes)
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

        protected override string[] ReadRoutes()
        {
            if (ApplicationData.Current.LocalSettings.Containers.ContainsKey("Navigation") == false)
            {
                return null;
            }

            var navContainer = ApplicationData.Current.LocalSettings.CreateContainer("Navigation", ApplicationDataCreateDisposition.Always);

            var count = Convert.ToInt32(navContainer.Values["RouteCount"]);
            var results = new List<string>();

            for (var i = count - 1; i >= 0; i--)
            {
                results.Add(navContainer.Values["Route" + i].ToString());
            }
            return results.ToArray();
        }
    }
}