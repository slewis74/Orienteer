using System.Reflection;
using Windows.UI.Xaml;
using Autofac;
using Orienteer.Autofac;
using Orienteer.Pages.Navigation;
using Orienteer.Universal;
using Orienteer.Universal.Data.Navigation;
using Orienteer.Universal.Pages;
using Orienteer.Xaml;

namespace UniversalSample.Modules
{
    public class NavigationModule : Autofac.Module
    {
        private const string DefaultRoute = "Artists/ShowAll";
        private const string ViewRootNamespace = "UniversalSample.Features";
        private const string ViewModelRootNamespace = "UniversalSample.Features";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .RegisterType<NavigationStack>()
                .As<INavigationStack>()
                .WithParameter("defaultRoute", DefaultRoute)
                .WithParameter("alwaysStartFromDefaultRoute", false)
                .InstancePerLifetimeScope()
                .PropertiesAutowired();

            builder
                .RegisterType<UniversalFrameAdapter>()
                .As<IUniversalFrameAdapter>()
                .WithParameter("viewRootNamespace", ViewRootNamespace)
                .WithParameter("baseFeatureFolder", "Features")
                .SingleInstance();

            builder
                .RegisterType<Navigator>().AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterType<AutofacViewFactory<FrameworkElement>>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterType<ViewLocator>()
                .As<IViewLocator>()
                .SingleInstance()
                .OnActivated(x => x.Instance.Configure(typeof(NavigationModule).GetTypeInfo().Assembly, ViewModelRootNamespace, ViewRootNamespace));

            builder.RegisterType<UniversalNavigationStackStorage>()
                .As<INavigationStackStorage>()
                .InstancePerDependency();
        }
    }
}