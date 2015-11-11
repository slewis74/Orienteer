﻿using System.Collections.ObjectModel;
using Orienteer.Pages.Navigation;
using Orienteer.Xaml.ViewModels;
using Sample.Shared.Model;
using PresentationBus;

namespace FormsSample.Features.Artists.Single
{
    public class ArtistViewModel : CanRequestNavigation
    {
        private readonly Artist _artist;

        public delegate ArtistViewModel Factory(Artist artist);

        public ArtistViewModel(
            IPresentationBus presentationBus,
            INavigator navigator,
            Artist artist)
            : base(navigator)
        {
            _artist = artist;

            DisplayAlbum = new DisplayAlbumCommand(Navigator, _artist);

            PlayArtist = new PlayArtistCommand(presentationBus);
        }

        public override string PageTitle
        {
            get { return "Artist"; }
        }

        public DisplayAlbumCommand DisplayAlbum { get; private set; }

        public string Name { get { return _artist.Name; } }

        public ObservableCollection<Album> Albums { get { return _artist.Albums; } }

        public string SmallBitmapUri { get { return _artist.SmallBitmapUri; } }
        public string LargeBitmapUri { get { return _artist.LargeBitmapUri; } }

        public PlayArtistCommand PlayArtist { get; private set; }

        public Artist GetArtist()
        {
            return _artist;
        }
    }
}