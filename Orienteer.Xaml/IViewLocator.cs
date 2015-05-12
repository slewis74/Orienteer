using System;

namespace Orienteer.Xaml
{
    public interface IViewLocator<out TFrameworkElement>
    {
        Type DetermineViewType(Type viewModelType);
        TFrameworkElement Resolve(object viewModel);
    }
}