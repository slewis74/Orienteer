using Slab.PresentationBus;

namespace Slab.WinStore.Requests
{
    public class CanGoBackChanged : PresentationEvent
    {
        public CanGoBackChanged(bool canGoBack)
        {
            CanGoBack = canGoBack;
        }

        public bool CanGoBack { get; set; }
    }
}