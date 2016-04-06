using PresentationBus;

namespace Orienteer.Xaml.ViewModels
{
    public class PresentationCommandSenderCommand<T> : Command
        where T : IPresentationCommand, new()
    {
        private readonly IPresentationBus _presentationBus;

        public PresentationCommandSenderCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override async void Execute(object parameter)
        {
            await _presentationBus.SendAsync(new T());
        }
    }
}