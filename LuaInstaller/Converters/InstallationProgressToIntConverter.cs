using LuaInstaller.Core;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LuaInstaller.Converters
{
    public class InstallationProgressToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int current = (int)((InstallationProgress)value);
            int last = (int)(InstallationProgress.Finished);
            return current % last;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
