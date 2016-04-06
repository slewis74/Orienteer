using Orienteer.Xaml.ViewModels;
using PresentationBus;

namespace Orienteer.WinStore.Commands
{
    public class GoHomeCommand : Command
    {
        private readonly IPresentationBus _presentationBus;

        public GoHomeCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override async void Execute(object parameter)
        {
            await _presentationBus.SendAsync(new Orienteer.Messages.GoHomeCommand());
        }
    }
}