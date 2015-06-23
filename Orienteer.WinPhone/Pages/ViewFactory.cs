using System;
using System.Windows;
using Orienteer.Xaml;

namespace Orienteer.WinPhone.Pages
{
    public class ViewFactory : IViewFactory<FrameworkElement>
    {
        public FrameworkElement Resolve(Type viewType)
        {
            return (FrameworkElement)Activator.CreateInstance(viewType);
        }
    }
}