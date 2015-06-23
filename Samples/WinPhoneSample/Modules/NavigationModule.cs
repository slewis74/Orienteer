using System.Reflection;
using System.Windows;
using Autofac;
using Orienteer.Autofac;
using Orienteer.Pages.Navigation;
using Orienteer.WinPhone;
using Orienteer.WinPhone.Data.Navigation;
using Orienteer.WinPhone.Pages;
using Orienteer.Xaml;

namespace WinPhoneSample.Modules
{
    public class NavigationModule : Autofac.Module
    {
        private const string DefaultRoute = "Artists/ShowAll";
        private const string ViewRootNamespace = "WinPhoneSample.Features";
        private const string ViewModelRootNamespace = "WinPhoneSample.Features";

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
                .RegisterType<PhoneApplicationFrameAdapter>()
                .As<IPhoneApplicationFrameAdapter>()
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

            builder.RegisterType<WinPhoneNavigationStackStorage>()
                .As<INavigationStackStorage>()
                .InstancePerDependency();
        }
    }
}