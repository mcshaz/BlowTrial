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
using BlowTrial.Infrastructure.Extensions;
using System.Data.Entity;
using System.Data.SqlServerCe;

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
        public void TestRandomising()
        {
            var parts = new List<Participant>(GetMultipleParticipantCategories(1));
            DateTime now = DateTime.Now;
            DateTime yesterday = now - TimeSpan.FromDays(1);
            int maxId;
            using (var db = new TrialDataContext())
            {
                maxId = (from p in db.Participants
                        where p.Id<20000
                        select p.Id).Max();
            }
            
            using (var repo = new Repository(typeof(TrialDataContext)))
            {
                //ratio 2:9:18
                for (int i=1;i <2000;i++)
                {
                    string dummyInfo = "dummyInfo" + i;
                    int weight;
                    if (i < 137) { weight = 1000-i; }
                    else if (i < 620) { weight = 1500 - i / 2; }
                    else { weight = 2000 - i / 5; }
                    repo.AddParticipant(dummyInfo, dummyInfo, dummyInfo, weight, 32, yesterday,
                        "", "1111111", i%2==0, true, now, 1, null, null);
                }
            }
            using (var context = new TrialDataContext())
            {
                var allAllocations = context.Participants.GroupBy(p => p.TrialArm).ToDictionary(p => p.Key, p => p.Count());
                int total = allAllocations.Values.Sum();
                const double tolerance = 0.01;
                const double thirdMin = 1 / 3 - tolerance;
                const double thirdMax = 1 / 3 + tolerance;
                foreach (var kv in allAllocations)
                {
                    double ratio = (double)kv.Value/total;
                    string msg =  string.Format("{0} ratio {1}",kv.Key,ratio);
                    Console.WriteLine(ratio);
                    Assert.IsTrue(thirdMin <= ratio && ratio <= thirdMax, "ratio shoule be 1/3 " + msg);
                }
                foreach (var kv in context.BalancedAllocations.ToDictionary(b=>b.RandomisationCategory, b=>b.IsEqualised))
                {
                    Assert.IsTrue(kv.Value, "Balanced Allocations not set to true for {0}", kv.Value);
                }
                //teardown
                context.Database.ExecuteSqlCommand(String.Format("delete from Participants where Id > {0} and Id <20000", maxId));
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
        IEnumerable<Participant> GetMultipleParticipantCategories(params int[] centreIds)
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
        [TestMethod]
        public void TestMappingVaccines()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<PatientProfiles>();
            });
            using (var context = new TrialDataContext())
            {
                foreach (var p in context.Participants.Include("VaccinesAdministered").Take(10).ToList())
                {
                    
                    var pb = Mapper.Map<ParticipantBaseModel>(p);
                    Assert.IsNotNull(pb.VaccinesAdministered);
                    var pm = Mapper.Map<ParticipantProgressModel>(p);
                    Assert.IsNotNull(pm.VaccineModelsAdministered);
                    Assert.IsNotNull(pm.VaccinesAdministered);
                    
                }
                
            }
        }
        [TestMethod]
        public void TestDbSetExtensions()
        {
            IEnumerable<Participant> parts = GetMultipleParticipantCategories(1);
            Participant p = parts.First();
            p.Id = 1;
            p.RecordLastModified = p.DateTimeBirth = p.RegisteredAt = DateTime.Now;
            int testWt = p.AdmissionWeight;
            RandomisationArm arm = p.TrialArm;
            using (var context = new TrialDataContext())
            {
                context.AttachAndMarkModified(p);
                context.SaveChanges();
                p = context.Participants.Find(1);
                Assert.AreEqual(testWt, p.AdmissionWeight);
                Assert.AreEqual(arm, p.TrialArm);
                context.Participants.Find(2);
                p.Id = 2;
                context.AttachAndMarkModified(p);
                p = context.Participants.Find(2);
                Assert.AreEqual(testWt, p.AdmissionWeight);
                Assert.AreEqual(arm, p.TrialArm);

            }

        }
    }
}
