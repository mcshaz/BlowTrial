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
        const double WeeksPerMonth = DaysPerMonth / 7;
        const double MaximumGestationalCorrection = 43;
        const double CeaseCorrectingDaysOfAge = DaysPerMonth * 24;
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
        public bool IsDataAvailable(double daysOfAge, bool isMale, double totalWeeksGestAtBirth = TermGestation)
        {
            return (isMale ? GestAgeRange.MaleRange : GestAgeRange.FemaleRange).Min <= (int)(totalWeeksGestAtBirth + daysOfAge / 7);
        }
        public double CumSnormForAge(double value, double daysOfAge, bool isMale, double totalWeeksGestAtBirth=TermGestation)
        {
            return LMSForAge(daysOfAge, isMale, totalWeeksGestAtBirth).CumNormalDistribution(value);
        }

        public double ZForAge(double value, double daysOfAge, bool isMale, double totalWeeksGestAtBirth=TermGestation)
        {
            return LMSForAge(daysOfAge, isMale, totalWeeksGestAtBirth).Zscore(value);
        }
        const double roundingFactor = 0.00001;
        public LMS LMSForAge(double daysOfAge, bool isMale, double totalWeeksGestAtBirth=TermGestation)
        {
            if (!IsDataAvailable(daysOfAge, isMale, totalWeeksGestAtBirth))
            {
                throw new ArgumentOutOfRangeException("totalWeeksGestAtBirth", totalWeeksGestAtBirth, string.Format("must be greater than {0} - check GestAgeRange property prior to calling", (isMale ? GestAgeRange.MaleRange : GestAgeRange.FemaleRange).Min));
            }
            if (totalWeeksGestAtBirth > MaximumGestationalCorrection)
            {
                totalWeeksGestAtBirth = MaximumGestationalCorrection;
            }
            if (daysOfAge < 0)
            {
                throw new ArgumentOutOfRangeException("daysOfAge", daysOfAge, "must be >= 0");
            }
            if (daysOfAge > CeaseCorrectingDaysOfAge) 
            {
                totalWeeksGestAtBirth = TermGestation;
            }
            double lookupTotalAge = daysOfAge/7 + totalWeeksGestAtBirth;
            int lookupAge = (int)(lookupTotalAge+roundingFactor);
            int maxVal = isMale?GestAgeRange.MaleRange.Max:GestAgeRange.FemaleRange.Max;
            if (lookupAge == maxVal)
            {
                int nextLookupAge = lookupAge + 1;
                return LMSForGestAge(lookupAge, isMale)
                    .LinearInterpolate(LMSForAgeWeeks(nextLookupAge - TermGestation, isMale), lookupTotalAge - (double)lookupAge);
            }
            if (lookupAge < maxVal)
            {
                int nextLookupAge = lookupAge + 1;
                return LMSForGestAge(lookupAge, isMale)
                    .LinearInterpolate(LMSForGestAge(nextLookupAge, isMale), lookupTotalAge - (double)lookupAge);
            }
            lookupTotalAge -= TermGestation;
            lookupAge = (int)(lookupTotalAge + roundingFactor);
            maxVal = isMale ? AgeWeeksRange.MaleRange.Max : AgeWeeksRange.FemaleRange.Max;
            if (lookupAge == maxVal)
            {
                double ageMonthsLookup = Math.Ceiling((daysOfAge + totalWeeksGestAtBirth - TermGestation) / DaysPerMonth);
                double fraction = (lookupTotalAge - (double)maxVal) / (ageMonthsLookup * WeeksPerMonth - (double)maxVal);
                return LMSForAgeWeeks(lookupAge, isMale)
                    .LinearInterpolate(LMSForAgeMonths((int)ageMonthsLookup, isMale), fraction);
            }
            if (lookupAge < maxVal)
            {
                int nextLookupAge = lookupAge + 1;
                return LMSForAgeWeeks(lookupAge, isMale)
                    .LinearInterpolate(LMSForAgeWeeks(nextLookupAge, isMale), lookupTotalAge - (double)lookupAge);
            }
            lookupTotalAge = (daysOfAge + totalWeeksGestAtBirth - TermGestation)/DaysPerMonth;
            lookupAge = (int)(lookupTotalAge + roundingFactor);
            maxVal = (isMale ? AgeMonthsRange.MaleRange.Max : AgeMonthsRange.FemaleRange.Max);
            if (lookupAge > maxVal) 
            {
                return LMSForAgeMonths(maxVal, isMale); 
            }
            int nextAge = lookupAge + 1;
            return LMSForAgeMonths(lookupAge, isMale)
                .LinearInterpolate(LMSForAgeMonths(nextAge, isMale), lookupTotalAge - (double)lookupAge);
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
