using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlowTrial.Domain.Providers;
using BlowTrial.Models;
using System.Collections.Generic;
using AutoMapper;
using BlowTrial.Infrastructure.Automapper;
using System.Linq;
using BlowTrial.ViewModel;
using BlowTrial.Migrations.TrialData;
using BlowTrial.Infrastructure.Randomising;
using BlowTrial.Domain.Tables;
using BlowTrial.Domain.Outcomes;
using BlowTrial.Infrastructure.Randomising;

namespace BlowTrialUnitTests
{
    [TestClass]
    public class LiveDataTests
    {
        [TestMethod]
        public void AllLiveDemographicsUpdatable()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<PatientProfiles>();
            }); 
            List<int> idsCannotExecute = new List<int>();
            using (var liveRepo = new Repository(() => new TrialDataContext()))
            { 
                foreach (var p in liveRepo.Participants.ToList())
                {
                    PatientDemographicsModel model = Mapper.Map<PatientDemographicsModel>(p);
                    model.StudyCentre = liveRepo.FindStudyCentre(p.CentreId);
                    Assert.IsNotNull(model.StudyCentre,"Foreign key constraint violated: database does not include record for given study centre id: {0}", p.CentreId);
                    var vm = new PatientDemographicsViewModel(liveRepo, model);
                    vm.Name += " ";
                    if (!vm.UpdateDemographicsCmd.CanExecute(null)) {idsCannotExecute.Add(model.Id);}
                }
            }
            Assert.IsFalse(idsCannotExecute.Any(), "unable to update demographics for ids: {0}", string.Join(",",idsCannotExecute));
        }
        [TestMethod]
        public void TestTrialDataSeed()
        {
            var tst = new SeedTestUtility();
            using (var t = new TrialDataContext())
            {
                tst.SeedTest(t);
            }
        }
        private class SeedTestUtility : TrialDataConfiguration
        {
            internal void SeedTest(BlowTrial.Domain.Providers.TrialDataContext context)
            {
                Seed(context);
            }
        }
        [TestMethod]
        public void TestSpecificAllocationBlocks()
        {
            using (var context = new TrialDataContext())
            {
                foreach (var part in GetMultipleParticipantCategories(new int[] {1, 20000}))
                {
                    if (part.TrialArm != RandomisationArm.NotSet)
                    {

                        RandomisationStrata strata;
                        var nextBlock = Engine.Get1stBlockWithSpaceForSpecificAllocation(part, out strata, context);
                        if (nextBlock == null)
                        {
                            Console.WriteLine("no spare allocations found for male:{0}, wt:{1}, arm:{2}",
                                part.IsMale, part.AdmissionWeight, part.TrialArm);
                            continue;
                        }
                        int parts = context.Participants.Count(p => p.AllocationBlockId == nextBlock.Id && p.TrialArm == part.TrialArm);
                        BlockComponent b = nextBlock.GetComponents();
                        int allocs = b.Ratios[part.TrialArm] * b.Repeats;
                        string outcome = string.Format("first block (Id:{0}) with space for allocation has {1} of {2} allocations taken",
                            nextBlock.Id,
                            parts,
                            allocs);
                        Assert.IsTrue(allocs > parts, outcome);
                        Console.WriteLine(outcome);
                    }
                }
            }
        }
        [TestMethod]
        public void TestUnfilledAllocationBlocks()
        {
            using (var context = new TrialDataContext())
            {
                foreach (var part in GetMultipleParticipantCategories(new int[] { 1, 20000 }).Where(p=>p.TrialArm== RandomisationArm.Control))
                {
                    part.TrialArm = RandomisationArm.NotSet;

                    RandomisationStrata strata;
                    var nextBlock = Engine.Get1stUnfilledBlock(part, out strata, context);
                    if (nextBlock == null)
                    {
                        Console.WriteLine("no spare allocations found for male:{0}, wt:{1}",
                            part.IsMale, part.AdmissionWeight, part.TrialArm);
                        continue;
                    }
                    int parts = context.Participants.Count(p => p.AllocationBlockId == nextBlock.Id);
                    BlockComponent b = nextBlock.GetComponents();
                    int allocs = b.Ratios.Values.Sum() * b.Repeats;
                    string outcome = string.Format("first block (Id:{0}) with space for allocation has {1} of {2} allocations taken",
                        nextBlock.Id,
                        parts,
                        allocs);
                    Assert.IsTrue(allocs > parts, outcome);
                    Console.WriteLine(outcome);
                }
            }
        }
        IEnumerable<Participant> GetMultipleParticipantCategories(IEnumerable<int> centreIds)
        {
            var returnVar = new List<Participant>();
            foreach (var cId in centreIds)
            {
                foreach (int wt in new int[] { 750, 1250, 1750 })
                {
                    foreach (bool male in new bool[] { true, false })
                    {
                        foreach (RandomisationArm arm in Enum.GetValues(typeof(RandomisationArm)))
                        {
                            returnVar.Add(new Participant { IsMale = male, AdmissionWeight = wt, CentreId = cId, TrialArm = arm });
                        }
                    }
                }
            }
            return returnVar;
        }

    }
}
