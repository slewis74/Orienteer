using Orienteer.Pages;
using Orienteer.WinPhone.Commands;

namespace WinPhoneSample.Features.Albums
{
    public class PinAlbumCommand : PinSecondaryTileCommand
    {
        public delegate PinAlbumCommand Factory(string artistName, string albumTitle);

        private readonly IControllerRouteConverter _controllerRouteConverter;
        private readonly string _artistName;
        private readonly string _albumTitle;

        public PinAlbumCommand(IControllerRouteConverter controllerRouteConverter, string artistName, string albumTitle)
        {
            _controllerRouteConverter = controllerRouteConverter;
            _artistName = artistName;
            _albumTitle = albumTitle;

            WideContent1 = _artistName;
        }

        public override string TileTitle
        {
            get { return _albumTitle; }
        }

        public override string ActivationRoute
        {
            get { return _controllerRouteConverter.GetAsyncRoute<AlbumController>(c => c.ShowAlbum(_artistName, _albumTitle)).Route; }
        }
    }
}