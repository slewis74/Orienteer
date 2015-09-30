using Autofac;
using Sample.Shared;
using UniversalSample.PlatformServices;

namespace UniversalSample.Modules
{
    public class PlatformServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<MusicProvider>()
                .As<IMusicProvider>()
                .SingleInstance();
        }
    }
}