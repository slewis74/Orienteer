using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Orienteer.Data;

namespace Sample.Shared.Model
{
    [DebuggerDisplay("Artist - {Name}")]
	public class Artist : Bindable
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

        protected bool Equals(Artist other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Artist) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}