using System.Linq;
using Orienteer.Data;
using Orienteer.Pages.Navigation;
using Orienteer.Xaml.ViewModels;
using Sample.Shared.Model;
using Slew.PresentationBus;

namespace WinPhoneSample.Features.Albums
{
    public class AlbumViewModel : CanRequestNavigationBase
    {
        private readonly Artist _artist;
        private readonly Album _album;

        public delegate AlbumViewModel Factory(Artist artist, Album album);

        public AlbumViewModel(
            IPresentationBus presentationBus,
            INavigator navigator,
            Artist artist,
            Album album,
            PinAlbumCommand.Factory pinAlbumCommandFactory)
            : base(navigator)
        {
            _artist = artist;
            _album = album;

            PinAlbum = pinAlbumCommandFactory(_artist.Name, _album.Title);

            PlaySong = new PlaySongCommand(presentationBus);
            PlayAlbum = new PlayAlbumCommand(presentationBus);
            AddSong = new AddSongCommand(presentationBus);
            AddAlbum = new AddAlbumCommand(presentationBus);

            Tracks = new DispatchingObservableCollection<TrackViewModel>(
                album.Songs
                .OrderBy(s => s.DiscNumber)
                .ThenBy(s => s.TrackNumber)
                .Select(t => new TrackViewModel(artist.Name, album.Title, t)));
        }

        public override string PageTitle
        {
            get { return "Album"; }
        }

        public PlaySongCommand PlaySong { get; private set; }
        public PlayAlbumCommand PlayAlbum { get; private set; }
        public AddSongCommand AddSong { get; private set; }
        public AddAlbumCommand AddAlbum { get; private set; }

        public PinAlbumCommand PinAlbum { get; set; }

        public string Title { get { return _album.Title; } }
        public string ArtistName { get { return _artist.Name; } }

        public string SmallBitmapUri { get { return _album.SmallBitmapUri; } }
        public string LargeBitmapUri { get { return _album.LargeBitmapUri; } }

        public DispatchingObservableCollection<TrackViewModel> Tracks { get; private set; }

        private TrackViewModel _selectedTrack;
        public TrackViewModel SelectedTrack
        {
            get { return _selectedTrack; }
            set
            {
                _selectedTrack = value;
                NotifyChanged(() => SelectedTrack);
            }
        }

        public Album GetAlbum()
        {
            return _album;
        }
    }
}