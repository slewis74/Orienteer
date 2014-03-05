using System.Reflection;
using Windows.UI.Xaml;

namespace SlabRt.Pages
{
    public interface IViewLocator
    {
        FrameworkElement Resolve(object viewModel, PageLayout pageLayout);
    }
}