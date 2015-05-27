using Autofac;
using Orienteer.Pages.Navigation;
using Orienteer.WinPhone.Data.Navigation;

namespace FormsSample.WinPhone.Modules
{
    public class NavigationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<WinPhoneNavigationStackStorage>()
                .As<INavigationStackStorage>()
                .InstancePerDependency();
        }
    }
}