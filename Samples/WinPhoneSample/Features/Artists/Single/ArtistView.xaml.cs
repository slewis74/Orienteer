using System;

namespace WinPhoneSample.Features.Artists.Single
{
    public partial class ArtistView
    {
        public ArtistView()
        {
            InitializeComponent();
        }

        private void Pin_Click(object sender, EventArgs e)
        {
            ((ArtistViewModel)DataContext).PinArtist.Execute(null);
        }
    }
}