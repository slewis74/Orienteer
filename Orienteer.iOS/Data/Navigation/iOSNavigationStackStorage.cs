using Foundation;
using Orienteer.Pages.Navigation;

namespace Orienteer.iOS.Data.Navigation
{
    public class iOSNavigationStackStorage : NavigationStackStorage
    {
        protected override void WriteRoutes(string[] routes)
        {
            var settings = NSUserDefaults.StandardUserDefaults;
            settings.RemoveObject("Navigation");
            settings["Navigation"] = NSArray.FromStrings(routes);
        }

        protected override string[] ReadRoutes()
        {
            var settings = NSUserDefaults.StandardUserDefaults;
            return settings.StringArrayForKey("Navigation");
        }
    }
}