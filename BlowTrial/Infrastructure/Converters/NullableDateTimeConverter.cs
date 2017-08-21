using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BlowTrial.Infrastructure.Converters
{
    [ValueConversion(typeof(DateTime?), typeof(string))]
    public class NullableDateTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dt = value as DateTime?;
            if (value == null || parameter == null) { return dt.ToString(); }
            return dt.Value.ToString((string)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (DateTime.TryParse((string)value, out DateTime dt))
            {
                return dt;
            }
            return null;
        }

        #endregion
    }
}
