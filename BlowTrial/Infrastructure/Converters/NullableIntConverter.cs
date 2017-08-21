using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BlowTrial.Infrastructure.Converters
{
    [ValueConversion(typeof(int?), typeof(string))]
    public class NullableIntConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var intVal = value as int?;
            if (value == null || parameter == null) { return intVal.ToString(); }
            return intVal.Value.ToString((string)parameter);
        }

        string _numberGroupSeperator;
        string NumberGroupSeperator
        {
            get { return _numberGroupSeperator ?? (_numberGroupSeperator = System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberGroupSeparator); }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (int.TryParse(((string)value).Replace(NumberGroupSeperator, ""), out int intVal))
            {
                return intVal;
            }
            return null;
        }

        #endregion
    }
}
