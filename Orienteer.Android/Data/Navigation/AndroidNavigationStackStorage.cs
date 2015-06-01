﻿using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Orienteer.Pages.Navigation;

namespace Orienteer.Android.Data.Navigation
{
    public class AndroidNavigationStackStorage : NavigationStackStorage
    {
        protected override void WriteRoutes(string[] routes)
        {
            using (var prefs = Application.Context.GetSharedPreferences("Navigation", FileCreationMode.Private))
            {
                using (var editor = prefs.Edit())
                {
                    editor.PutStringSet("Stack", routes);
                    editor.Apply();
                }
            }
        }

        protected override string[] ReadRoutes()
        {
            ICollection<string> routes = null;
            using (var prefs = Application.Context.GetSharedPreferences("Navigation", FileCreationMode.Private))
            {
                routes = prefs.GetStringSet("Stack", null);
            }
            return routes == null ? null : routes.ToArray();
        }
    }
}