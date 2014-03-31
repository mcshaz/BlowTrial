using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BlowTrial.Infrastructure.Converters
{
    public class GenericNullableValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }
        public T NullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (true.Equals(value))
            {
                return TrueValue;
            }
            if (false.Equals(value))
            {
                return FalseValue;
            }
            return NullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // one way binding only
        }
    }
    public class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return FalseValue;
            else
                return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null ? value.Equals(TrueValue) : false;
        }
    }

    public class NullableBoolToStyleConverter : GenericNullableValueConverter<Style> { }
    public class BoolToWizardPageConverter : GenericNullableValueConverter<Xceed.Wpf.Toolkit.WizardPage> { }
    public class BoolToStringConverter : BoolToValueConverter<string> { }
}
