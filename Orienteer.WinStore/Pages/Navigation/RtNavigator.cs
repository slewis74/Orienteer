using System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Controls;
using Orienteer.Pages;
using Orienteer.Pages.Navigation;
using PresentationBus;

namespace Orienteer.WinStore.Pages.Navigation
{
    public class RtNavigator : Navigator, IRtNavigator
    {
        public RtNavigator(IPresentationBus presentationBus, IControllerInvoker controllerInvoker) : base(presentationBus, controllerInvoker)
        {
        }

        protected override void DoNavigate(ControllerInvokerResult controllerResult, bool animated)
        {
            var result = controllerResult.Result;
            var settingsResult = result as ISettingsPageActionResult;
            if (settingsResult != null)
            {
                DoSettingsPopup(settingsResult);
                return;
            }

            base.DoNavigate(controllerResult, animated);
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