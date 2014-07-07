using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.Infrastructure.Randomising
{
    public enum RandomisationStrata
    {
        NotSet = 0,
        SmallestWeightMale = 1,
        SmallestWeightFemale = 2,
        MidWeightMale = 3,
        MidWeightFemale = 4,
        TopWeightMale = 5,
        TopWeightFemale = 6
    }
    internal static class RandomisingExtensions
    {
        internal static RandomisationStrata RandomisationCategory(this Participant p)
        {
            return RandomisationCategory(p.AdmissionWeight, p.IsMale);
        }

        internal static RandomisationStrata RandomisationCategory(double admissionWeight, bool isMale)
        {
            return admissionWeight < Engine.BlockWeight1
                ? isMale ? RandomisationStrata.SmallestWeightMale : RandomisationStrata.SmallestWeightFemale
                : admissionWeight >= Engine.BlockWeight2
                    ? isMale ? RandomisationStrata.TopWeightMale : RandomisationStrata.TopWeightFemale
                    : isMale ? RandomisationStrata.MidWeightMale : RandomisationStrata.MidWeightFemale;
        }

        internal static bool IsSameRandomisingCategory(bool oldIsMale, bool newIsMale, int oldWeight, int newWeight)
        {
            return oldIsMale == newIsMale &&
                    oldWeight < Engine.BlockWeight1 == newWeight < Engine.BlockWeight1 &&
                    oldWeight >= Engine.BlockWeight2 == newWeight >= Engine.BlockWeight2;
        }
    }
}
