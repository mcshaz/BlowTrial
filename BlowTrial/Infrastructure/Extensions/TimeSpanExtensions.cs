using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Extensions
{
    static class TimeSpanExtensions
    {
        internal static string AsNeonatalAgeString(this TimeSpan age)
        {
            int daysAge = age.Days;
            if (daysAge < 14)
            {
                return daysAge.ToString() + ' ' + Strings.DaysOld;
            }
            return string.Format("{0}.{1} {2}",(daysAge/7),(daysAge%7),Strings.WeeksOld);
        }
    }
}
