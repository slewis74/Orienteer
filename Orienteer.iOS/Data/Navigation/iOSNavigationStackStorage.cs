using Foundation;
using Orienteer.Pages.Navigation;

namespace Orienteer.iOS.Data.Navigation
{
    public class iOSNavigationStackStorage : NavigationStackStorage
    {
        protected override void WriteRoutes(string[] routes)
        {
            NSUserDefaults settings = NSUserDefaults.StandardUserDefaults;
            settings.RemoveObject("Navigation");
            settings["Navigation"] = NSArray.FromStrings(routes);
        }

        protected override string[] ReadRoutes()
        {
            NSUserDefaults settings = NSUserDefaults.StandardUserDefaults;
            return settings.StringArrayForKey("Navigation");
        }
    }
}