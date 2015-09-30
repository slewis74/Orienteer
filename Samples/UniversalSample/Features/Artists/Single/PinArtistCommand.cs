using System;
using System.Linq;
using Orienteer.Pages;
using Orienteer.Universal.Commands;
using Sample.Shared;

namespace UniversalSample.Features.Artists.Single
{
    public class PinArtistCommand : TogglePinCommand
    {
        private readonly IAlbumArtStorage _albumArtStorage;
        private readonly ArtistViewModel _artistViewModel;

        public delegate PinArtistCommand Factory(ArtistViewModel artistViewModel);

        public PinArtistCommand(
            IAlbumArtStorage albumArtStorage,
            ArtistViewModel artistViewModel)
        {
            _albumArtStorage = albumArtStorage;
            _artistViewModel = artistViewModel;
        }

        public override string AppbarTileId
        {
            get
            {
                return string.Format("Artist.{0}",
                    _artistViewModel.Name.Replace(' ', '.').Replace('!', '.'));
            }
        }

        public override string TileTitle
        {
            get { return _artistViewModel.Name; }
        }

        public override string ActivationArguments
        {
            get { return "Artists/ShowArtist?name=" + _artistViewModel.Name; }
        }

        public override Uri TileMediumImageUri
        {
            get { return new Uri("ms-appdata:///local/" + _albumArtStorage.AlbumArtFileName(_artistViewModel.Albums.First().Folder, 150)); }
        }

        public override Uri TileLargeImageUri
        {
            get { return new Uri("ms-appdata:///local/" + _albumArtStorage.AlbumArtFileName(_artistViewModel.Albums.First().Folder, 310)); }
        }

        public override Uri TileWideImageUri
        {
            get { return new Uri("ms-appdata:///local/" + _albumArtStorage.AlbumArtFileName(_artistViewModel.Albums.First().Folder, 310)); }
        }
    }
}