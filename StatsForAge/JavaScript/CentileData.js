
    function CentileData(argObj) {
        var termGestation = 40,
            daysPerMonth = 365.25 / 12,
            weeksPerMonth = DaysPerMonth / 7,
            maximumGestationalCorrection = 43,
            ceaseCorrectingDaysOfAge = DaysPerMonth * 24,
            roundingFactor = 0.00001;

        this.gestAgeRange = argObj.gestAgeRange || new GenderRange(23, 43);
        this.ageWeeksRange = argObj.ageWeeksRange || new GenderRange(4, 13); 
        this.ageMonthsRange = argObj.AgeMonthsRange || new GenderRange(3, 240);

        if (typeof (argObj.lMSForGestAge) != "function") { throw ("lMSForGestAge must be a supplied function"); }
        if (typeof (argObj.lMSForAgeWeeks) != "function") { throw ("lMSForAgeWeeks must be a supplied function"); }
        if (typeof (argObj.lMSForAgeMonths) != "function") { throw ("lMSForAgeMonths must be a supplied function"); }

        this.cumSnormForAge = function (value, daysOfAge, isMale, totalWeeksGestAtBirth) {
            return LMSForAge(daysOfAge, isMale, totalWeeksGestAtBirth).CumSnormfromParams(value);
        };

        this.zForAge = function (value, daysOfAge, isMale, totalWeeksGestAtBirth) {
            return LMSForAge(daysOfAge, isMale, totalWeeksGestAtBirth).ZfromParams(value);
        };

        this.LMSForAge = function(daysOfAge, isMale, totalWeeksGestAtBirth) {
            var lookupTotalAge, lookupAge, maxVal, nextLookupAge, ageMonthsLookup, fraction;
            if (isMale && (totalWeeksGestAtBirth < GestAgeRange.MaleRange.Min) || 
                (!isMale && totalWeeksGestAtBirth < GestAgeRange.FemaleRange.Min)) {
                throw ("totalWeeksGestAtBirth must be greater than GestAgeRange - check property prior to calling");
            }
            totalWeeksGestAtBirth = totalWeeksGestAtBirth || termGestation;
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
            lookupTotalAge = daysOfAge/7 + totalWeeksGestAtBirth;
            lookupAge = parseInt(lookupTotalAge+roundingFactor);
            maxVal = isMale?GestAgeRange.MaleRange.Max:GestAgeRange.FemaleRange.Max;
            if (lookupAge == maxVal)
            {
                nextLookupAge = lookupAge + 1;
                return linearInterpolate(
                    argObj.lMSForGestAge(lookupAge, isMale),
                    argObj.lMSForAgeWeeks(nextLookupAge - TermGestation, isMale),
                    lookupTotalAge - lookupAge);
            }
            if (lookupAge < maxVal)
            {
                nextLookupAge = lookupAge + 1;
                return linearInterpolate(
                    argObj.lMSForGestAge(lookupAge, isMale),
                    argObj.lMSForGestAge(nextLookupAge, isMale),
                    lookupTotalAge - lookupAge);
            }
            lookupTotalAge -= TermGestation;
            lookupAge = parseInt(lookupTotalAge + roundingFactor);
            maxVal = isMale ? AgeWeeksRange.MaleRange.Max : AgeWeeksRange.FemaleRange.Max;
            if (lookupAge == maxVal)
            {
                ageMonthsLookup = Math.Ceiling((daysOfAge + totalWeeksGestAtBirth - TermGestation) / DaysPerMonth);
                fraction = (lookupTotalAge - maxVal) / (ageMonthsLookup * WeeksPerMonth - maxVal);
                return linearInterpolate(
                    argObj.lMSForAgeWeeks(lookupAge, isMale),
                    argObj.lMSForAgeMonths(ageMonthsLookup, isMale),
                    fraction);
            }
            if (lookupAge < maxVal)
            {
                nextLookupAge = lookupAge + 1;
                return linearInterpolate(
                    lMSForAgeWeeks(lookupAge, isMale),
                    argObj.lMSForAgeWeeks(nextLookupAge, isMale), 
                    lookupTotalAge - lookupAge);
            }
            lookupTotalAge = (daysOfAge + totalWeeksGestAtBirth - TermGestation)/DaysPerMonth;
            lookupAge = parseInt(lookupTotalAge + roundingFactor);
            maxVal = (isMale ? AgeMonthsRange.MaleRange.Max : AgeMonthsRange.FemaleRange.Max);
            if (lookupAge > maxVal) 
            {
                return argObj.lMSForAgeMonths(maxVal, isMale); 
            }
            nextLookupAge = lookupAge + 1;
            return linearInterpolate(
                argObj.lMSForAgeMonths(lookupAge, isMale),
                argObj.lMSForAgeMonths(nextLookupAge, isMale),
                lookupTotalAge - lookupAge);
        };
    }

    function GenderRange(ageMin, ageMax /*for males, then ageMin, ageMax for females */) {
        this.maleRange = new AgeRange(ageMin, ageMax);
        if (arguments.length > 2) {
            this.femaleRange = new AgeRange(arguments[2], arguments[3]);
        } else {
            this.femaleRange = new AgeRange(ageMin, ageMax);
        }
    };

    function AgeRange(min, max) {
        if (min < 0) { throw ("min must be >=0"); }
        if (max < min) { throw ("max must be >= min"); }
        this.min = min;
        this.max = max;
    };

    function Lms(l, m, s) {
        this.l = l;
        this.m = m;
        this.s = s;
    }

    function linearInterpolate(start, interpolWith, fraction) {
        if (fraction < 0 || fraction > 1) {
            throw ("fraction must be between 0 and 1");
        }
        var oppFraction = 1 - fraction;
        return new Lms(
            oppFraction * start.l + fraction * interpolWith.l,
            oppFraction * start.m + fraction * interpolWith.m,
            oppFraction * start.s + fraction * interpolWith.s
        );
    }