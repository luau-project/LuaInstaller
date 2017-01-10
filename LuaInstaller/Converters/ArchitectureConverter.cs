using LuaInstaller.Core;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LuaInstaller.Converters
{
    public class ArchitectureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Architecture arch = (Architecture)value;
            return arch.ToString().Equals((string)parameter, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.Parse(typeof(Architecture), ((string)parameter).ToUpperInvariant());
        }
    }
}
