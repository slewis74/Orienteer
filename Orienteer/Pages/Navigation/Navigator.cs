using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Orienteer.Messages;
using PresentationBus;

namespace Orienteer.Pages.Navigation
{
    public class Navigator : INavigator
    {
        private readonly IPresentationBus _presentationBus;
        private readonly IControllerInvoker _controllerInvoker;

        public Navigator(
            IPresentationBus presentationBus, 
            IControllerInvoker controllerInvoker)
        {
            _presentationBus = presentationBus;
            _controllerInvoker = controllerInvoker;
        }

        public void Navigate<TController>(Expression<Func<TController, ActionResult>> action, bool animated = true)
            where TController : IController
        {
            var controllerResult = _controllerInvoker.Call(action);

            DoNavigate(controllerResult, animated);
        }

        public async Task NavigateAsync<TController>(Expression<Func<TController, Task<ActionResult>>> action, bool animated = true)
            where TController : IController
        {
            var controllerResult = await _controllerInvoker.CallAsync(action);

            DoNavigate(controllerResult, animated);
        }

        public async Task NavigateAsync(string route, bool animated = true)
        {
            var controllerResult = await _controllerInvoker.CallAsync(route);
            DoNavigate(controllerResult, animated);
        }

        protected virtual async void DoNavigate(ControllerInvokerResult controllerResult, bool animated)
        {
            var route = controllerResult.Route;
            var result = controllerResult.Result;

            var pageResult = result as IPageActionResult;
            if (pageResult != null)
            {
                await _presentationBus.SendAsync(new PageNavigationCommand(route,
                                                                   new PageNavigationRequestEventArgs(pageResult.PageType,
                                                                                                      pageResult.Parameter)));
                return;
            }

            var viewModelResult = result as IViewModelActionResult;
            if (viewModelResult != null)
            {
                await _presentationBus.SendAsync(new ViewModelNavigationCommand(route,
                                                                        new ViewModelNavigationRequestEventArgs(
                                                                            viewModelResult.ViewModelInstance),
                                                                            animated));
            }
        }

        public DataActionResult<TData> GetData<TController, TData>(
            Expression<Func<TController, ActionResult>> action)
            where TController : IController
        {
            var controllerResult = _controllerInvoker.Call(action);
            var result = controllerResult.Result;

            if ((result is DataActionResult<TData>) == false)
            {
                throw new InvalidOperationException("Controller action must return a DataActionResult when using GetData");
            }
            return (DataActionResult<TData>)result;
        }

        public async Task<DataActionResult<TData>> GetDataAsync<TController, TData>(
            Expression<Func<TController, Task<ActionResult>>> action)
            where TController : IController
        {
            var controllerResult = await _controllerInvoker.CallAsync(action);
            var result = controllerResult.Result;

            if ((result is DataActionResult<TData>) == false)
            {
                throw new InvalidOperationException("Controller action must return a DataActionResult when using GetData");
            }
            return (DataActionResult<TData>)result;
        }
    }
}