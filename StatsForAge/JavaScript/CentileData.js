; (function () {
    window.dateConst = {
        daysPerYear: 365.25,
        msPerDay: 86400000 //24 * 60 * 60 * 1000
    };
    dateConst.daysPerMonth = dateConst.daysPerYear / 12;
    dateConst.weeksPerMonth = dateConst.daysPerMonth / 7;
    var tconst = {
        termGestation: 40,
        ceaseCorrectingDaysOfAge: dateConst.daysPerMonth * 24,
        roundingFactor: 0.00001,
        maximumGestationalCorrection: 43
    };
    window.GenderRange = function (ageMin, ageMax /*for males, then ageMin, ageMax for females */) {
        this.maleRange = new AgeDaysRange(ageMin, ageMax);
        if (arguments.length > 2) {
            this.femaleRange = new AgeDaysRange(arguments[2], arguments[3]);
        } else {
            this.femaleRange = new AgeDaysRange(ageMin, ageMax);
        }
    };

    window.AgeDaysRange = function (min, max) {
        if (min < 0) { throw new RangeError("min must be >=0"); }
        if (max < min) { throw new RangeError("max must be >= min"); }
        this.min = min;
        this.max = max;
    };

    window.Lms = function (l, m, s) {
        this.l = l;
        this.m = m;
        this.s = s;
    }

    window.Lms.prototype.linearInterpolate = function (interpolWith, fraction) {
        if (fraction < 0 || fraction > 1) {
            throw new Error("fraction must be between 0 and 1");
        }
        var oppFraction = 1 - fraction;
        return new Lms(
            oppFraction * this.l + fraction * interpolWith.l,
            oppFraction * this.m + fraction * interpolWith.m,
            oppFraction * this.s + fraction * interpolWith.s
        );
    }

    window.Lms.prototype.zFromParam = function (param) {
        if (this.l == 0) {
            return Math.log(param / this.m) / this.s;
        }
        return (Math.pow(param / this.m, this.l) - 1) / (this.l * this.s);
    }

    window.Lms.prototype.cumSnormfromParam = function (param) {
        return stat.cumSnorm(this.zFromParam(param));
    }
    window.stat = {};
    window.stat.cumSnorm = function(zScore) {
        var zAbs = Math.abs(zScore),
            returnVal,
            build;
        if (zAbs > 37) {
            return 0;
        } else {
            var Exponential = Math.exp(-Math.pow(zAbs, 2) / 2);
            if (zAbs < 7.07106781186547) {
                build = 3.52624965998911E-02 * zAbs + 0.700383064443688;
                build = build * zAbs + 6.37396220353165;
                build = build * zAbs + 33.912866078383;
                build = build * zAbs + 112.079291497871;
                build = build * zAbs + 221.213596169931;
                build = build * zAbs + 220.206867912376;
                returnVal = Exponential * build;
                build = 8.83883476483184E-02 * zAbs + 1.75566716318264;
                build = build * zAbs + 16.064177579207;
                build = build * zAbs + 86.7807322029461;
                build = build * zAbs + 296.564248779674;
                build = build * zAbs + 637.333633378831;
                build = build * zAbs + 793.826512519948;
                build = build * zAbs + 440.413735824752;
                returnVal = returnVal / build;
            }
            else {
                build = zAbs + 0.65;
                build = zAbs + 4 / build;
                build = zAbs + 3 / build;
                build = zAbs + 2 / build;
                build = zAbs + 1 / build;
                returnVal = Exponential / build / 2.506628274631;
            }
        }
        return (zScore < 0) ? returnVal : 1 - returnVal;
    }

    window.CentileData = function (argObj) {
        var that = this;
        this.gestAgeRange = argObj.gestAgeRange || new GenderRange(23, 43);
        this.ageWeeksRange = argObj.ageWeeksRange || new GenderRange(4, 13);
        this.ageMonthsRange = argObj.AgeMonthsRange || new GenderRange(3, 240);

        if (typeof (argObj.lMSForGestAge) != "function") { throw ("lMSForGestAge must be a supplied function"); }
        if (typeof (argObj.lMSForAgeWeeks) != "function") { throw ("lMSForAgeWeeks must be a supplied function"); }
        if (typeof (argObj.lMSForAgeMonths) != "function") { throw ("lMSForAgeMonths must be a supplied function"); }

        this.cumSnormForAge = function (value, daysOfAge, isMale, totalWeeksGestAtBirth) {
            return that.lMSForAge(daysOfAge, isMale, totalWeeksGestAtBirth).cumSnormfromParam(value);
        };

        this.zForAge = function (value, daysOfAge, isMale, totalWeeksGestAtBirth) {
            return that.lMSForAge(daysOfAge, isMale, totalWeeksGestAtBirth).zFromParam(value);
        };

        this.lMSForAge = function(daysOfAge, isMale, totalWeeksGestAtBirth) {
            var lookupTotalAge, lookupAge, maxVal, nextLookupAge, ageMonthsLookup, fraction;
            if (isMale && (totalWeeksGestAtBirth < that.gestAgeRange.maleRange.min) || 
                (!isMale && totalWeeksGestAtBirth < that.gestAgeRange.femaleRange.min)) {
                throw new RangeError("totalWeeksGestAtBirth must be greater than GestAgeRange - check property prior to calling");
            }
            totalWeeksGestAtBirth = totalWeeksGestAtBirth || tconst.termGestation;
            if (totalWeeksGestAtBirth > tconst.maximumGestationalCorrection)
            {
                totalWeeksGestAtBirth = tconst.maximumGestationalCorrection;
            }
            if (daysOfAge < 0)
            {
                throw new RangeError("daysOfAge", daysOfAge, "must be >= 0");
            }
            if (daysOfAge > tconst.ceaseCorrectingDaysOfAge) 
            {
                totalWeeksGestAtBirth = tconst.termGestation;
            }
            lookupTotalAge = daysOfAge/7 + totalWeeksGestAtBirth;
            lookupAge = parseInt(lookupTotalAge+tconst.roundingFactor);
            maxVal = isMale?that.gestAgeRange.maleRange.max:that.gestAgeRange.femaleRange.max;
            if (lookupAge == maxVal)
            {
                nextLookupAge = lookupAge + 1;
                return argObj.lMSForGestAge(lookupAge, isMale).linearInterpolate(argObj.lMSForAgeWeeks(nextLookupAge - tconst.termGestation, isMale), lookupTotalAge - lookupAge);
            }
            if (lookupAge < maxVal)
            {
                nextLookupAge = lookupAge + 1;
                return argObj.lMSForGestAge(lookupAge, isMale).linearInterpolate(
                    argObj.lMSForGestAge(nextLookupAge, isMale),
                    lookupTotalAge - lookupAge);
            }
            lookupTotalAge -= tconst.termGestation;
            lookupAge = parseInt(lookupTotalAge + tconst.roundingFactor);
            maxVal = isMale ? that.ageWeeksRange.maleRange.max : that.ageWeeksRange.femaleRange.max;
            if (lookupAge == maxVal)
            {
                ageMonthsLookup = Math.ceil((daysOfAge + totalWeeksGestAtBirth - tconst.termGestation) / dateConst.daysPerMonth);
                fraction = (lookupTotalAge - maxVal) / (ageMonthsLookup * dateConst.weeksPerMonth - maxVal);
                return argObj.lMSForAgeWeeks(lookupAge, isMale).linearInterpolate(
                    argObj.lMSForAgeMonths(ageMonthsLookup, isMale),
                    fraction);
            }
            if (lookupAge < maxVal)
            {
                nextLookupAge = lookupAge + 1;
                return argObj.lMSForAgeWeeks(lookupAge, isMale).linearInterpolate(
                    argObj.lMSForAgeWeeks(nextLookupAge, isMale), 
                    lookupTotalAge - lookupAge);
            }
            lookupTotalAge = (daysOfAge + totalWeeksGestAtBirth - tconst.termGestation) / dateConst.daysPerMonth;
            lookupAge = parseInt(lookupTotalAge + tconst.roundingFactor);
            maxVal = (isMale ? that.ageMonthsRange.maleRange.max : that.ageMonthsRange.femaleRange.max);
            if (lookupAge > maxVal) 
            {
                return argObj.lMSForAgeMonths(maxVal, isMale); 
            }
            nextLookupAge = lookupAge + 1;
            return argObj.lMSForAgeMonths(lookupAge, isMale).linearInterpolate(
			    argObj.lMSForAgeMonths(nextLookupAge, isMale),
			    lookupTotalAge - lookupAge);
        };
    }

    /*
    function any(arr, func) {
        var i = 0;
        for (; i < arr.length; i++) {
            if (func(arr[i], i)) {
                return true;
            }
        }
        return false;
    }
    */
})();
