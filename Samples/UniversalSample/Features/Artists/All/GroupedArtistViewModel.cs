using Orienteer.Data;
using Sample.Shared.Model;

namespace UniversalSample.Features.Artists.All
{
    public class GroupedArtistViewModel : BindableBase
    {
        public GroupedArtistViewModel(Artist artist)
        {
            Artist = artist;
        }

        public Artist Artist { get; set; }

        private int _horizontalSize = 1;
        public int HorizontalSize
        {
            get { return _horizontalSize; }
            set { SetProperty(ref _horizontalSize, value); }
        }

        private int _verticalSize = 1;
        public int VerticalSize
        {
            get { return _verticalSize; }
            set { SetProperty(ref _verticalSize, value); }
        }
    }
}