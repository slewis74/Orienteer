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

        public override void Execute(AlbumViewModel parameter)
        {
            _presentationBus.Send(new PlayAlbumNowCommand(parameter.ArtistName, parameter.GetAlbum()));
        }

    }
}