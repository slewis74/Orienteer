using System;
using System.Threading.Tasks;

namespace Orienteer.Data
{
    public abstract class DispatchesToUIThread
    {
        protected async Task DispatchCall(Action action)
        {
            await UIDispatcher.Execute(action);
        }
    }
}