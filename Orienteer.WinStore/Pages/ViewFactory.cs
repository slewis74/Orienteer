using System;
using Windows.UI.Xaml;
using Orienteer.Xaml;

namespace Orienteer.WinStore.Pages
{
    public class ViewFactory : IViewFactory<FrameworkElement>
    {
        public FrameworkElement Resolve(Type viewType)
        {
            return (FrameworkElement) Activator.CreateInstance(viewType);
        }
    }
}