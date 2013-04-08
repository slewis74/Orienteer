using System;
using System.Linq.Expressions;
using Slab.Pages.Navigation;

namespace Slab.Pages
{
    public interface IContollerInvoker
    {
        ActionResult Call<TController>(Expression<Func<TController, ActionResult>> action)
            where TController : IController;
    }
}