using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Slab.Pages;
using Slab.Pages.Navigation;
using Slab.PresentationBus;

namespace Slab.WinStore.Pages.Settings
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