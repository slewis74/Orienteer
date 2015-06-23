using System.Reflection;
using Autofac;

namespace FormsSample.Modules
{
    public class ViewModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(typeof(ViewModule).GetTypeInfo().Assembly)
                .Where(t => t.Name.EndsWith("View"))
                .AsSelf()
                .InstancePerDependency();
        }
    }
}