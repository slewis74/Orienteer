using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Orienteer.Pages.Navigation;

namespace Orienteer.Pages
{
    public class ControllerInvoker : IControllerInvoker
    {
        private readonly IControllerFactory _controllerFactory;
        private readonly IControllerLocator _controllerLocator;

        public ControllerInvoker(
            IControllerFactory controllerFactory,
            IControllerLocator controllerLocator)
        {
            _controllerFactory = controllerFactory;
            _controllerLocator = controllerLocator;
        }

        public ControllerInvokerResult Call<TController>(Expression<Func<TController, ActionResult>> action)
            where TController : IController
        {
            var instance = _controllerFactory.Create<TController>();

            var body = (MethodCallExpression)action.Body;
            var parameterValues = new List<object>();
            var route = BuildMethodCall<TController>(action, body, parameterValues);

            var result = (ActionResult)body.Method.Invoke(instance, parameterValues.ToArray());
            
            return new ControllerInvokerResult
                       {
                           Route = route,
                           Result = result
                       };
        }

        public async Task<ControllerInvokerResult> CallAsync<TController>(Expression<Func<TController, Task<ActionResult>>> action)
            where TController : IController
        {
            var instance = _controllerFactory.Create<TController>();

            var body = (MethodCallExpression)action.Body;
            var parameterValues = new List<object>();
            var route = BuildMethodCall<TController>(action, body, parameterValues);

            var result = await (Task<ActionResult>)body.Method.Invoke(instance, parameterValues.ToArray());

            return new ControllerInvokerResult
            {
                Route = route,
                Result = result
            };
        }

        private static string BuildMethodCall<TController>(LambdaExpression action, MethodCallExpression body, List<object> parameterValues)
            where TController : IController
        {
            var parameters = body.Method.GetParameters();
            var arguments = body.Arguments;
            for (var i = 0; i < parameters.Length && i < arguments.Count; i++)
            {
                var argument = arguments[i];
                var lambda = Expression.Lambda<Func<TController, object>>(Expression.Convert(argument, typeof (object)),
                                                                          action.Parameters.ToList());
                var compiled = lambda.Compile();
                var value = compiled(default(TController));

                parameterValues.Add(value);
            }

            var route = typeof (TController).Name.Replace("Controller", string.Empty) + "/" +
                      body.Method.Name;
            if (parameterValues.Any())
            {
                route += "?";
                for (var paramIndex = 0; paramIndex < parameters.Length; paramIndex++)
                {
                    if (paramIndex > 0)
                    {
                        route += "&";
                    }
                    route += parameters[paramIndex].Name + "=" + parameterValues[paramIndex];
                }
            }
            return route;
        }

        public async Task<ControllerInvokerResult> CallAsync(string route)
        {
            var controllerRoute = ParseRoute(route);

            var controller = _controllerLocator.Create(controllerRoute.ControllerName);
            var controllerType = controller.GetType().GetTypeInfo();

            var methodInfos = controllerType.GetDeclaredMethods(controllerRoute.ActionName).ToArray();

            if (methodInfos == null || methodInfos.Any() == false)
            {
                throw new InvalidOperationException(string.Format("Unable to locate action {0} on {1} controller.", controllerRoute.ActionName, controllerRoute.ControllerName));
            }

            var methodInfo = methodInfos[0];

            if (typeof(ActionResult).GetTypeInfo().IsAssignableFrom(methodInfo.ReturnType.GetTypeInfo()) == false &&
                typeof(Task<ActionResult>).GetTypeInfo().IsAssignableFrom(methodInfo.ReturnType.GetTypeInfo()) == false)
                throw new InvalidOperationException(string.Format("Controller action {0} must return an ActionResult or Task<ActionResult>.", controllerRoute.ActionName));

            var parameters = new List<object>();
            foreach (var methodParam in methodInfo.GetParameters())
            {
                // TODO: fix support for other types.
                if (methodParam.ParameterType == typeof (int))
                    parameters.Add(Convert.ToInt32(controllerRoute.Parameters[methodParam.Name]));
                else
                    parameters.Add(controllerRoute.Parameters[methodParam.Name]);
            }

            if (typeof (ActionResult).GetTypeInfo().IsAssignableFrom(methodInfo.ReturnType.GetTypeInfo()))
            {
                var result = methodInfo.Invoke(controller, parameters.ToArray());

                return new ControllerInvokerResult
                           {
                               Route = route,
                               Result = (ActionResult) result
                           };
            }

            var result1 = await ((Task<ActionResult>)methodInfo.Invoke(controller, parameters.ToArray()));
            return new ControllerInvokerResult
                       {
                           Route = route,
                           Result = result1
                       };
        }

        private ControllerRoute ParseRoute(string route)
        {
            var indexOfControllerActionSeparator = route.IndexOf('/');
            var controllerName = route.Substring(0, indexOfControllerActionSeparator);

            string actionName;

            var parametersIndex = route.IndexOf("?");
            var parameters = new Dictionary<string, string>();

            if (parametersIndex != -1)
            {
                actionName = route.Substring(indexOfControllerActionSeparator + 1, parametersIndex - (indexOfControllerActionSeparator + 1));
                var parametersString = route.Substring(parametersIndex + 1);
                var paramPairs = parametersString.Split('&');

                foreach (var paramPair in paramPairs)
                {
                    var splitPair = paramPair.Split('=');
                    parameters.Add(splitPair[0], splitPair[1]);
                }
            }
            else
            {
                actionName = route.Substring(indexOfControllerActionSeparator + 1);
            }

            var controller = _controllerLocator.Create(controllerName);
            var controllerType = controller.GetType().GetTypeInfo();

            var methodInfo = controllerType.GetDeclaredMethods(actionName);

            if (methodInfo == null || methodInfo.Any() == false)
            {
                throw new InvalidOperationException(string.Format("Unable to locate action {0} on {1} controller.", actionName, controllerName));
            }
            
            return new ControllerRoute
                       {
                           ControllerName = controllerName,
                           ActionName = actionName,
                           Parameters = parameters
                       };
        }

        private class ControllerRoute
        {
            public string ControllerName { get; set; }
            public string ActionName { get; set; }

            public Dictionary<string, string> Parameters { get; set; } 
        }
    }
}