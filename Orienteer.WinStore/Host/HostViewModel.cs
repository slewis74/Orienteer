using Windows.UI.Xaml;
using Orienteer.Data;
using Orienteer.Pages;
using Orienteer.Pages.Navigation;
using Orienteer.WinStore.Commands;
using Orienteer.WinStore.Pages.Navigation;
using Orienteer.Xaml;
using PresentationBus;

namespace Orienteer.WinStore.Host
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