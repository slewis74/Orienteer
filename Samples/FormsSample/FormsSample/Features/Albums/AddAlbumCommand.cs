using Orienteer.Xaml.ViewModels;
using Sample.Shared.Requests;
using PresentationBus;

namespace FormsSample.Features.Albums
{
    public class AddAlbumCommand : Command<AlbumViewModel>
    {
        private readonly IPresentationBus _presentationBus;

        public AddAlbumCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override void Execute(AlbumViewModel parameter)
        {
            _presentationBus.Send(new AddAlbumToCurrentPlaylistCommand { Album = parameter.GetAlbum() });
        }
    }
}