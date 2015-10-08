using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.UI.ApplicationSettings;
using Orienteer.Data;
using Orienteer.Pages;
using Orienteer.Pages.Navigation;
using Orienteer.WinStore.Messages;
using PresentationBus;

namespace Orienteer.WinStore.Pages.Settings
{
    public class SettingsManager : 
        BindableBase, 
        ISettingsManager,
        IHandlePresentationCommand<DisplaySettingsCommand>
    {
        private readonly INavigator _navigator;
        private readonly Dictionary<Type, Dictionary<object, SettingsViewConfig>> _settings;

        public SettingsManager(INavigator navigator)
        {
            _navigator = navigator;
            _settings = new Dictionary<Type, Dictionary<object, SettingsViewConfig>>();
        }

        public void Add<TController>(object id, string label, Expression<Func<TController, ActionResult>> action)
            where TController : IController
        {
            AddSetting(typeof(GlobalSettings), id, label, action);
        }
        public void Add<TController>(object id, string label, Expression<Func<TController, Task<ActionResult>>> action)
            where TController : IController
        {
            AddSetting(typeof(GlobalSettings), id, label, action);
        }

        public void Add<TView, TController>(object id, string label, Expression<Func<TController, ActionResult>> action)
            where TController : IController
        {
            AddSetting(typeof(TView), id, label, action);
        }

        private void AddSetting<TController>(
            Type scope, 
            object id, 
            string label, 
            Expression<Func<TController, ActionResult>> action) 
            where TController : IController
        {
            var setting = _settings
                .GetSettingsForScope(scope)
                .GetSettingsForId(id);

            setting.Label = label;
            setting.Action = () => _navigator.Navigate(action);
        }
        private void AddSetting<TController>(
            Type scope, 
            object id, 
            string label, 
            Expression<Func<TController, Task<ActionResult>>> action) 
            where TController : IController
        {
            var setting = _settings
                .GetSettingsForScope(scope)
                .GetSettingsForId(id);

            setting.Label = label;
            setting.Action = () => _navigator.NavigateAsync(action);
        }

        public IEnumerable<SettingsViewConfig> GetGlobalSettings()
        {
            return _settings.GetSettingsForScope(typeof(GlobalSettings)).Values;
        }

        public IEnumerable<SettingsViewConfig> GetViewSettings<TView>()
        {
            return GetViewSettings(typeof (TView));
        }

        private IEnumerable<SettingsViewConfig> GetViewSettings(Type type)
        {
            return GetGlobalSettings().Concat(_settings.GetSettingsForScope(type).Values);
        }

        public void Handle(DisplaySettingsCommand command)
        {
            var settings = GetViewSettings(command.Args);

            foreach (var setting in settings)
            {
                command.CommandsRequest.ApplicationCommands.Add(
                    new SettingsCommand(
                        setting.Id,
                        setting.Label,
                        _ =>
                            {
                                setting.Action();
                            }));
            }
        }

        private class GlobalSettings
        {}
    }

    public static class SettingsExtensions
    {
        public static Dictionary<object, SettingsViewConfig> GetSettingsForScope(this Dictionary<Type, Dictionary<object, SettingsViewConfig>> @this, Type scope)
        {
            Dictionary<object, SettingsViewConfig> result;
            if (@this.TryGetValue(scope, out result) == false)
            {
                result = new Dictionary<object, SettingsViewConfig>();
                @this.Add(scope, result);
            }
            return result;
        }

        public static SettingsViewConfig GetSettingsForId(this Dictionary<object, SettingsViewConfig> @this, object id)
        {
            SettingsViewConfig result;
            if (@this.TryGetValue(id, out result) == false)
            {
                result = new SettingsViewConfig
                             {
                                 Id = id
                             };
                @this.Add(id, result);
            }
            return result;
        }
    }

    public class SettingsViewConfig
    {
        public object Id { get; set; }
        public string Label { get; set; }

        public Action Action { get; set; }
    }
}