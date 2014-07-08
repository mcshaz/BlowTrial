using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlowTrial.Infrastructure.Randomising;
using BlowTrial.Domain.Tables;

namespace BlowTrialUnitTests
{
    [TestClass]
    public class RandomisingUtilityTests
    {
        [TestMethod]
        public void TestGetRatio()
        {
            ArmData.GetRatio(AllocationGroups.IndiaThreeArmBalanced);
        }
    }
}
