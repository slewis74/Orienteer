using System;
using Autofac;
using Orienteer.Xaml;

namespace Orienteer.Autofac
{
    public class AutofacViewFactory<TFrameworkElement> : IViewFactory<TFrameworkElement>
    {
        private readonly IComponentContext _componentContext;

        public AutofacViewFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public TFrameworkElement Resolve(Type viewType)
        {
            return (TFrameworkElement)_componentContext.Resolve(viewType);
        }
    }
}