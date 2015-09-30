using Autofac;
using PresentationBus;

namespace UniversalSample.Modules
{
    public class PresentationBusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PresentationBus.PresentationBus>()
                .As<IPresentationBus>()
                .As<IPresentationBusConfiguration>()
                .SingleInstance();
        }
    }
}