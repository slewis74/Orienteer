using Windows.UI.Xaml.Controls;
using Slab.Pages.Navigation;

namespace SlabRt.Pages.Navigation
{
    public class SettingsPageActionResult<TView, TViewModel> : PageActionResult<TView>, ISettingsPageActionResult where TView : SettingsFlyout
    {
        public SettingsPageActionResult(TViewModel viewModel) : base(viewModel)
        {}
    }
}