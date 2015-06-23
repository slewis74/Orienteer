using System;

namespace Orienteer.Xaml
{
    public interface IViewFactory<out TFrameworkElement>
    {
        TFrameworkElement Resolve(Type viewType);
    }
}