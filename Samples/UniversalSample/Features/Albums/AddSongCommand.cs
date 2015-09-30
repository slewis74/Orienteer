using Orienteer.Xaml.ViewModels;
using PresentationBus;
using Sample.Shared.Requests;

namespace UniversalSample.Features.Albums
{
    public class AddSongCommand : Command<TrackViewModel>
    {
        private readonly IPresentationBus _presentationBus;

        public AddSongCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override void Execute(TrackViewModel parameter)
        {
            _presentationBus.Send(new AddSongToCurrentPlaylistCommand { Song = parameter.GetSong() });
        }
    }
}