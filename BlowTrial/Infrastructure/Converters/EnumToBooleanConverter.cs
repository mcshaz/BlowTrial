using BlowTrial.Domain.Outcomes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BlowTrial.Infrastructure.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }
    public class EnumAnyMatchToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return null;
            }

            string checkValue = value.ToString();
            string[] targetValues = parameter.ToString().Split(new char[] { ',' });
            return targetValues.Any(t => checkValue.Equals(t,
                     StringComparison.InvariantCultureIgnoreCase));
        }

        public object ConvertBack(object value, Type targetType,
                          object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
