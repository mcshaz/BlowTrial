using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatsForAge
{
    public abstract class CentileData
    {
        #region fields
        GenderRange _gestAgeRange;
        GenderRange _ageWeeksRange;
        GenderRange _ageMonthsRange;
        #endregion

        #region constructor
        #endregion

        #region const
        public const int TermGestation = 40;
        public const double DaysPerMonth = 365.25 / 12;
        const double maximumGestationalCorrection = 43;
        #endregion

        public GenderRange GestAgeRange
        {
            get { return _gestAgeRange ?? (_gestAgeRange = new GenderRange(23, 43)); }
            protected set { _gestAgeRange = value; }
        }
        public GenderRange AgeWeeksRange
        {
            get { return _ageWeeksRange ?? (_ageWeeksRange = new GenderRange(4, 13)); }
            protected set { _ageWeeksRange = value; }
        }
        public GenderRange AgeMonthsRange
        {
            get { return _ageMonthsRange ?? (_ageMonthsRange = new GenderRange(3, 240)); }
            protected set { _ageMonthsRange = value; }
        }


        #region virtual methods
        protected abstract LMS LMSForGestAge(int gestAgeWeeks, bool isMale);
        protected abstract LMS LMSForAgeWeeks(int ageWeeks, bool isMale);
        protected abstract LMS LMSForAgeMonths(int ageMonths, bool isMale);
        #endregion

        #region methods
        public double CumSnormForAge(double value, int daysOfAge, bool isMale, int gestAgeInWeeksAtBirth = TermGestation, int gestAgeInDaysAtBirth = 0)
        {
            double totalWeeksGestation = (double)gestAgeInWeeksAtBirth + (double)gestAgeInDaysAtBirth/7;
            return CumSnormForAge(value, daysOfAge, isMale, totalWeeksGestation);
        }
        double CumSnormForAge(double value, int daysOfAge, bool isMale, double gestAgeInWeeksAtBirth)
        {
            return LMSForAge(daysOfAge, isMale, gestAgeInWeeksAtBirth).CumSnormfromParams(value);
        }

        double ZForAge(double value, int daysOfAge, bool isMale, int gestAgeInWeeksAtBirth = TermGestation, int gestAgeInDaysAtBirth = 0)
        {
            double totalWeeksGestation = (double)gestAgeInWeeksAtBirth + (double)gestAgeInDaysAtBirth / 7;
            return ZForAge(value, daysOfAge, isMale, totalWeeksGestation);
        }
        double ZForAge(double value, int daysOfAge, bool isMale, double gestAgeInWeeksAtBirth)
        {
            return LMSForAge(daysOfAge, isMale, gestAgeInWeeksAtBirth).ZfromParams(value);
        }
        LMS LMSForAge(int daysOfAge, bool isMale, double gestAgeAtBirth)
        {
            if (isMale && (gestAgeAtBirth < GestAgeRange.MaleRange.Min) || 
                (!isMale && gestAgeAtBirth < GestAgeRange.FemaleRange.Min))
            {
                throw new ArgumentOutOfRangeException("gestAgeAtBirth", gestAgeAtBirth, string.Format("must be greater than {0}", (isMale ? GestAgeRange.MaleRange : GestAgeRange.FemaleRange).Min));
            }
            if (gestAgeAtBirth > maximumGestationalCorrection)
            {
                gestAgeAtBirth = maximumGestationalCorrection;
            }
            if (daysOfAge < 0)
            {
                throw new ArgumentOutOfRangeException("daysOfAge", daysOfAge, "must be >= 0");
            }

            int lookupAge = (int)(daysOfAge / 7 + gestAgeAtBirth);
            int maxVal = isMale?GestAgeRange.MaleRange.Max:GestAgeRange.FemaleRange.Max;
            if (lookupAge == maxVal)
            {
                return LMSForGestAge(lookupAge, isMale)
                    .LinearInterpolate(LMSForAgeWeeks(++lookupAge - TermGestation, isMale), DaysToWeekFraction(daysOfAge));
            }
            if (lookupAge < maxVal)
            {
                return LMSForGestAge(lookupAge, isMale)
                    .LinearInterpolate(LMSForGestAge(++lookupAge, isMale), DaysToWeekFraction(daysOfAge));
            }
            lookupAge -= TermGestation;
            maxVal = isMale ? AgeWeeksRange.MaleRange.Max : AgeWeeksRange.FemaleRange.Max;
            if (lookupAge == maxVal)
            {
                double ageMonthsLookup = Math.Ceiling(daysOfAge / DaysPerMonth);
                double weeksMaxInDays = (double)maxVal * 7;
                double fraction = ((double)daysOfAge - weeksMaxInDays) / (ageMonthsLookup * DaysPerMonth - weeksMaxInDays);
                return LMSForAgeWeeks(lookupAge, isMale)
                    .LinearInterpolate(LMSForAgeMonths((int)ageMonthsLookup, isMale), fraction);
            }
            if (lookupAge < maxVal)
            {
                return LMSForAgeWeeks(lookupAge, isMale)
                    .LinearInterpolate(LMSForAgeWeeks(++lookupAge, isMale), DaysToWeekFraction(daysOfAge));
            }
            lookupAge = (int)(daysOfAge / DaysPerMonth);
            maxVal = (isMale ? AgeMonthsRange.MaleRange.Max : AgeMonthsRange.FemaleRange.Max);
            if (lookupAge > maxVal) 
            {
                return LMSForAgeMonths(maxVal, isMale); 
            }
            return LMSForAgeMonths(lookupAge, isMale)
                .LinearInterpolate(LMSForAgeMonths(++lookupAge, isMale), DaysToMonthFraction(daysOfAge));
        }
        static double DaysToWeekFraction(double days)
        {
            double weeks = (days / 7);
            return weeks - Math.Floor(weeks);
        }
        static double DaysToMonthFraction(double days)
        {
            double months = (days / DaysPerMonth);
            return months - Math.Floor(months);
        }

        #endregion
    }
    public class GenderRange
    {
        public GenderRange() { }
        public GenderRange(int ageMin, int ageMax)
        {
            MaleRange = new AgeRange(ageMin, ageMax);
            FemaleRange = new AgeRange(ageMin, ageMax);
        }
        public AgeRange MaleRange { get; set; }
        public AgeRange FemaleRange { get; set; }
    }
    public class AgeRange
    {
        public AgeRange(int min, int max)
        {
            if (min < 0) { throw new ArgumentOutOfRangeException("min", min, "must be >=0"); }
            if (max < min) { throw new ArgumentOutOfRangeException("max", max, "must be >= min"); }
            Min = min;
            Max = max;
        }
        public int Min { get; private set; }
        public int Max { get; private set; }
    }
}
