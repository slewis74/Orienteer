using Orienteer.Data;
using Orienteer.Requests;
using Slew.PresentationBus;

namespace Orienteer.Universal
{
    public class UniversalFrameAdapter :
        DispatchesToOriginalThreadBase,
        IUniversalFrameAdapter,
        IHandlePresentationEvent<ViewModelNavigationRequest>
    {
         
    }

    public interface IUniversalFrameAdapter
    {
        
    }
}