using System.Reflection;
using Autofac;
using Orienteer.Autofac;
using Orienteer.Pages;

namespace FormsSample.Modules
{
    public class ControllerModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<AutofacControllerFactory>()
                .As<IControllerFactory>()
                .InstancePerLifetimeScope();

            builder
                .RegisterAssemblyTypes(typeof(ControllerModule).GetTypeInfo().Assembly)
                .Where(t => t.IsAssignableTo<IController>())
                .AsSelf()
                .InstancePerDependency();
            builder
                .RegisterAssemblyTypes(typeof(ControllerModule).GetTypeInfo().Assembly)
                .Where(t => t.IsAssignableTo<IController>())
                .As<IController>()
                .InstancePerDependency();

            builder.RegisterType<ControllerLocator>().As<IControllerLocator>().SingleInstance();
            builder.RegisterType<ControllerInvoker>().As<IControllerInvoker>().InstancePerDependency();
            builder.RegisterType<ControllerRouteConverter>().As<IControllerRouteConverter>().InstancePerDependency();
        }
    }
}