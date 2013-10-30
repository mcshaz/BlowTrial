using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BlowTrial.Infrastructure.Converters
{
    public class NullableBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true.Equals(value)? true : Binding.DoNothing;
        }

        #endregion
    }
}
