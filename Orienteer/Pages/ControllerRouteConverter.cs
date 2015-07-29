using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Orienteer.Pages.Navigation;

namespace Orienteer.Pages
{
    public class ControllerRouteConverter : IControllerRouteConverter
    {
        public RouteDescriptor GetRoute<TController>(Expression<Func<TController, ActionResult>> action)
            where TController : IController
        {
            var routeDescriptor = new RouteDescriptor();
            var body = (MethodCallExpression)action.Body;
            routeDescriptor.Route = BuildMethodCall<TController>(action, body, routeDescriptor.ParameterValues);

            return routeDescriptor;
        }

        public RouteDescriptor GetAsyncRoute<TController>(Expression<Func<TController, Task<ActionResult>>> action)
            where TController : IController
        {
            var routeDescriptor = new RouteDescriptor();
            var body = (MethodCallExpression)action.Body;
            routeDescriptor.Route = BuildMethodCall<TController>(action, body, routeDescriptor.ParameterValues);

            return routeDescriptor;
        }

        private static string BuildMethodCall<TController>(LambdaExpression action, MethodCallExpression body, IList<object> parameterValues)
            where TController : IController
        {
            var parameters = body.Method.GetParameters();
            var arguments = body.Arguments;
            for (var i = 0; i < parameters.Length && i < arguments.Count; i++)
            {
                var argument = arguments[i];
                var lambda = Expression.Lambda<Func<TController, object>>(Expression.Convert(argument, typeof(object)),
                                                                          action.Parameters.ToList());
                var compiled = lambda.Compile();
                var value = compiled(default(TController));

                if (parameterValues != null)
                parameterValues.Add(value);
            }

            var route = "/" + typeof(TController).Name.Replace("Controller", string.Empty) + "/" +
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
    }
}