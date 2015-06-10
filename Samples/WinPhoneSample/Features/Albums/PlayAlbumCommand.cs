using Orienteer.Xaml.ViewModels;
using Sample.Shared.Requests;
using Slew.PresentationBus;

namespace WinPhoneSample.Features.Albums
{
    public class PlayAlbumCommand : Command<AlbumViewModel>
    {
        private readonly IPresentationBus _presentationBus;

        public PlayAlbumCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override void Execute(AlbumViewModel parameter)
        {
            _presentationBus.PublishAsync(new PlayAlbumNowRequest(parameter.ArtistName, parameter.GetAlbum()));
        }

    }
}