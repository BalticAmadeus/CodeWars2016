using System;
using System.Globalization;
using System.Windows.Data;

namespace Game.AdminClient.Converters
{
    public class NullableToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var nullable = value as Guid?;
            if (nullable == null)
                return string.Empty;

            return "In game";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
