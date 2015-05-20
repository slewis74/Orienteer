﻿using System;
using Windows.UI.Xaml;
using Orienteer.Xaml;

namespace Orienteer.WinStore.Pages
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