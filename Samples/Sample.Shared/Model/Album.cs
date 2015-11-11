using System.Collections.ObjectModel;
using System.Diagnostics;
using Orienteer.Data;

namespace Sample.Shared.Model
{
    [DebuggerDisplay("Album - {Title}")]
    public class Album : Bindable
	{
        public Album()
        {
            Songs = new ObservableCollection<Song>();
        }

        public string Title { get; set; }

		public ObservableCollection<Song> Songs { get; private set; }

        private string _smallBitmapUri;
        public string SmallBitmapUri
        {
            get { return _smallBitmapUri; }
            set { _smallBitmapUri = value; NotifyChanged(() => SmallBitmapUri); }
        }

        private string _largeBitmapUri;
        public string LargeBitmapUri
        {
            get { return _largeBitmapUri; }
            set { _largeBitmapUri = value; NotifyChanged(() => LargeBitmapUri); }
        }

        public string Folder { get; set; }
	}
}