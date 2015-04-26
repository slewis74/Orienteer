using Windows.UI.Xaml;
using Slab.Data;
using Slab.Pages;
using Slab.Pages.Navigation;
using Slab.WinStore.Commands;
using Slab.WinStore.Pages.Navigation;
using Slab.Xaml;
using Slew.PresentationBus;

namespace Slab.WinStore.Host
{
    public class HostViewModel : BindableBase
    {
        public IPresentationBus PresentationBus { get; set; }
        public IRtNavigator Navigator { get; set; }
        public IViewLocator<FrameworkElement> ViewLocator { get; set; }
        public INavigationStack NavigationStack { get; set; }
        public IControllerInvoker ControllerInvoker { get; set; }
        public GoBackCommand Back { get; set; }
    }
}