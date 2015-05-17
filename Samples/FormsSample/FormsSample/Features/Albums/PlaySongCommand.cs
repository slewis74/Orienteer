using Orienteer.Xaml.ViewModels;
using Sample.Shared.Requests;
using Slew.PresentationBus;

namespace FormsSample.Features.Albums
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
            _presentationBus.PublishAsync(new PlaySongNowRequest(parameter.ArtistName, parameter.AlbumTitle, parameter.GetSong()));
        }
    }
}