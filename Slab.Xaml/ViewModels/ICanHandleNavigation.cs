using System;

namespace Slab.Xaml.ViewModels
{
    public interface ICanHandleNavigation
    {
        void Navigate(Type viewType);
        void Navigate(Type viewType, object parameter);
    }
}