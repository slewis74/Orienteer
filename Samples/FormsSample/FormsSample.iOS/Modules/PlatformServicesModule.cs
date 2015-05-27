using Autofac;
using FormsSample.iOS.PlatformServices;
using Sample.Shared;

namespace FormsSample.iOS.Modules
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