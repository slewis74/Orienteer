using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Orienteer.Pages.Navigation;

namespace Orienteer.Pages
{
    public interface IControllerRouteConverter
    {
        RouteDescriptor GetRoute<TController>(Expression<Func<TController, ActionResult>> action)
            where TController : IController;

        RouteDescriptor GetAsyncRoute<TController>(Expression<Func<TController, Task<ActionResult>>> action)
            where TController : IController;
    }
}