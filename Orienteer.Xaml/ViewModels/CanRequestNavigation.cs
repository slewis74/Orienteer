using Orienteer.Pages.Navigation;

namespace Orienteer.Xaml.ViewModels
{
    public abstract class CanRequestNavigation : HasPageTitle
    {
        protected CanRequestNavigation(INavigator navigator)
        {
            Navigator = navigator;
        }

        public INavigator Navigator { get; private set; }
    }
}