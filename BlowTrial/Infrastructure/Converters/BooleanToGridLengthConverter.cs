using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace BlowTrial.Infrastructure.Converters 
{
    [ValueConversion(typeof(bool), typeof(GridLength))]
    class BooleanToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null || parameter == null) return Binding.DoNothing;

            bool shouldCollapse;
            Boolean.TryParse(value.ToString(), out shouldCollapse);
            return shouldCollapse 
                ? new GridLength(0) 
                : (GridLength) parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        { throw new NotImplementedException(); }
    }
}
