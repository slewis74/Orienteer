using Orienteer.Xaml.ViewModels;
using Sample.Shared.Requests;
using PresentationBus;

namespace FormsSample.Features.Albums
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