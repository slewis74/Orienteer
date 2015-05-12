using System.Collections.Generic;

namespace Orienteer.Pages.Navigation
{
    public abstract class NavigationStackStorage : INavigationStackStorage
    {
        public void StoreRoutes(string[] routes)
        {
            WriteRoutes(routes);
        }

        protected abstract void WriteRoutes(string[] routes);

        public string[] RetrieveRoutes(string defaultRoute)
        {
            var routes = ReadRoutes();

            if (routes == null || routes.Length == 0)
                return new [] { defaultRoute };

            var results = new List<string>();
            var count = routes.Length;

            // read the entries back in reverse index order, so the stack comes out the right
            // way around.
            for (var i = 0; i < count; i++)
            {
                results.Add(routes[(count - 1 - i)]);
            }

            return results.ToArray();
        }

        protected abstract string[] ReadRoutes();
    }
}