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

        public override void Execute(ArtistViewModel parameter)
        {
            var artist = parameter.GetArtist();
            _presentationBus.Send(new PlayArtistNowCommand(artist));
        }
    }
}