using Slab.PresentationBus;

namespace Slab.ViewModels
{
    public class PresentationRequestCommand<T> : Command
        where T : IPresentationRequest, new()
    {
        private readonly IPresentationBus _presentationBus;

        public PresentationRequestCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override void Execute(object parameter)
        {
            _presentationBus.PublishAsync(new T());
        }
    }
}