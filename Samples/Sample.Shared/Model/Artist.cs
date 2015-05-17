using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Orienteer.Data;

namespace Sample.Shared.Model
{
    [DebuggerDisplay("Artist - {Name}")]
	public class Artist : BindableBase
    {
        public Artist()
        {
            Albums = new ObservableCollection<Album>();
        }

        public string Name { get; set; }

        public ObservableCollection<Album> Albums { get; private set; }

		public void AddAlbum(Album album)
		{
			if (Albums.Contains(album))
				return;
			Albums.Add(album);
		}

        public string SmallBitmapUri
        {
            get
            {
                var album = Albums.FirstOrDefault();
                return album == null ? string.Empty : album.SmallBitmapUri;
            }
        }

        public string LargeBitmapUri
        {
            get
            {
                var album = Albums.FirstOrDefault();
                return album == null ? string.Empty : album.LargeBitmapUri;
            }
        }
	}
}