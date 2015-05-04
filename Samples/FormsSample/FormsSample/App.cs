using System.Reflection;
using Autofac;
using Slab.Forms.Pages;
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

            // The root page of your application
            var slabNavigationPage = _container.Resolve<SlabNavigationPage>();
            MainPage = slabNavigationPage;

            slabNavigationPage.DoStartup();
        }

        private void LetThereBeIoC(Assembly callingAssembly)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(new[] { this.GetType().GetTypeInfo().Assembly, callingAssembly });

            _container = builder.Build();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
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
