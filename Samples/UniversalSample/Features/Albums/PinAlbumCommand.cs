using System;
using Orienteer.Universal.Commands;
using Sample.Shared;

namespace UniversalSample.Features.Albums
{
    public class PinAlbumCommand : TogglePinCommand
    {
        private readonly IAlbumArtStorage _albumArtStorage;
        private readonly AlbumViewModel _albumViewModel;

        public delegate PinAlbumCommand Factory(AlbumViewModel albumViewModel);

        public PinAlbumCommand(
            IAlbumArtStorage albumArtStorage,
            AlbumViewModel albumViewModel)
        {
            _albumArtStorage = albumArtStorage;
            _albumViewModel = albumViewModel;
        }

        public override string AppbarTileId
        {
            get
            {
                return string.Format("Album.{0}.{1}",
                    _albumViewModel.ArtistName.Replace(' ', '.').Replace('!', '.'),
                    _albumViewModel.Title.Replace(' ', '.').Replace(':', '.').Replace(",", string.Empty).Replace("!", string.Empty));
            }
        }

        public override string TileTitle
        {
            get { return _albumViewModel.Title; }
        }

        public override string ActivationArguments
        {
            get { return "Album/ShowAlbum?artistName=" + _albumViewModel.ArtistName + "&albumTitle=" + _albumViewModel.Title; }
        }

        public override Uri TileMediumImageUri
        {
            get { return new Uri("ms-appdata:///local/" + _albumArtStorage.AlbumArtFileName(_albumViewModel.GetAlbum().Folder, 150)); }
        }

        public override Uri TileLargeImageUri
        {
            get { return new Uri("ms-appdata:///local/" + _albumArtStorage.AlbumArtFileName(_albumViewModel.GetAlbum().Folder, 310)); }
        }

        public override Uri TileWideImageUri
        {
            get { return new Uri("ms-appdata:///local/" + _albumArtStorage.AlbumArtFileName(_albumViewModel.GetAlbum().Folder, 310)); }
        }
    }
}