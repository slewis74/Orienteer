using System.Threading;
using Slab.Pages.Navigation;

namespace Slab.ViewModels
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