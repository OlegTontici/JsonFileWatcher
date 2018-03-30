using System;
using System.Globalization;
using System.Windows.Data;

namespace JsonFileWatcher.Converters
{
    public class ObjectToFormatedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value.GetType() == typeof(string))
            {
                return $"\"{value}\"";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
