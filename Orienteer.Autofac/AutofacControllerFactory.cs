using System;
using Autofac;
using Orienteer.Pages;

namespace Orienteer.Autofac
{
    public class AutofacControllerFactory : ControllerFactory
    {
        private readonly IComponentContext _componentContext;

        public AutofacControllerFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public override object Create(Type controllerType)
        {
            return _componentContext.Resolve(controllerType);
        }
    }
}