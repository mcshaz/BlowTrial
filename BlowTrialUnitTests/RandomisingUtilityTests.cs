using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlowTrial.Infrastructure.Randomising;
using BlowTrial.Domain.Tables;
using System.Collections.Generic;
using System.Linq;
using BlowTrial.Domain.Outcomes;
using Moq;
using MathNet.Numerics;

namespace BlowTrialUnitTests
{
    [TestClass]
    public class RandomisingUtilityTests
    {
        [TestMethod]
        public void TestGetRatio()
        {
            GetRatio();
        }
        public IEnumerable<BlockComponent> GetRatio()
        {
            return (from a in Enum.GetValues(typeof(AllocationGroups)).Cast<AllocationGroups>()
                    where a != AllocationGroups.NotApplicable
                    select ArmData.GetRatio(a)).ToList();
        }
        [TestMethod]
        public void TestCumulativeP()
        {
            var emptyBlock = new RandomisationArm[0];
            double d = 0;
            var moqRand = new Mock<IRandom>(MockBehavior.Strict);
            moqRand.Setup(m => m.NextDouble()).Returns(()=>d);
            foreach (var r in GetRatio())
            {
                var counter = new Counter<RandomisationArm>();
                double blockSize = r.TotalBlockSize();
                double increment = 1 / (blockSize);
                for (d=increment/2;d<1;d+=increment)
                {
                    var nextAlloc = BlockRandomisation.NextAllocation(emptyBlock, r, moqRand.Object);
                    counter[nextAlloc]++;
                }
                MyCollectionAssert.AreEquivalent(r.GetAllocations(), counter);
            }
        }
        [TestMethod]
        public void TestRandomisingAlgorithm()
        {
            var moqRand = new Mock<IRandom>(MockBehavior.Strict);
            var rnd = new Random();
            moqRand.Setup(m => m.NextDouble()).Returns(rnd.NextDouble);
            int minAllocations = int.MaxValue;
            foreach (var r in GetRatio())
            {
                const int maxRptIncr = 3;
                const int sameBlockRpt = 1000;
                for (int incr=0;incr<maxRptIncr;incr++)
                {
                    var allArms = new List<List<RandomisationArm>>(sameBlockRpt);
                    var rClone = r.Clone((byte)(r.Repeats + incr));
                    int allocations = rClone.TotalBlockSize();
                    if (allocations < minAllocations) { minAllocations = allocations; }
                    foreach (var dummy in Enumerable.Repeat(0, sameBlockRpt))
                    {
                        var arms = new List<RandomisationArm>(allocations);
                        while (arms.Count < allocations)
                        {
                            var newArm = BlockRandomisation.NextAllocation(arms, rClone, moqRand.Object);
                            arms.Add(newArm);
                        }
                        CollectionAssert.AreEquivalent(rClone.ToList(), arms);

                        allArms.Add(arms);
                    }
                    //keep this simple 2 arms, block size 4 - total permutations = 2^2, block size 6 = 6*5*4
                    //block 3 arms, block size 6 - permutations = 6*5 + 4*3
                    //blokk 3 arms, 2:1:1 size 8 = 8*7*6*5 + 4*3
                    //
                    
                    var ocurrences = new Counter<IList<RandomisationArm>>(new EnumListEqualityComparer<RandomisationArm>());
                    foreach (var block in allArms)
                    {
                        ocurrences[block]++;
                    }
                    
                    double permutations = SpecialFunctions.Factorial(allocations)
                                                    /
                    (double)rClone.Ratios.Values.Aggregate(1, (prev, cur) => (int)(prev * SpecialFunctions.Factorial(cur * rClone.Repeats)));

                    double expectedOccurence = sameBlockRpt/permutations;
                    string assertMessage = null;
                    const double accceptableP = 0.00001;
                    var bin = new MathNet.Numerics.Distributions.Binomial(1/permutations, allArms.Count);
                    foreach (var kv in ocurrences)
                    {
                        string msg;
                        double p;
                        if (kv.Value > expectedOccurence)
                        {
                            p = 1 - bin.CumulativeDistribution(kv.Value - 1);
                            msg = "(as or more extreme)";
                            
                        }
                        else
                        {
                            p = bin.CumulativeDistribution(kv.Value);
                            msg = "(as or less extreme)";
                        }
                        msg = String.Format("{{{0}}} occured {1} times: p {2:N4} " + msg,
                            string.Join(",", kv.Key.Select(a => ((int)a).ToString())),
                            kv.Value,
                            p);
                        Console.WriteLine(msg);
                        if (p < accceptableP)
                        {
                            assertMessage = msg;
                        }
                    }
                    int unusedPermutations = (int)permutations - ocurrences.Count;
                    var cc = ocurrences.CountOfCounts();
                    cc[0] = unusedPermutations;
                    var mean = (double)cc.Sum(kv=>kv.Key*kv.Value)/permutations;
                    var variance = (double)cc.Sum(kv=>Math.Pow(kv.Key - mean,2)*kv.Value)/permutations;
                    Console.WriteLine("mean: {0} variance: {1}", mean, variance);
                    if (unusedPermutations > 0)
                    {
                        //double p0 = MathNet.Numerics.Distributions.Binomial.PMF(unusedPermutations / permutations, (int)permutations * sameBlockRpt, 0);
                        double lambda = sameBlockRpt/permutations;
                        double p0 = MathNet.Numerics.Distributions.Poisson.PMF(lambda,0);
                        double z = (unusedPermutations/sameBlockRpt-p0)/Math.Sqrt(lambda);
                        string msg = String.Format("{0} other permutations were never used, p {1:N4}",
                            unusedPermutations,
                            p0);
                        Console.WriteLine(msg);
                    }
                    
                    /*
                    //to do - get poisson deviance and pearson statistic for binomial in order to validate gof
                    var pBin = MathNet.Numerics.Distributions.ChiSquared.CDF(cc.Count,binCor);
                    var poi = new MathNet.Numerics.Distributions.Poisson(1 / permutations);
                    //
                    var pPoi = MathNet.Numerics.Distributions.ChiSquared.CDF(cc.Count, poiCor);
                     * */
                    if (assertMessage !=null)
                    {
                        throw new AssertFailedException("unacceptably low probability "+ assertMessage);
                    }
                }
            }
        }
    }
}
