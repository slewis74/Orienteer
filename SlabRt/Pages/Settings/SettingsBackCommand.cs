using Slab.ViewModels;
using SlabRt.Pages.Navigation;

namespace SlabRt.Pages.Settings
{
    public class SettingsBackCommand : Command
    {
        private readonly IRtNavigator _navigator;

        public SettingsBackCommand(IRtNavigator navigator)
        {
            _navigator = navigator;
        }

        public override void Execute(object parameter)
        {
            _navigator.SettingsNavigateBack();
        }
    }
}