using Slab.Data;
using Slab.Pages;
using Slab.PresentationBus;
using SlabRt.Data.Navigation;
using SlabRt.Pages;
using SlabRt.Pages.Navigation;

namespace SlabRt.Host
{
    public class HostViewModel : BindableBase
    {
        public IPresentationBus PresentationBus { get; set; }
        public IRtNavigator Navigator { get; set; }
        public IViewLocator ViewLocator { get; set; }
        public INavigationStackStorage NavigationStackStorage { get; set; }
        public IControllerInvoker ControllerInvoker { get; set; }
    }
}