using System;
using System.Threading.Tasks;

namespace Orienteer.Data
{
    public interface IUIThreadDispatchHandler
    {
        Task ExecuteOnUIThread(Action action);
    }
}