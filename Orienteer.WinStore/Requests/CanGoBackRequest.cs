using Slew.PresentationBus;

namespace Orienteer.WinStore.Requests
{
    public class CanGoBackRequest : PresentationRequest
    {
        public bool CanGoBack { get; set; }
    }
}