using Orienteer.Xaml.ViewModels;
using PresentationBus;
using Sample.Shared.Requests;

namespace UniversalSample.Features.Artists.Single
{
    public class PlayArtistCommand : Command<ArtistViewModel>
    {
        private readonly IPresentationBus _presentationBus;

        public PlayArtistCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override async void Execute(ArtistViewModel parameter)
        {
            var artist = parameter.GetArtist();
            await _presentationBus.SendAsync(new PlayArtistNowCommand(artist));
        }
    }
}