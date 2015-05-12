using System.Threading;
using Orienteer.Pages.Navigation;

namespace Orienteer.Xaml.ViewModels
{
    public abstract class CanRequestNavigationBase : HasPageTitleBase
    {
        protected CanRequestNavigationBase(INavigator navigator)
        {
            Navigator = navigator;
        }

        protected CanRequestNavigationBase(INavigator navigator, SynchronizationContext synchronizationContext) : base(synchronizationContext)
        {
            Navigator = navigator;
        }

        public INavigator Navigator { get; private set; }
    }
}