using System;

namespace Slab.Pages
{
    public interface IControllerFactory
    {
        TController Create<TController>() where TController : IController;
        object Create(Type controllerType);
    }
}