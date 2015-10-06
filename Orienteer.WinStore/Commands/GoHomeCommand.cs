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

        public override void Execute(object parameter)
        {
            _presentationBus.Send(new Messages.GoHomeCommand());
        }
    }
}