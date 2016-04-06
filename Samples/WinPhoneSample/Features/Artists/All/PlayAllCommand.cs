using System.Collections.Generic;
using Orienteer.Xaml.ViewModels;
using Sample.Shared.Model;
using Sample.Shared.Requests;
using PresentationBus;

namespace WinPhoneSample.Features.Artists.All
{
    public class PlayAllCommand : Command
    {
        private readonly IPresentationBus _presentationBus;
        private readonly IEnumerable<Artist> _artists;

        public PlayAllCommand(IPresentationBus presentationBus, IEnumerable<Artist> artists)
        {
            _presentationBus = presentationBus;
            _artists = artists;
        }

        public override async void Execute(object parameter)
        {
            await _presentationBus.SendAsync(new PlayAllNowCommand(_artists));
        }
    }
}