using System;
using System.Linq.Expressions;
using Orienteer.Pages.Navigation;

namespace Orienteer.Pages
{
    public interface IContollerInvoker
    {
        ActionResult Call<TController>(Expression<Func<TController, ActionResult>> action)
            where TController : IController;
    }
}