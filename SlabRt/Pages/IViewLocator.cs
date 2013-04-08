using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace SlabRt.Pages
{
    public interface IViewLocator
    {
        FrameworkElement Resolve(object viewModel, ApplicationViewState applicationViewState);
    }
}