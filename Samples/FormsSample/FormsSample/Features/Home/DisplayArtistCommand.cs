using FormsSample.Features.Artists;
using Orienteer.Pages.Navigation;
using Orienteer.Xaml.ViewModels;
using Sample.Shared.Model;

namespace FormsSample.Features.Home
{
    public class DisplayArtistCommand : NavigationCommand<Artist>
    {
        public DisplayArtistCommand(INavigator navigator)
            : base(navigator)
        { }

        public override void Execute(Artist parameter)
        {
            Navigator.Navigate<ArtistsController>(c => c.ShowArtist(parameter.Name));
        }
    }
}