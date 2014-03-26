using System;
using Slab.PresentationBus;
using Windows.UI.ApplicationSettings;

namespace SlabRt.Requests
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