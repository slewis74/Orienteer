using Autofac;
using Autofac.Core;
using PresentationBus;

namespace UniversalSample.Modules
{
    public class PresentationBusSubscriptionModule : Autofac.Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry registry, IComponentRegistration registration)
        {
            registration.Activated += OnComponentActivated;
        }

        static void OnComponentActivated(object sender, ActivatedEventArgs<object> e)
        {
            if (e == null)
                return;

            var handler = e.Instance as IHandlePresentationMessages;
            if (handler == null)
                return;
            var bus = e.Context.Resolve<IPresentationBusConfiguration>();
            bus.Subscribe(handler);
        }
    }
}