using Slew.PresentationBus;

namespace Orienteer.WinStore.Events
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