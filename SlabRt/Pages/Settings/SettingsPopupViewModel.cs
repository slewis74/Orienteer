using Slab.Data;

namespace SlabRt.Pages.Settings
{
    public class SettingsPopupViewModel : BindableBase
    {
        public SettingsPopupViewModel(SettingsBackCommand settingsBackCommand)
        {
            Back = settingsBackCommand;
        }

        public SettingsBackCommand Back { get; private set; }
    }
}