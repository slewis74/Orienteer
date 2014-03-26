using Windows.UI.Xaml;
using Slab.Data;
using Slab.Pages;
using Slab.PresentationBus;
using Slab.Xaml;
using SlabRt.Commands;
using SlabRt.Data.Navigation;
using SlabRt.Pages.Navigation;

namespace SlabRt.Host
{
    public class HostViewModel : BindableBase
    {
        public IPresentationBus PresentationBus { get; set; }
        public IRtNavigator Navigator { get; set; }
        public IViewLocator<FrameworkElement> ViewLocator { get; set; }
        public INavigationStackStorage NavigationStackStorage { get; set; }
        public IControllerInvoker ControllerInvoker { get; set; }
        public GoBackCommand Back { get; set; }
    }
}