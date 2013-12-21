using Slab.Messages;
using Slab.PresentationBus;
using Slab.ViewModels;

namespace SlabRt.Commands
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
            _presentationBus.Publish(new GoHomeRequest());
        }
    }
}