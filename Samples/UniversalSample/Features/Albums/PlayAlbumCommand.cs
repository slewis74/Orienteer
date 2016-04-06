using Orienteer.Xaml.ViewModels;
using PresentationBus;
using Sample.Shared.Requests;

namespace UniversalSample.Features.Albums
{
    public class PlayAlbumCommand : Command<AlbumViewModel>
    {
        private readonly IPresentationBus _presentationBus;

        public PlayAlbumCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override async void Execute(AlbumViewModel parameter)
        {
            await _presentationBus.SendAsync(new PlayAlbumNowCommand(parameter.ArtistName, parameter.GetAlbum()));
        }

    }
}