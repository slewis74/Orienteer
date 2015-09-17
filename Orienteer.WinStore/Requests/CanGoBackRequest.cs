using PresentationBus;

namespace Orienteer.WinStore.Requests
{
    public class CanGoBackRequest : PresentationRequest<CanGoBackRequest, CanGoBackResponse>
    {
    }

    public class CanGoBackResponse : IPresentationResponse
    {
        public bool CanGoBack { get; set; }
    }
}