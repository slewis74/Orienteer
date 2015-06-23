using Orienteer.Xaml;
using Xamarin.Forms;

namespace Orienteer.Forms.Pages
{
    public class ViewLocator : ViewLocatorBase<Page>, IViewLocator
    {
        private readonly IViewFactory<Page> _viewFactory;

        public ViewLocator(IViewFactory<Page> viewFactory)
        {
            _viewFactory = viewFactory;
        }

        public override Page Resolve(object viewModel)
        {
            var viewType = DetermineViewType(viewModel.GetType());
            var view = _viewFactory.Resolve(viewType);
            view.BindingContext = viewModel;
            return view;
        }
    }

    public interface IViewLocator : IViewLocator<Page>
    { }
}