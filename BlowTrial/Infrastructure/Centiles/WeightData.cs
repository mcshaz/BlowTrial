using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Centiles
{
    public static class WeightData
    {
        public static double CumSnormWtForAge(double weightKg, TimeSpan age, double gestAgeInWeeksAtBirth, bool isMale)
        {
            return WtForAge(age, gestAgeInWeeksAtBirth, isMale).CumSnormfromParams(weightKg);
        }
        public static double CumSnormBirthWtForGestAge(int weightInGrams, int gestAgeWeeks, int gestAgeDays, bool isMale)
        {
            double weightKg = ((double)weightInGrams)/1000;
            double gestAgeCombined = (double)gestAgeWeeks + ((double)gestAgeDays)/7;
            return CumSnormWtForAge(weightKg, new TimeSpan(0), gestAgeCombined, isMale);
        }
        public static double ZwtForAge(double weightKg, TimeSpan age, double gestAgeInWeeksAtBirth, bool isMale)
        {
            return WtForAge(age, gestAgeInWeeksAtBirth, isMale).ZfromParams(weightKg);
        }
        /// <summary>
        /// Use linear intrapolation to get weight for gestational age
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="gestAge"></param>
        /// <param name="isMale"></param>
        /// <returns></returns>
        public static LMS WtForAge(TimeSpan age, double gestAgeAtBirth, bool isMale)
        {
            if (gestAgeAtBirth < WtForGestAgeMin || gestAgeAtBirth > WtForGestAgeMax)
            {
                throw new ArgumentOutOfRangeException("gestAgeAtBirth", gestAgeAtBirth ,string.Format(AgeOutOfRangeError,WtForGestAgeMin, WtForGestAgeMax));
            }
            double daysOfAge = age.TotalDays;
            if (daysOfAge < 0 || daysOfAge > maxDays_AgeMonths)
            {
                throw new ArgumentOutOfRangeException("age.Days", daysOfAge ,string.Format(AgeOutOfRangeError,0, maxDays_AgeMonths));
            }

            int lookupAge = (int)(daysOfAge/7 + gestAgeAtBirth);
            if (lookupAge == WtForGestAgeMax)
            {
                return WtForGestAge(lookupAge, isMale)
                    .LinearInterpolate(WtForAgeWeeks(++lookupAge-TermGestation, isMale), DaysToWeekFraction(daysOfAge));
            }
            if (lookupAge < WtForGestAgeMax)
            {
                return WtForGestAge(lookupAge, isMale)
                    .LinearInterpolate(WtForGestAge(++lookupAge, isMale), DaysToWeekFraction(daysOfAge));
            }
            lookupAge -= TermGestation; 
            if (lookupAge == WtForAgeWeeksMax)
            {
                double ageMonthsLookup = Math.Ceiling(daysOfAge/DaysPerMonth);
                double fraction = (daysOfAge - WtForAgeWeeksMaxInDays)/(ageMonthsLookup*DaysPerMonth - WtForAgeWeeksMaxInDays);
                return WtForAgeWeeks(lookupAge, isMale)
                    .LinearInterpolate(WtForAgeMonths((int)ageMonthsLookup, isMale), fraction);
            }
            if (lookupAge < WtForAgeWeeksMax)
            {
                return WtForAgeWeeks(lookupAge, isMale)
                    .LinearInterpolate(WtForAgeWeeks(++lookupAge, isMale), DaysToWeekFraction(daysOfAge));
            }
            lookupAge = (int)(daysOfAge/DaysPerMonth);
            return WtForAgeMonths(lookupAge, isMale)
                .LinearInterpolate(WtForAgeMonths(++lookupAge, isMale), DaysToMonthFraction(daysOfAge));
        }
        static double DaysToWeekFraction(double days)
        {
            double weeks = (days/7);
            return weeks - Math.Floor(weeks);
        }
        static double DaysToMonthFraction(double days)
        {
            double months = (days/DaysPerMonth);
            return months-Math.Floor(months);
        }
        public const int TermGestation = 40;
        public const int WtForGestAgeMin = 23;
        public const int WtForGestAgeMax = 43;
        public const int WtForAgeWeeksMin = 4;
        public const int WtForAgeWeeksMax = 13;
        public const int WtForAgeMonthsMin = 3;
        public const int WtForAgeMonthsMax = 12;
        const double DaysPerMonth = 365.25/12;

        const int maxDays_AgeMonths = (int)(DaysPerMonth*WtForAgeMonthsMax);
        const int WtForAgeWeeksMaxInDays = 7* WtForAgeWeeksMax;

        const string AgeOutOfRangeError = "Must be between {0} & {1}";
        public static LMS WtForGestAge(int gestAgeWeeks,bool isMale)
        {
            if (isMale)
            {
                switch(gestAgeWeeks)
                {
                    case 23:
                        return new LMS{L=1.147, M=0.6145, S=0.15875};
                    case 24:
                        return new LMS{L=1.126, M=0.7142, S=0.16249};
                    case 25:
                        return new LMS{L=1.104, M=0.8167, S=0.16628};
                    case 26:
                        return new LMS{L=1.083, M=0.9244, S=0.17007};
                    case 27:
                        return new LMS{L=1.061, M=1.0364, S=0.17355};
                    case 28:
                        return new LMS{L=1.04, M=1.1577, S=0.17663};
                    case 29:
                        return new LMS{L=1.018, M=1.2898, S=0.17905};
                    case 30:
                        return new LMS{L=0.997, M=1.436, S=0.18056};
                    case 31:
                        return new LMS{L=0.975, M=1.605, S=0.18092};
                    case 32:
                        return new LMS{L=0.954, M=1.7993, S=0.1798};
                    case 33:
                        return new LMS{L=0.932, M=2.0156, S=0.17703};
                    case 34:
                        return new LMS{L=0.911, M=2.2472, S=0.17262};
                    case 35:
                        return new LMS{L=0.889, M=2.486, S=0.16668};
                    case 36:
                        return new LMS{L=0.868, M=2.7257, S=0.15938};
                    case 37:
                        return new LMS{L=0.846, M=2.9594, S=0.15117};
                    case 38:
                        return new LMS{L=0.825, M=3.1778, S=0.14258};
                    case 39:
                        return new LMS{L=0.803, M=3.3769, S=0.13469};
                    case 40:
                        return new LMS{L=0.782, M=3.5551, S=0.12851};
                    case 41:
                        return new LMS{L=0.76, M=3.7172, S=0.12412};
                    case 42:
                        return new LMS{L=0.739, M=3.8702, S=0.12085};
                    case 43:
                        return new LMS{L=244.2, M=4.0603, S=138.07};
                    default:
                        throw new ArgumentOutOfRangeException("gestAge", gestAgeWeeks,string.Format(AgeOutOfRangeError,WtForGestAgeMin, WtForGestAgeMax));
                }
            }
            switch(gestAgeWeeks) //Female
            {
                case 23:
                    return new LMS{L=1.326, M=0.5589, S=0.17378};
                case 24:
                    return new LMS{L=1.278, M=0.6584, S=0.17716};
                case 25:
                    return new LMS{L=1.229, M=0.7611, S=0.18066};
                case 26:
                    return new LMS{L=1.181, M=0.8672, S=0.18429};
                case 27:
                    return new LMS{L=1.132, M=0.9775, S=0.18779};
                case 28:
                    return new LMS{L=1.084, M=1.0929, S=0.19058};
                case 29:
                    return new LMS{L=1.035, M=1.2166, S=0.19209};
                case 30:
                    return new LMS{L=0.987, M=1.3593, S=0.19212};
                case 31:
                    return new LMS{L=0.938, M=1.525, S=0.19052};
                case 32:
                    return new LMS{L=0.89, M=1.7118, S=0.1873};
                case 33:
                    return new LMS{L=0.841, M=1.9163, S=0.18261};
                case 34:
                    return new LMS{L=0.793, M=2.1342, S=0.17659};
                case 35:
                    return new LMS{L=0.744, M=2.3607, S=0.1694};
                case 36:
                    return new LMS{L=0.695, M=2.5903, S=0.16107};
                case 37:
                    return new LMS{L=0.647, M=2.8164, S=0.15165};
                case 38:
                    return new LMS{L=0.598, M=3.0334, S=0.14174};
                case 39:
                    return new LMS{L=0.55, M=3.2362, S=0.13249};
                case 40:
                    return new LMS{L=0.501, M=3.413, S=0.12481};
                case 41:
                    return new LMS{L=0.453, M=3.5539, S=0.11855};
                case 42:
                    return new LMS{L=0.404, M=3.6743, S=0.11308};
                case 43:
                    return new LMS{L=0.2024, M=3.8352, S=0.1406};
                default:
                    throw new ArgumentOutOfRangeException("gestAge", gestAgeWeeks,string.Format(AgeOutOfRangeError,WtForGestAgeMin, WtForGestAgeMax));

            }
        }
        public static LMS WtForAgeWeeks(int ageWeeks, bool isMale)
        {
            if (isMale)
            {
                switch (ageWeeks)
                {
                    case 4:
                        return new LMS { L = 233.1, M = 4.3671, S = 134.97 };
                    case 5:
                        return new LMS { L = 223.7, M = 4.659, S = 132.15 };
                    case 6:
                        return new LMS { L = 215.5, M = 4.9303, S = 129.6 };
                    case 7:
                        return new LMS { L = 208.1, M = 5.1817, S = 127.29 };
                    case 8:
                        return new LMS { L = 201.4, M = 5.4149, S = 125.2 };
                    case 9:
                        return new LMS { L = 195.2, M = 5.6319, S = 123.3 };
                    case 10:
                        return new LMS { L = 189.4, M = 5.8346, S = 121.57 };
                    case 11:
                        return new LMS { L = 184, M = 6.0242, S = 120.01 };
                    case 12:
                        return new LMS { L = 178.9, M = 6.2019, S = 118.6 };
                    case 13:
                        return new LMS { L = 174, M = 6.369, S = 117.32 };
                    default:
                        throw new ArgumentOutOfRangeException("ageWeeks", ageWeeks, string.Format(AgeOutOfRangeError,WtForAgeWeeksMin, WtForAgeWeeksMax));
                }
            }
            switch (ageWeeks) //Female
            {
                case 4:
                    return new LMS { L = 0.1789, M = 4.0987, S = 0.13805 };
                case 5:
                    return new LMS { L = 0.1582, M = 4.3476, S = 0.13583 };
                case 6:
                    return new LMS { L = 0.1395, M = 4.5793, S = 0.13392 };
                case 7:
                    return new LMS { L = 0.1224, M = 4.795, S = 0.13228 };
                case 8:
                    return new LMS { L = 0.1065, M = 4.9959, S = 0.13087 };
                case 9:
                    return new LMS { L = 0.0918, M = 5.1842, S = 0.12966 };
                case 10:
                    return new LMS { L = 0.0779, M = 5.3618, S = 0.12861 };
                case 11:
                    return new LMS { L = 0.0648, M = 5.5295, S = 0.1277 };
                case 12:
                    return new LMS { L = 0.0525, M = 5.6883, S = 0.12691 };
                case 13:
                    return new LMS { L = 0.0407, M = 5.8393, S = 0.12622 };
                default:
                    throw new ArgumentOutOfRangeException("ageWeeks", ageWeeks, string.Format(AgeOutOfRangeError,WtForAgeWeeksMin, WtForAgeWeeksMax));
            }
        }
        public static LMS WtForAgeMonths(int ageMonths, bool isMale)
        {
            if (isMale)
            {
                switch (ageMonths)
                {
                    case 3:
                        return new LMS { L = 173.8, M = 6.3762, S = 117.27 };
                    case 4:
                        return new LMS { L = 155.3, M = 7.0023, S = 113.16 };
                    case 5:
                        return new LMS { L = 139.5, M = 7.5105, S = 110.8 };
                    case 6:
                        return new LMS { L = 125.7, M = 7.934, S = 109.58 };
                    case 7:
                        return new LMS { L = 113.4, M = 8.297, S = 109.02 };
                    case 8:
                        return new LMS { L = 102.1, M = 8.6151, S = 108.82 };
                    case 9:
                        return new LMS { L = 91.7, M = 8.9014, S = 108.81 };
                    case 10:
                        return new LMS { L = 82, M = 9.1649, S = 108.91 };
                    case 11:
                        return new LMS { L = 73, M = 9.4122, S = 109.06 };
                    case 12:
                        return new LMS { L = 64.4, M = 9.6479, S = 109.25 };
                    default:
                        throw new ArgumentOutOfRangeException("ageMonths", ageMonths, string.Format(AgeOutOfRangeError, WtForAgeMonthsMin, WtForAgeMonthsMax));
                }
            }
            switch (ageMonths) //Female
            {
                case 3:
                    return new LMS { L = 0.0402, M = 5.8458, S = 0.12619 };
                case 4:
                    return new LMS { L = -0.005, M = 6.4237, S = 0.12402 };
                case 5:
                    return new LMS { L = -0.043, M = 6.8985, S = 0.12274 };
                case 6:
                    return new LMS { L = -0.0756, M = 7.297, S = 0.12204 };
                case 7:
                    return new LMS { L = -0.1039, M = 7.6422, S = 0.12178 };
                case 8:
                    return new LMS { L = -0.1288, M = 7.9487, S = 0.12181 };
                case 9:
                    return new LMS { L = -0.1507, M = 8.2254, S = 0.12199 };
                case 10:
                    return new LMS { L = -0.17, M = 8.48, S = 0.12223 };
                case 11:
                    return new LMS { L = -0.1872, M = 8.7192, S = 0.12247 };
                case 12:
                    return new LMS { L = -0.2024, M = 8.9481, S = 0.12268 };
                default:
                    throw new ArgumentOutOfRangeException("ageMonths", ageMonths, string.Format(AgeOutOfRangeError, WtForAgeMonthsMin, WtForAgeMonthsMax));

            }
        }
    }
}
