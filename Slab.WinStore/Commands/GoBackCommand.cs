using Slab.Requests;
using Slab.WinStore.Events;
using Slab.WinStore.Requests;
using Slab.Xaml.ViewModels;
using Slew.PresentationBus;

namespace Slab.WinStore.Commands
{
    public class GoBackCommand : 
        Command,
        IHandlePresentationEvent<CanGoBackChanged>
    {
        private readonly IPresentationBus _presentationBus;
        private bool? _canGoBack;

        public GoBackCommand(IPresentationBus presentationBus)
        {
            _presentationBus = presentationBus;
        }

        public override bool CanExecute(object parameter)
        {
            if (_canGoBack.HasValue == false)
                _canGoBack = RequestCanGoBackState();
            return _canGoBack.GetValueOrDefault();
        }

        private bool? RequestCanGoBackState()
        {
            var request = new CanGoBackRequest();
            _presentationBus.PublishAsync(request);
            return request.CanGoBack;
        }

        public override void Execute(object parameter)
        {
            _presentationBus.PublishAsync(new GoBackRequest());
        }

        public void Handle(CanGoBackChanged presentationEvent)
        {
            _canGoBack = presentationEvent.CanGoBack;
            RaiseCanExecuteChanged();
        }
    }
}