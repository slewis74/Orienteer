using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Slab.Requests;
using Slew.PresentationBus;

namespace Slab.Pages.Navigation
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

        public void Navigate<TController>(Expression<Func<TController, ActionResult>> action)
            where TController : IController
        {
            var controllerResult = _controllerInvoker.Call(action);

            DoNavigate(controllerResult);
        }

        public async void Navigate<TController>(Expression<Func<TController, Task<ActionResult>>> action)
            where TController : IController
        {
            var controllerResult = await _controllerInvoker.CallAsync(action);

            DoNavigate(controllerResult);
        }

        public async void Navigate(string route)
        {
            var controllerResult = await _controllerInvoker.CallAsync(route);
            DoNavigate(controllerResult);
        }

        protected virtual void DoNavigate(ControllerInvokerResult controllerResult)
        {
            var route = controllerResult.Route;
            var result = controllerResult.Result;

            var pageResult = result as IPageActionResult;
            if (pageResult != null)
            {
                _presentationBus.PublishAsync(new PageNavigationRequest(route,
                                                                   new PageNavigationRequestEventArgs(pageResult.PageType,
                                                                                                      pageResult.Parameter)));
                return;
            }

            var viewModelResult = result as IViewModelActionResult;
            if (viewModelResult != null)
            {
                _presentationBus.PublishAsync(new ViewModelNavigationRequest(route,
                                                                        new ViewModelNavigationRequestEventArgs(
                                                                            viewModelResult.ViewModelInstance)));
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