using System;
using Windows.UI.Xaml;
using Slab.Xaml;

namespace Slab.WinStore.Pages
{
    public class ViewLocator : ViewLocatorBase<FrameworkElement>
    {
        public override FrameworkElement Resolve(object viewModel)
        {
            var view = (FrameworkElement)Activator.CreateInstance(DetermineViewType(viewModel.GetType()));
            view.DataContext = viewModel;
            return view;
        }
    }
}