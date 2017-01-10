using System;
using System.Globalization;
using System.Windows.Data;

namespace LuaInstaller.Converters
{
    public class VariableTargetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EnvironmentVariableTarget variableTarget = (EnvironmentVariableTarget)value;
            return variableTarget.ToString().Equals((string)parameter, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.Parse(typeof(EnvironmentVariableTarget), (string)parameter);
        }
    }
}
