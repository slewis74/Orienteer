using PresentationBus;

namespace Orienteer.WinStore.Messages
{
    public class CanGoBackRequest : PresentationRequest<CanGoBackRequest, CanGoBackResponse>
    {
    }

    public class CanGoBackResponse : IPresentationResponse
    {
        public bool CanGoBack { get; set; }
    }
}