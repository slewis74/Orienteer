using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace SlabRt.Pages
{
    public class ViewLocator : IViewLocator
    {
        private Dictionary<ViewModelTypeOrientationKey, Type> _cache;

        public ViewLocator()
        {
            _cache = new Dictionary<ViewModelTypeOrientationKey, Type>();
        }

        public FrameworkElement Resolve(object viewModel, ApplicationViewState applicationViewState)
        {
            var view = (FrameworkElement)Activator.CreateInstance(DetermineViewType(viewModel.GetType(), applicationViewState));
            view.DataContext = viewModel;
            return view;
        }

        private Type DetermineViewType(Type viewModelType, ApplicationViewState applicationViewState)
        {
            var key = new ViewModelTypeOrientationKey(viewModelType, applicationViewState);

            if (_cache.ContainsKey(key))
                return _cache[key];

            var vmTypeName = viewModelType.Name;
            var logicalTypeName = vmTypeName.Replace("ViewModel", string.Empty);
            string viewTypeName;

            var exportedTypesInSameNamespace = viewModelType.GetTypeInfo().Assembly.ExportedTypes
                .Where(t => t.Namespace == viewModelType.Namespace)
                .ToArray();

            switch (applicationViewState)
            {
                case ApplicationViewState.Filled:
                    viewTypeName = ConvertLogicalTypeToViewTypeName(logicalTypeName, "Filled");
                    break;
                case ApplicationViewState.Snapped:
                    viewTypeName = ConvertLogicalTypeToViewTypeName(logicalTypeName, "Snapped");
                    break;
                case ApplicationViewState.FullScreenPortrait:
                    viewTypeName = ConvertLogicalTypeToViewTypeName(logicalTypeName, "Portrait");
                    break;
                case ApplicationViewState.FullScreenLandscape:
                default:
                    viewTypeName = ConvertLogicalTypeToViewTypeName(logicalTypeName, "Landscape");
                    break;
            }
            viewTypeName += "View";

            var viewTypes = exportedTypesInSameNamespace.Where(t => t.Name == viewTypeName).ToArray();

            if (viewTypes.Any() == false)
            {
                if (applicationViewState == ApplicationViewState.Filled)
                {
                    // Can't find a Filled View, so try to fall back to the Lanscape view
                    return DetermineViewType(viewModelType, ApplicationViewState.FullScreenLandscape);
                }
                if (applicationViewState == ApplicationViewState.FullScreenLandscape)
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
            public ViewModelTypeOrientationKey(Type viewModelType, ApplicationViewState applicationViewState)
            {
                ViewModelType = viewModelType;
                ApplicationViewState = applicationViewState;
            }

            public Type ViewModelType { get; set; }
            public ApplicationViewState ApplicationViewState { get; set; }

            public override bool Equals(object obj)
            {
                var otherKey = obj as ViewModelTypeOrientationKey;
                if (otherKey == null)
                    return false;
                return otherKey.ViewModelType == ViewModelType &&
                       otherKey.ApplicationViewState == ApplicationViewState;
            }
            public override int GetHashCode()
            {
                return ViewModelType.GetHashCode() + ApplicationViewState.GetHashCode();
            }
        }
    }
}