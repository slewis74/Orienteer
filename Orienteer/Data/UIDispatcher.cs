using System;
using System.Threading.Tasks;

namespace Orienteer.Data
{
    public static class UIDispatcher
    {
        private static IUIThreadDispatchHandler _handler;

        public static void Initialize(IUIThreadDispatchHandler handler)
        {
            _handler = handler;
        }

        public async static Task Execute(Action action)
        {
            if (_handler == null)
                throw new InvalidOperationException("UIDispatcher.Initialize must be called before calling Execute");
            await _handler.ExecuteOnUIThread(action);
        }
    }
}