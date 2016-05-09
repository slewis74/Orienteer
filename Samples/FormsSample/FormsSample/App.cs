using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Orienteer.Data;
using Orienteer.Forms.Data;
using Orienteer.Forms.Pages;
using Sample.Shared;
using Xamarin.Forms;

namespace FormsSample
{
    public class App : Application
    {
        private IContainer _container;

        /// <summary>
        /// Constructs the application, including IoC container setup.
        /// </summary>
        /// <param name="callingAssembly">The calling assembly, which will be scanned for Modules for the IoC configuration.</param>
        public App(Assembly callingAssembly)
        {
            try
            {
                UIDispatcher.Initialize(new FormsUIThreadDispatchHandler());

                LetThereBeIoC(callingAssembly);

                var navigationPage = _container.Resolve<OrienteerNavigationPage>();
                MainPage = navigationPage;

                // If DoStartup isn't called here then iOS/Android crashes.
                navigationPage.DoStartup();

                // ReSharper disable once CSharpWarnings::CS4014
                DoRescan();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: {0} {1}", ex.Message, ex.StackTrace);
                throw;
            }
        }

        private void LetThereBeIoC(Assembly callingAssembly)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(new[] { GetType().GetTypeInfo().Assembly, callingAssembly });

            _container = builder.Build();
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private void DoRescan()
        {
            var musicProvider = _container.Resolve<IMusicProvider>();

            Task.Run(async () => await musicProvider.ReScanMusicLibrary());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
