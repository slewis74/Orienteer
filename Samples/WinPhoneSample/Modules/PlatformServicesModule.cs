using Autofac;
using FormsSample.WinPhone.PlatformServices;
using Sample.Shared;

namespace WinPhoneSample.Modules
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