using System.Threading.Tasks;
using Orienteer.Universal.Messages;
using Orienteer.Xaml.ViewModels;
using PresentationBus;

namespace Orienteer.Universal.Commands
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
                RequestCanGoBackState();
            return _canGoBack.GetValueOrDefault();
        }

        private async Task RequestCanGoBackState()
        {
            var request = new CanGoBackRequest();
            var response = await _presentationBus.Request(request);
            _canGoBack = response.CanGoBack;
            RaiseCanExecuteChanged();
        }

        public override void Execute(object parameter)
        {
            _presentationBus.Send(new Orienteer.Messages.GoBackCommand());
        }

        public void Handle(CanGoBackChanged presentationEvent)
        {
            _canGoBack = presentationEvent.CanGoBack;
            RaiseCanExecuteChanged();
        }
    }
}