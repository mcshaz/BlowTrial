using BlowTrial.Properties;
using System;
using System.Windows.Data;

namespace BlowTrial.Infrastructure.Converters
{
    [ValueConversion(typeof(TimeSpan?), typeof(string))]
    public class TimeSpanToWeeksConverter : IValueConverter
    {
        #region IValueConverter Members


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ts = value as TimeSpan?;
            if (!ts.HasValue) { return string.Empty; }
            int days = ts.Value.Days;
            const int daysperweek = 7;
            if (days < daysperweek) { return days + ' ' + Strings.Days; } 
            int weeks = days / daysperweek;
            days = days - (weeks * daysperweek);
            return weeks.ToString() + ' ' + Strings.Weeks + ' ' + Strings.And + ' ' + days.ToString() + ' ' + Strings.Days;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
