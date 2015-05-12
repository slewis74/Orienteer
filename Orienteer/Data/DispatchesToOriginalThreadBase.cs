using System.Threading;

namespace Orienteer.Data
{
    public abstract class DispatchesToOriginalThreadBase
    {
        protected readonly SynchronizationContext SynchronizationContext = SynchronizationContext.Current;

        protected DispatchesToOriginalThreadBase()
        {
        }

        protected DispatchesToOriginalThreadBase(SynchronizationContext synchronizationContext)
        {
            SynchronizationContext = synchronizationContext;            
        }

        protected void DispatchCall(SendOrPostCallback call)
        {
            if (SynchronizationContext.Current != SynchronizationContext)
            {
                SynchronizationContext.Post(call, null);
            }
            else
            {
                call(null);
            }
        }
    }
}