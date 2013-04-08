using Slab.Pages.Navigation;
using SlabRt.Pages.Settings;

namespace SlabRt.Pages.Navigation
{
    public class SettingsPageActionResult<TView, TViewModel> : PageActionResult<TView>, ISettingsPageActionResult where TView : SettingsView
    {
        public SettingsPageActionResult(TViewModel viewModel) : base(viewModel)
        {}
    }
}