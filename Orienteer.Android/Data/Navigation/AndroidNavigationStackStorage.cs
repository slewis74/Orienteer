using System;
using Android.App;
using Android.Content;
using Orienteer.Pages.Navigation;

namespace Orienteer.Android.Data.Navigation
{
    /// <summary>
    /// NOTE: tried using PutStringSet/GetStringSet but the get doesn't return the strings in the same order as the
    /// put.  Storing in a single string for now, using ;;; as a separator.
    /// </summary>
    public class AndroidNavigationStackStorage : NavigationStackStorage
    {
        protected override void WriteRoutes(string[] routes)
        {
            using (var prefs = Application.Context.GetSharedPreferences("Navigation", FileCreationMode.Private))
            {
                using (var editor = prefs.Edit())
                {
                    editor.PutString("Stack", string.Join(";;;", routes));
                    editor.Apply();
                }
            }
        }

        protected override string[] ReadRoutes()
        {
            string routes = null;
            using (var prefs = Application.Context.GetSharedPreferences("Navigation", FileCreationMode.Private))
            {
                routes = prefs.GetString("Stack", null);
            }
            return routes == null ? null : routes.Split(new []{ ";;;" }, StringSplitOptions.None);
        }
    }
}