using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BlowTrial.Infrastructure.Converters
{
    [ValueConversion(typeof(string), typeof(int?))]
    class StringToNullableIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var returnVal = value as int?;
            if (returnVal == null) { return null; }
            return returnVal.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (int.TryParse((string)value, out int returnVar))
            {
                return returnVar;
            }
            return null;

        }
    }
}
