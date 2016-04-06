using System;
using System.Threading;
using System.Threading.Tasks;
using Orienteer.Data;

namespace Orienteer.Forms.Data
{
    public class FormsUIThreadDispatchHandler : IUIThreadDispatchHandler
    {
        private readonly SynchronizationContext _context;

        public FormsUIThreadDispatchHandler()
        {
            _context = SynchronizationContext.Current;
        }

        public async Task ExecuteOnUIThread(Action action)
        {
            _context.Send(_ => action(), null);
        }
    }
}