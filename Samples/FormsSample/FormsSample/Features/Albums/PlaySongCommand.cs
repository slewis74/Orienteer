using Orienteer.Xaml.ViewModels;
using Sample.Shared.Requests;
using PresentationBus;

namespace FormsSample.Features.Albums
{
    public class PlaySongCommand : Command<TrackViewModel>
    {
        private readonly IPresentationBus _presentationBus;

        public PlaySongCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override async void Execute(TrackViewModel parameter)
        {
            await _presentationBus.SendAsync(new PlaySongNowCommand(parameter.ArtistName, parameter.AlbumTitle, parameter.GetSong()));
        }
    }
}