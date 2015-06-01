using Autofac;
using FormsSample.Droid.PlatformServices;
using Sample.Shared;

namespace FormsSample.Droid.Modules
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