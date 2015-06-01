using Autofac;
using Orienteer.Android.Data.Navigation;
using Orienteer.Pages.Navigation;

namespace FormsSample.Droid.Modules
{
    public class NavigationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<AndroidNavigationStackStorage>()
                .As<INavigationStackStorage>()
                .InstancePerDependency();
        }
    }
}