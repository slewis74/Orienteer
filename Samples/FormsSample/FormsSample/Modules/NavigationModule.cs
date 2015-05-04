using System.Reflection;
using Autofac;
using Slab.Forms.Pages;
using Slab.Pages.Navigation;
using Module = Autofac.Module;

namespace FormsSample.Modules
{
    public class NavigationModule : Module
    {
        private const string DefaultRoute = "Main/Home";
        private const string ViewRootNamespace = "FormsSample.Features";
        private const string ViewModelRootNamespace = "FormsSample.Features";

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<NavigationStack>()
                .As<INavigationStack>()
                .WithParameter("defaultRoute", DefaultRoute)
                .WithParameter("alwaysStartFromDefaultRoute", false)
                .InstancePerLifetimeScope()
                .PropertiesAutowired();

            builder
                .RegisterType<SlabNavigationPage>()
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterType<Navigator>().AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterType<ViewLocator>()
                .As<IViewLocator>()
                .SingleInstance()
                .OnActivated(x => x.Instance.Configure(typeof(NavigationModule).GetTypeInfo().Assembly, ViewModelRootNamespace, ViewRootNamespace));
        }
    }

}