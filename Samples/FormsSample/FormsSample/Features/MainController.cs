using System;
using System.Threading.Tasks;
using FormsSample.Features.Home;
using Orienteer.Pages;
using Orienteer.Pages.Navigation;
using Sample.Shared;

namespace FormsSample.Features
{
    public class MainController : Controller
    {
        private readonly Func<HomeViewModel> _homeViewModelFactory;
        private readonly IMusicProvider _musicProvider;

        public MainController(
            Func<HomeViewModel> homeViewModelFactory,
            IMusicProvider musicProvider)
        {
            _homeViewModelFactory = homeViewModelFactory;
            _musicProvider = musicProvider;
        }

        public ActionResult Home()
        {
            var artists = _musicProvider.Artists;

            return new ViewModelActionResult(_homeViewModelFactory);
        }
         
    }
}