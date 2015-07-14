using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatsForAge.DataSets;

namespace BlowTrialUnitTests
{
    [TestClass]
    public class TestCentileData
    {
        [TestMethod]
        public void TestAllAges()
        {
            var wd = new UKWeightData();
            Console.WriteLine( wd.ZForAge(88,365.25*50,true));
        }
    }
}
