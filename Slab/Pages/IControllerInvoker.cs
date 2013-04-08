using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Slab.Pages.Navigation;

namespace Slab.Pages
{
    public interface IControllerInvoker
    {
        ControllerInvokerResult Call<TController>(Expression<Func<TController, ActionResult>> action)
            where TController : IController;

        Task<ControllerInvokerResult> CallAsync<TController>(Expression<Func<TController, Task<ActionResult>>> action)
            where TController : IController;

        Task<ControllerInvokerResult> CallAsync(string route);
    }
}