using PresentationBus;

namespace Orienteer.Universal.Messages
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