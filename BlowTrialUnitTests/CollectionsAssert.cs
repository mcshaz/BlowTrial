using BlowTrial.Domain.Outcomes;
using BlowTrial.Infrastructure.Randomising;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrialUnitTests
{
    static class MyAssert
    {
        public static void AreEqual(string expected, double actual)
        {
            double exp;
            if (!double.TryParse(expected, out exp))
            {
                throw new ArgumentException("Expected must be able to be parsed as a double");
            }
            int dpIndex = expected.IndexOf(".");
            int dp = (dpIndex>-1)
                ?(expected.Length - dpIndex -1)
                :0;
            double dif = (actual - exp)*Math.Pow(10, dp);
            if (dif >=1 || dif <-0.5)
            {
                throw new AssertFailedException(string.Format("values are not equal: expected a value of <{0}> but is <{1}>", expected, actual));
            }
        }
    }
    static class MyCollectionAssert
    {
        public static void AreEquivalent(BlockComponent expected, Counter<RandomisationArm> actual)
        { }
        public static void AreEquivalent<T>(IDictionary<T,int> expected, Counter<T> actual)
        {
            const string assertFailTemplate = "CollectionAssert.AreEquivalent Failed on element {0}: should have {1} occurences, but has {2}";
            foreach (var e in expected)
            {
                if (actual[e.Key] != e.Value)
                {
                    throw new AssertFailedException(string.Format(assertFailTemplate, e.Key, e.Value, actual[e.Key]));
                }
            }
            foreach (var a in actual)
            {
                if (!expected.ContainsKey(a.Key) && a.Value > 0) 
                {
                    throw new AssertFailedException(string.Format(assertFailTemplate, a.Key, 0, a.Value));
                }
            }
        }
        public static List<RandomisationArm> ToList(this BlockComponent block)
        {
            var returnVar = new List<RandomisationArm>();
            foreach (var r in block.Ratios)
            {
                int count = r.Value * block.Repeats;
                for (int i=0;i<count;i++)
                {
                    returnVar.Add(r.Key);
                }
            }
            return returnVar;
        }
    }
}
