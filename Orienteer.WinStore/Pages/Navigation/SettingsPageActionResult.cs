using Windows.UI.Xaml.Controls;
using Orienteer.Pages.Navigation;

namespace Orienteer.WinStore.Pages.Navigation
{
    public class SettingsPageActionResult<TView, TViewModel> : PageActionResult<TView>, ISettingsPageActionResult where TView : SettingsFlyout
    {
        public SettingsPageActionResult(TViewModel viewModel) : base(viewModel)
        {}
    }
}