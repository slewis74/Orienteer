using Orienteer.Pages.Navigation;
using Orienteer.Xaml.ViewModels;
using Sample.Shared.Model;

namespace FormsSample.Features.Artists.All
{
    public class DisplayArtistCommand : NavigationCommand<Artist>
    {
        public DisplayArtistCommand(INavigator navigator)
            : base(navigator)
        { }

        public override void Execute(Artist parameter)
        {
            Navigator.NavigateAsync<ArtistsController>(c => c.ShowArtist(parameter.Name));
        }
    }
}