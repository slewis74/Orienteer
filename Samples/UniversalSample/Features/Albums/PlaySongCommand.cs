using Orienteer.Xaml.ViewModels;
using PresentationBus;
using Sample.Shared.Requests;

namespace UniversalSample.Features.Albums
{
    public class PlaySongCommand : Command<TrackViewModel>
    {
        private readonly IPresentationBus _presentationBus;

        public PlaySongCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override void Execute(TrackViewModel parameter)
        {
            _presentationBus.Send(new PlaySongNowCommand(parameter.ArtistName, parameter.AlbumTitle, parameter.GetSong()));
        }
    }
}