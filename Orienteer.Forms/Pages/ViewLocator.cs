using System;
using Orienteer.Xaml;
using Xamarin.Forms;

namespace Orienteer.Forms.Pages
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