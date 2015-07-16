using BlowTrial.Properties;
using System;

namespace BlowTrial.Infrastructure.Extensions
{
    static class TimeSpanExtensions
    {
        internal static string AsNeonatalAgeString(this TimeSpan age)
        {
            int daysAge = age.Days;
            if (daysAge < 14)
            {
                return daysAge.ToString() + ' ' + Strings.Days;
            }
            return string.Format("{0}.{1} {2}",(daysAge/7),(daysAge%7),Strings.Weeks);
        }
    }
}
