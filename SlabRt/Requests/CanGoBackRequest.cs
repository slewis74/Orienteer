using Slab.PresentationBus;

namespace SlabRt.Requests
{
    public class CanGoBackRequest : PresentationRequest
    {
        public bool CanGoBack { get; set; }
    }
}