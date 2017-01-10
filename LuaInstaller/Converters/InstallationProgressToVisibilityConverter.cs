using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LuaInstaller.Converters
{
    public class InstallationProgressToVisibilityConverter : IValueConverter
    {
        private readonly InstallationProgressToIntConverter converter;

        public InstallationProgressToVisibilityConverter()
        {
            converter = new InstallationProgressToIntConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)converter.Convert(value, targetType, parameter, culture)) == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
