using System;

namespace Orienteer.Pages
{
    public interface IControllerFactory
    {
        TController Create<TController>() where TController : IController;
        object Create(Type controllerType);
    }
}