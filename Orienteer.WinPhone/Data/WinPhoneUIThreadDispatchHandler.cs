using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Orienteer.Data;

namespace Orienteer.WinPhone.Data
{
    public class WinPhoneUIThreadDispatchHandler : IUIThreadDispatchHandler
    {
        private readonly CoreDispatcher _dispatcher;

        public WinPhoneUIThreadDispatchHandler()
        {
            var window = CoreWindow.GetForCurrentThread();
            _dispatcher = window.Dispatcher;
        }

        public async Task ExecuteOnUIThread(Action action)
        {
            if (_dispatcher.HasThreadAccess)
            {
                action();
                return;
            }

            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }
    }
}