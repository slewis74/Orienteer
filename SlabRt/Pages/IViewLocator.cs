using System.Reflection;
using Windows.UI.Xaml;

namespace SlabRt.Pages
{
    public interface IViewLocator
    {
        void Configure(Assembly assembly, string baseViewModelNamespace, string baseViewNamespace);

        FrameworkElement Resolve(object viewModel, PageLayout pageLayout);
    }
}