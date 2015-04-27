using System;
using Slab.Xaml;
using Xamarin.Forms;

namespace Slab.Forms.Pages
{
    public class ViewLocator : ViewLocatorBase<Page>, IViewLocator
    {
        public override Page Resolve(object viewModel)
        {
            var viewType = DetermineViewType(viewModel.GetType());
            var view = (Page)Activator.CreateInstance(viewType);
            view.BindingContext = viewModel;
            return view;
        }
    }

    public interface IViewLocator : IViewLocator<Page>
    { }
}