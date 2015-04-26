using System.IO.IsolatedStorage;
using Slab.Pages.Navigation;

namespace Slab.WinPhone.Data.Navigation
{
    public class WinPhoneNavigationStackStorage : NavigationStackStorage
    {
        protected override void WriteRoutes(string[] routes)
        {
            IsolatedStorageSettings.ApplicationSettings.Remove("Navigation");

            IsolatedStorageSettings.ApplicationSettings.Add("Navigation", routes);
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        protected override string[] ReadRoutes()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("Navigation") == false)
                return null;
            var navStackItems = (string[])IsolatedStorageSettings.ApplicationSettings["Navigation"];
            return navStackItems;
        }
    }
}