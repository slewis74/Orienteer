using System;
using Windows.UI.ApplicationSettings;
using Slab.PresentationBus;

namespace Slab.WinStore.Requests
{
    public class DisplaySettingsRequest : PresentationRequest<Type>
    {
        public DisplaySettingsRequest(Type args, SettingsPaneCommandsRequest commandsRequest) : base(args)
        {
            CommandsRequest = commandsRequest;
            MustBeHandled = true;
        }

        public SettingsPaneCommandsRequest CommandsRequest { get; set; }
    }
}