using System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Controls;
using Slab.Pages;
using Slab.Pages.Navigation;
using Slew.PresentationBus;

namespace Slab.WinStore.Pages.Navigation
{
    public class RtNavigator : Navigator, IRtNavigator
    {
        public RtNavigator(IPresentationBus presentationBus, IControllerInvoker controllerInvoker) : base(presentationBus, controllerInvoker)
        {
        }

        protected override void DoNavigate(ControllerInvokerResult controllerResult)
        {
            var result = controllerResult.Result;
            var settingsResult = result as ISettingsPageActionResult;
            if (settingsResult != null)
            {
                DoSettingsPopup(settingsResult);
                return;
            }

            base.DoNavigate(controllerResult);
        }

        public void SettingsNavigateBack()
        {
            // the back button shows the Settings pane again.
            SettingsPane.Show();
        }

        private void DoSettingsPopup(ISettingsPageActionResult settingsResult)
        {
            var view = (SettingsFlyout)Activator.CreateInstance(settingsResult.PageType);
            view.DataContext = settingsResult.Parameter;

            view.Show();
        }
    }
}