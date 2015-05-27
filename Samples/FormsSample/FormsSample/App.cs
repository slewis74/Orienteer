using System.Reflection;
using System.Threading.Tasks;
using Autofac;
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
            LetThereBeIoC(callingAssembly);
        }

        private void LetThereBeIoC(Assembly callingAssembly)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(new[] { GetType().GetTypeInfo().Assembly, callingAssembly });

            _container = builder.Build();

            // ReSharper disable once CSharpWarnings::CS4014
            DoInit();

            // The root page of your application
            var orienteerNavigationPage = _container.Resolve<OrienteerNavigationPage>();
            MainPage = orienteerNavigationPage;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private async Task DoInit()
        {
            var musicProvider = _container.Resolve<IMusicProvider>();
            await musicProvider.LoadContent();

            // ReSharper disable once CSharpWarnings::CS4014
            Task.Run(async () =>
            {
                await musicProvider.ReScanMusicLibrary();
            });
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
