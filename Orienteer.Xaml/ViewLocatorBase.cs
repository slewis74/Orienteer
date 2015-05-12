using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Orienteer.Xaml
{
    public abstract class ViewLocatorBase<TFrameworkElement> : IViewLocator<TFrameworkElement>
    {
        private readonly Dictionary<Type, Type> _cache;
        private Assembly _assembly;
        private string _baseViewModelNamespace;
        private string _baseViewNamespace;

        protected ViewLocatorBase()
        {
            _cache = new Dictionary<Type, Type>();
        }

        public void Configure(Assembly assembly, string baseViewModelNamespace, string baseViewNamespace)
        {
            _assembly = assembly;
            _baseViewModelNamespace = baseViewModelNamespace;
            _baseViewNamespace = baseViewNamespace;
        }

        public abstract TFrameworkElement Resolve(object viewModel);

        public Type DetermineViewType(Type viewModelType)
        {
            if (_cache.ContainsKey(viewModelType))
                return _cache[viewModelType];

            var vmTypeName = viewModelType.Name;
            var logicalTypeName = vmTypeName.Replace("ViewModel", string.Empty);
            string viewTypeName = logicalTypeName + "View";

            var exportedTypesInSameNamespace = _assembly.ExportedTypes
                .Where(t => t.Namespace == viewModelType.Namespace.Replace(_baseViewModelNamespace, _baseViewNamespace))
                .ToArray();                              

            var viewTypes = exportedTypesInSameNamespace.Where(t => t.Name == viewTypeName).ToArray();

            if (viewTypes.Any() == false)
            {
                throw new InvalidOperationException(string.Format("Unable to locate view for {0}", vmTypeName));
            }

            var determinedViewType = viewTypes.Single();
            _cache.Add(viewModelType, determinedViewType);
            return determinedViewType;
        }
    }
}