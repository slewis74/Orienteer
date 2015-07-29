using Orienteer.Pages;
using Orienteer.WinPhone.Commands;

namespace WinPhoneSample.Features.Artists.Single
{
    public class PinArtistCommand : PinSecondaryTileCommand
    {
        public delegate PinArtistCommand Factory(string artistName);

        private readonly IControllerRouteConverter _controllerRouteConverter;
        private readonly string _artistName;

        public PinArtistCommand(IControllerRouteConverter controllerRouteConverter, string artistName)
        {
            _controllerRouteConverter = controllerRouteConverter;
            _artistName = artistName;
        }

        public override string TileTitle
        {
            get { return _artistName; }
        }

        public override string ActivationRoute
        {
            get { return _controllerRouteConverter.GetAsyncRoute<ArtistsController>(c => c.ShowArtist(_artistName)).Route; }
        }
    }
}