using System;
using System.Windows.Data;
using BlowTrial.Infrastructure.Extensions;

namespace BlowTrial.Infrastructure.Converters
{
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumToSeperatedWordsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return value;
            }
            return value.ToString().ToSeparatedWords();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
