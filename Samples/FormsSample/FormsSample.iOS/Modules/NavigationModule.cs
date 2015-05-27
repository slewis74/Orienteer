using Autofac;
using Orienteer.iOS.Data.Navigation;
using Orienteer.Pages.Navigation;

namespace FormsSample.iOS.Modules
{
    public class NavigationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<iOSNavigationStackStorage>()
                .As<INavigationStackStorage>()
                .InstancePerDependency();
        }
    }
}