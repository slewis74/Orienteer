using System;

namespace UniversalSample.Features.Albums
{
    public partial class AlbumView
    {
        public AlbumView()
        {
            InitializeComponent();
        }

        private void Pin_Click(object sender, EventArgs e)
        {
            ((AlbumViewModel)DataContext).PinAlbum.Execute(null);
        }
    }
}