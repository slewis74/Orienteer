using System;
using Windows.UI.ApplicationSettings;
using Slew.PresentationBus;

namespace Slab.WinStore.Requests
{
    public class DisplaySettingsRequest : PresentationRequest
    {
        public DisplaySettingsRequest(Type args, SettingsPaneCommandsRequest commandsRequest)
        {
            Args = args;
            CommandsRequest = commandsRequest;
            MustBeHandled = true;
        }

        public Type Args { get; set; }
        public SettingsPaneCommandsRequest CommandsRequest { get; set; }
    }
}