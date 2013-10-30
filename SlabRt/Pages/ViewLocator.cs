using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;

namespace SlabRt.Pages
{
    public class ViewLocator : IViewLocator
    {
        private readonly Dictionary<ViewModelTypeOrientationKey, Type> _cache;

        public ViewLocator()
        {
            _cache = new Dictionary<ViewModelTypeOrientationKey, Type>();
        }

        public FrameworkElement Resolve(object viewModel, PageLayout pageLayout)
        {
            var view = (FrameworkElement)Activator.CreateInstance(DetermineViewType(viewModel.GetType(), pageLayout));
            view.DataContext = viewModel;
            return view;
        }

        private Type DetermineViewType(Type viewModelType, PageLayout pageLayout)
        {
            var key = new ViewModelTypeOrientationKey(viewModelType, pageLayout);

            if (_cache.ContainsKey(key))
                return _cache[key];

            var vmTypeName = viewModelType.Name;
            var logicalTypeName = vmTypeName.Replace("ViewModel", string.Empty);
            string viewTypeName;

            var exportedTypesInSameNamespace = viewModelType.GetTypeInfo().Assembly.ExportedTypes
                .Where(t => t.Namespace == viewModelType.Namespace)
                .ToArray();

            switch (pageLayout)
            {
                case PageLayout.Narrow:
                    viewTypeName = ConvertLogicalTypeToViewTypeName(logicalTypeName, "Narrow");
                    break;
                case PageLayout.Portrait:
                    viewTypeName = ConvertLogicalTypeToViewTypeName(logicalTypeName, "Portrait");
                    break;
                case PageLayout.Landscape:
                default:
                    viewTypeName = ConvertLogicalTypeToViewTypeName(logicalTypeName, "Landscape");
                    break;
            }
            viewTypeName += "View";

            var viewTypes = exportedTypesInSameNamespace.Where(t => t.Name == viewTypeName).ToArray();

            if (viewTypes.Any() == false)
            {
                if (pageLayout == PageLayout.Landscape)
                {
                    // Can't find a Landscape View, so try to fall back to the view without any orientation specifier
                    viewTypes = exportedTypesInSameNamespace.Where(t => t.Name == logicalTypeName + "View").ToArray();
                }

                if (viewTypes.Any() == false)
                {
                    throw new InvalidOperationException(string.Format("Unable to locate view for {0}", vmTypeName));
                }
            }
            if (viewTypes.Count() > 1)
            {
                throw new InvalidOperationException(string.Format("Ambiguous view list for {0}", vmTypeName));
            }

            var determinedViewType = viewTypes.First();
            _cache.Add(key, determinedViewType);
            return determinedViewType;
        }

        private static string ConvertLogicalTypeToViewTypeName(string logicalTypeName, string suffixToTrim)
        {
            return logicalTypeName.EndsWith(suffixToTrim) ? logicalTypeName : logicalTypeName + suffixToTrim;
        }

        public class ViewModelTypeOrientationKey
        {
            public ViewModelTypeOrientationKey(Type viewModelType, PageLayout pageLayout)
            {
                ViewModelType = viewModelType;
                PageLayout = pageLayout;
            }

            public Type ViewModelType { get; set; }
            public PageLayout PageLayout { get; set; }

            public override bool Equals(object obj)
            {
                var otherKey = obj as ViewModelTypeOrientationKey;
                if (otherKey == null)
                    return false;
                return otherKey.ViewModelType == ViewModelType &&
                       otherKey.PageLayout == PageLayout;
            }
            public override int GetHashCode()
            {
                return ViewModelType.GetHashCode() + PageLayout.GetHashCode();
            }
        }
    }
}