using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BlowTrial.Infrastructure.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolInverterConverter : IValueConverter
    {
        #region IValueConverter Members


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true.Equals(value) ? false : Binding.DoNothing;
        }

        #endregion
    }
}
