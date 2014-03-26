using Slab.PresentationBus;

namespace Slab.WinStore.Requests
{
    public class CanGoBackRequest : PresentationRequest
    {
        public bool CanGoBack { get; set; }
    }
}