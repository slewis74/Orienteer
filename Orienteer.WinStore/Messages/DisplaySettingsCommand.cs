using System;
using Windows.UI.ApplicationSettings;
using PresentationBus;

namespace Orienteer.WinStore.Messages
{
    public class DisplaySettingsCommand : IPresentationCommand
    {
        public DisplaySettingsCommand(Type args, SettingsPaneCommandsRequest commandsRequest)
        {
            Args = args;
            CommandsRequest = commandsRequest;
        }

        public Type Args { get; set; }
        public SettingsPaneCommandsRequest CommandsRequest { get; set; }
    }
}