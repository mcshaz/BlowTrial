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
    public enum RandomisationCategories
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
        internal static RandomisationCategories RandomisationCategory(Participant p)
        {
            return p.AdmissionWeight < RandomisingEngine.BlockWeight1
                ? p.IsMale ? RandomisationCategories.SmallestWeightMale : RandomisationCategories.SmallestWeightFemale
                : p.AdmissionWeight >= RandomisingEngine.BlockWeight2
                    ? p.IsMale ? RandomisationCategories.TopWeightMale : RandomisationCategories.TopWeightFemale
                    : p.IsMale ? RandomisationCategories.MidWeightMale : RandomisationCategories.MidWeightFemale;
        }

        internal static bool IsSameRandomisingCategory(bool oldIsMale, bool newIsMale, int oldWeight, int newWeight)
        {
            return oldIsMale == newIsMale &&
                    oldWeight < RandomisingEngine.BlockWeight1 == newWeight < RandomisingEngine.BlockWeight1 &&
                    oldWeight >= RandomisingEngine.BlockWeight2 == newWeight >= RandomisingEngine.BlockWeight2;
        }
    }
}
