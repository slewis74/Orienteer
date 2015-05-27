using System.Reflection;
using Autofac;
using Orienteer.Forms.Pages;
using Orienteer.Pages.Navigation;
using Module = Autofac.Module;

namespace FormsSample.Modules
{
    public class NavigationModule : Module
    {
        private const string DefaultRoute = "Artists/ShowAll";
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
                .RegisterType<OrienteerNavigationPage>()
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