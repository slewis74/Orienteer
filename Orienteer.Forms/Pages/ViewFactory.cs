using System;
using Orienteer.Xaml;
using Xamarin.Forms;

namespace Orienteer.Forms.Pages
{
    public class ViewFactory : IViewFactory<Page>
    {
        public Page Resolve(Type viewType)
        {
            return (Page) Activator.CreateInstance(viewType);
        }
    }
}