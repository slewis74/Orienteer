using Windows.UI.Xaml;
using Orienteer.Xaml;

namespace Orienteer.Universal.Pages
{
    public class ViewLocator : ViewLocatorBase<FrameworkElement>, IViewLocator
    {
        private readonly IViewFactory<FrameworkElement> _viewFactory;

        public ViewLocator(IViewFactory<FrameworkElement> viewFactory)
        {
            _viewFactory = viewFactory;
        }

        public override FrameworkElement Resolve(object viewModel)
        {
            var view = _viewFactory.Resolve(DetermineViewType(viewModel.GetType()));
            view.DataContext = viewModel;
            return view;
        }
    }

    public interface IViewLocator : IViewLocator<FrameworkElement>
    { }
}