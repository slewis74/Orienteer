using System;
using System.Windows;
using Orienteer.Xaml;

namespace Orienteer.WinPhone.Pages
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