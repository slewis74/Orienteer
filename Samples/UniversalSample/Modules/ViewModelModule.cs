using System.Reflection;
using System.Windows.Input;
using Autofac;
using Orienteer.Universal.Commands;

namespace UniversalSample.Modules
{
    public class ViewModelModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterAssemblyTypes(typeof(ViewModelModule).GetTypeInfo().Assembly)
                .Where(t => t.Name.EndsWith("ViewModel"))
                .AsSelf()
                .InstancePerDependency();

            builder
                .RegisterAssemblyTypes(typeof(GoBackCommand).GetTypeInfo().Assembly)
                .Where(t => t.IsAssignableTo<ICommand>())
                .AsSelf()
                .InstancePerDependency();

            builder
                .RegisterAssemblyTypes(typeof(ViewModelModule).GetTypeInfo().Assembly)
                .Where(t => t.IsAssignableTo<ICommand>())
                .AsSelf()
                .InstancePerDependency();
        }
    }
}