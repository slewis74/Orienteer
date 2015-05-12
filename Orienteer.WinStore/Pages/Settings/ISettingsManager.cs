using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Orienteer.Pages;
using Orienteer.Pages.Navigation;
using Slew.PresentationBus;

namespace Orienteer.WinStore.Pages.Settings
{
    public interface ISettingsManager : IHandlePresentationEvents
    {
        void Add<TController>(object id, string label, Expression<Func<TController, ActionResult>> action)
            where TController : IController;
        void Add<TController>(object id, string label, Expression<Func<TController, Task<ActionResult>>> action)
            where TController : IController;

        void Add<TView, TController>(object id, string label, Expression<Func<TController, ActionResult>> action)
            where TController : IController;

        IEnumerable<SettingsViewConfig> GetGlobalSettings();
        IEnumerable<SettingsViewConfig> GetViewSettings<TView>();
    }
}