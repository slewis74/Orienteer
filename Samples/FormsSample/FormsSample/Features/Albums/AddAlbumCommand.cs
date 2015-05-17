using Orienteer.Xaml.ViewModels;
using Sample.Shared.Requests;
using Slew.PresentationBus;

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
            _presentationBus.PublishAsync(new AddAlbumToCurrentPlaylistRequest { Album = parameter.GetAlbum() });
        }
    }
}