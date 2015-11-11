using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Orienteer.Data;

namespace Orienteer.WinStore.Data
{
    public class WinStoreUIThreadDispatchHandler : IUIThreadDispatchHandler
    {
        private readonly CoreDispatcher _dispatcher;

        public WinStoreUIThreadDispatchHandler()
        {
            _dispatcher = Window.Current.Dispatcher;
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