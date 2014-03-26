using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Slab.WinStore.Pages.Converters
{
    /// <summary>
    /// Value converter that translates false to <see cref="Visibility.Visible"/> and true to
    /// <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public sealed class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && ((bool)value == false)) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Collapsed;
        }
    }
}