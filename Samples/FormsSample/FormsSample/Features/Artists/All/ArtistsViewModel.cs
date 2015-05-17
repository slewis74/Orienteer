using System.Linq;
using FormsSample.Features.Home;
using Orienteer.Data;
using Orienteer.Pages.Navigation;
using Orienteer.Xaml.ViewModels;
using Sample.Shared;
using Sample.Shared.Model;
using Slew.PresentationBus;

namespace FormsSample.Features.Artists.All
{
    public class ArtistsViewModel : CanRequestNavigationBase
    {
        private readonly DistinctAsyncObservableCollection<Artist> _artists;
        private DispatchingObservableCollection<GroupedData<GroupedArtistViewModel>> _groups;

        public delegate ArtistsViewModel Factory();

        public ArtistsViewModel(
            IPresentationBus presentationBus,
            INavigator navigator,
            IMusicProvider musicProvider)
            : base(navigator)
        {
            _artists = musicProvider.Artists;
            _artists.CollectionChanged += (sender, args) => NotifyChanged(() => GroupedItems);

            DisplayArtist = new DisplayArtistCommand(Navigator);
            PlayAll = new PlayAllCommand(presentationBus, _artists);
        }

        public override string PageTitle
        {
            get { return "Albums"; }
        }

        public DisplayArtistCommand DisplayArtist { get; private set; }
        public PlayAllCommand PlayAll { get; private set; }

        public DistinctAsyncObservableCollection<Artist> Items { get { return _artists; } }

        public DispatchingObservableCollection<GroupedData<GroupedArtistViewModel>> GroupedItems
        {
            get
            {
                if (_groups == null)
                    _groups = new DispatchingObservableCollection<GroupedData<GroupedArtistViewModel>>();

                _groups.StartLargeUpdate();
                _groups.Clear();
                var query = from item in _artists
                            orderby item.Name
                            group item by item.Name.Substring(0, 1) into g
                            select new { GroupName = g.Key, Items = g };
                foreach (var g in query)
                {
                    var info = new GroupedData<GroupedArtistViewModel>
                    {
                        Key = g.GroupName
                    };
                    info.AddRange(g.Items.Select(z => new GroupedArtistViewModel(z)));
                    var size = 2;

                    info[0].HorizontalSize = size;
                    info[0].VerticalSize = size;
                    _groups.Add(info);
                }
                _groups.CompleteLargeUpdate();

                return _groups;
            }
        }
    }
}