namespace BlowTrial.Migrations.TrialData
{
    using BlowTrial.Domain.Providers;
    using BlowTrial.Domain.Tables;
    using BlowTrial.Infrastructure;
    using BlowTrial.Infrastructure.Randomising;
    using ErikEJ.SqlCe;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Text;
    using BlowTrial.Infrastructure.Extensions;

    public class TrialDataConfiguration : DbMigrationsConfiguration<TrialDataContext>
    {
        public TrialDataConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(TrialDataContext context)
        {
            /*
            if (!(from v in context.Vaccines
                where v.Id == DataContextInitialiser.DanishBcg.Id
                select v).Any())
            */
            //natural key argument on name not id in case name changes
            context.Vaccines.AddOrUpdate(DataContextInitialiser.RussianBcg); //Guid.ParseExact("8eeb3307-445c-475f-b150-a51a66559ae2","D")
            context.Vaccines.AddOrUpdate(DataContextInitialiser.Opv); //Guid.ParseExact("161a6935-3e1d-4362-9b23-eb6f49512fc9", "D") 
            context.Vaccines.AddOrUpdate(DataContextInitialiser.HepB); // Guid.ParseExact("a756168a-2e0b-404b-a903-bc0cfd02f33f", "D")
            context.Vaccines.AddOrUpdate(DataContextInitialiser.DanishBcg);
            context.Vaccines.AddOrUpdate(DataContextInitialiser.BcgGreenSignal);
            context.Vaccines.AddOrUpdate(DataContextInitialiser.BcgJapan);
            context.SaveChanges(true);

            /*
            if(context.StudyCentres.Any(s=>s.Id == 1) && !context.Participants.Any(p=>p.WasEnvelopeRandomised))
            {
                context.Database.ExecuteSqlCommand(string.Format("UPDATE [Participants] SET [WasEnvelopeRandomised] = 1 WHERE Id <= {0} OR MultipleSiblingId <= {0};", EnvelopeDetails.MaxEnvelopeNumber));
            }
            context.Database.ExecuteSqlCommand("UPDATE [Participants] SET [PhoneNumber] = null WHERE SUBSTRING(PhoneNumber,LEN(PhoneNumber)-8,8) = '99999999';");
            
            
            if (!context.Participants.Any(p=>p.RandomisationCategory!=RandomisationCategories.NotSet))
            {
                ExecuteSqlCommands(context.Database, Envelope.UpdateCategoriesSql);
                ExecuteSqlCommands(context.Database, string.Format(
                    "UPDATE [Participants] SET [RandomisationCategory] = 1 WHERE [RandomisationCategory]=0 AND IsMale = 1 AND AdmissionWeight < {0};" +
                    "UPDATE [Participants] SET [RandomisationCategory] = 2 WHERE [RandomisationCategory]=0 AND IsMale = 0 AND AdmissionWeight < {0};" +
                    "UPDATE [Participants] SET [RandomisationCategory] = 3 WHERE [RandomisationCategory]=0 AND IsMale = 1 AND AdmissionWeight >= {0} AND AdmissionWeight < {1};" +
                    "UPDATE [Participants] SET [RandomisationCategory] = 4 WHERE [RandomisationCategory]=0 AND IsMale = 0 AND AdmissionWeight >= {0} AND AdmissionWeight < {1};" +
                    "UPDATE [Participants] SET [RandomisationCategory] = 5 WHERE [RandomisationCategory]=0 AND IsMale = 1 AND AdmissionWeight >= {1};" +
                    "UPDATE [Participants] SET [RandomisationCategory] = 6 WHERE [RandomisationCategory]=0 AND IsMale = 0 AND AdmissionWeight >= {1};",
                    RandomisingEngine.BlockWeight1, RandomisingEngine.BlockWeight2));
            }
            */
            if (!context.BalancedAllocations.Any())
            {
                context.BalancedAllocations.AddRange(GetAllBalancedAllocations(context.StudyCentres.Select(s => s.Id).ToList()));
                context.SaveChanges();
            }

            if (!context.AllocationBlocks.Any() && context.Participants.Any())
            {
                CreateAllocationBlocks(context);
                //context.Database.ExecuteSqlCommand("Alter Table Participants Drop Column BlockNumber");
                //context.Database.ExecuteSqlCommand("Alter Table Participants Drop Column BlockSize");
            }
            const int replicatedPartId = 20206;
            if (context.VaccinesAdministered.Count(va=>va.ParticipantId==replicatedPartId && va.VaccineId==1)>2)
            {
                var vaIds = (from va in context.VaccinesAdministered
                             where va.ParticipantId == replicatedPartId
                             orderby va.Id
                             select va.Id).ToList();
                vaIds.RemoveRange(vaIds.Count - 2, 2);
                context.Database.ExecuteSqlCommand(string.Format("delete from VaccinesAdministered where Id in ({0})", string.Join(",", vaIds)));
            }

            if(!context.Participants.Any(p=>p.FollowUpBabyBCGReaction != Domain.Outcomes.FollowUpBabyBCGReactionStatus.Missing) && context.StudyCentres.Any(s=>s.Id==20000))
            {
                UpdatePapuleData(context);
            }

            ReIdUnsuccesfulFollowUps.Process(context);
        }
        class OldParticipant 
        {
            public int Id { get; set; }
            public int CentreId { get; set; }
            public bool WasEnvelopeRandomised { get; set; }
            public int? BlockNumber { get; set; }
            public int BlockSize { get; set; }
            public int Strata { get; set; }
        }
        static void CreateAllocationBlocks(TrialDataContext context)
        {
            var participants = context.Database.SqlQuery(typeof(OldParticipant), string.Format(
                "select Id,CentreId,WasEnvelopeRandomised,BlockNumber,BlockSize, case when IsMale = 1 then case when AdmissionWeight< {0} then {2} when AdmissionWeight >={1} then {3} else {4} end else case when AdmissionWeight< {0} then {5} when AdmissionWeight >={1} then {6} else {7} end end as Strata from Participants order by CentreId, BlockNumber, Id",
                Engine.BlockWeight1, 
                Engine.BlockWeight2,
                (int)RandomisationStrata.SmallestWeightMale,
                (int)RandomisationStrata.TopWeightMale,
                (int)RandomisationStrata.MidWeightMale,
                (int)RandomisationStrata.SmallestWeightFemale,
                (int)RandomisationStrata.TopWeightFemale,
                (int)RandomisationStrata.MidWeightFemale)).Cast<OldParticipant>();
            const string updatePreface = "Update Participants set AllocationBlockId = Case Id";
            const string updateSufix = " else AllocationBlockId end;";
            const int maxArgs = 64;
            List<StringBuilder> participantUpdate = new List<StringBuilder>();
            StringBuilder currentUpdate = new StringBuilder(updatePreface);
            participantUpdate.Add(currentUpdate);
            List<AllocationBlock> blocksToInsert = new List<AllocationBlock>();
            var now = DateTime.Now;
            int currentCentreId = 0;
            int nextId = 0;
            int argCount = 0;
            IDictionary<int, RandomisationStrata> blockNumberLookup = null;
             
            foreach (var g in participants.GroupBy(p=>new {p.CentreId, p.BlockNumber }))
            {
                if (g.Key.CentreId != currentCentreId)
                {
                    currentCentreId = g.Key.CentreId;
                    nextId = (currentCentreId==1)
                        ?EnvelopeDetails.MaxEnvelopeNumber+1
                        :currentCentreId;
                }
                //if envelope randomised, use the allocationGroupId for the correctly assigned group
                //otherwise, keep the allocationGroupId for the goup with the highest number

                IEnumerable<IGrouping<int,OldParticipant>> forReallocation = g.GroupBy(p => p.Strata).ToList();
                if (currentCentreId == 1 && g.Key.BlockNumber.HasValue)
                {
                    var currentAlloc = new AllocationBlock
                    {
                        Id = g.Key.BlockNumber.Value,
                        AllocationGroup = AllocationGroups.India2Arm,
                        RecordLastModified = now
                    };
                    blocksToInsert.Add(currentAlloc);

                    if (g.Any(p => p.WasEnvelopeRandomised))
                    {
                        RandomisationStrata definedStrata;
                        (blockNumberLookup ?? (blockNumberLookup = EnvelopeDetails.GetStrataByBlockNumber())).TryGetValue(g.Key.BlockNumber.Value, out definedStrata);
                        int strataVal = (int)definedStrata;
                        var correctlyAssigned = forReallocation.FirstOrDefault(r=>r.Key == strataVal);
                        if (correctlyAssigned==null)
                        {
                            blocksToInsert.RemoveAt(blocksToInsert.Count-1);
                        }
                        else
                        {
                            currentAlloc.GroupRepeats = (byte)((correctlyAssigned.Count()-1)/2 + 1);
                            currentAlloc.RandomisationCategory = definedStrata;
                            forReallocation = forReallocation.Where(r=>r!=correctlyAssigned);
                        }
                    }
                    else
                    {
                        int maxCount = 0;
                        int largestRandCat = 0;
                        foreach (var r in forReallocation)
                        {
                            int grpCount = r.Count();
                            if (grpCount > maxCount)
                            {
                                maxCount = grpCount;
                                largestRandCat = r.Key;
                            }
                        }
                        currentAlloc.RandomisationCategory = (RandomisationStrata)largestRandCat;
                        currentAlloc.GroupRepeats = (byte)((maxCount-1)/2+1);
                        forReallocation = forReallocation.Where(r => r.Key != largestRandCat);
                        System.Diagnostics.Debug.Assert(currentAlloc.Id != 1);
                    }
                    
                }
                

                foreach (var s in forReallocation)
                {
                    string nextIdStr = nextId.ToString();
                    blocksToInsert.Add(new AllocationBlock 
                    { 
                        Id = nextId++, 
                        AllocationGroup = AllocationGroups.India2Arm, 
                        GroupRepeats = (byte)((s.Count()-1) / 2 + 1), 
                        RandomisationCategory = (RandomisationStrata)s.Key, 
                        RecordLastModified = now });
                    argCount += s.Count();
                    if (argCount > maxArgs)
                    {
                        argCount = 0;
                        currentUpdate.Append(updateSufix);
                        currentUpdate = new StringBuilder(updatePreface);
                        participantUpdate.Add(currentUpdate);
                    }
                    currentUpdate.AppendFormat(string.Join("", s.Select(p => " When " + p.Id + " Then " + nextIdStr)));
                    
                }

            }
            using (SqlCeBulkCopy bc = new SqlCeBulkCopy((System.Data.SqlServerCe.SqlCeConnection)context.Database.Connection))
            {
                bc.DestinationTableName = "AllocationBlocks";
                bc.WriteToServer(blocksToInsert);
            }
            
            if (argCount==0)
            {
                participantUpdate.RemoveAt(participantUpdate.Count - 1);
            }
            else
            {
                currentUpdate.Append(updateSufix);
            }
            
            foreach (var sb in participantUpdate)
            {
                context.Database.ExecuteSqlCommand(sb.ToString());
            }
            context.Database.ExecuteSqlCommand("update Participants set RecordLastModified = GetDate()");
            context.Database.ExecuteSqlCommand("Update Participants set AllocationBlockId = BlockNumber where AllocationBlockId is null;");
        }
        internal static IEnumerable<BalancedAllocation> GetAllBalancedAllocations(ICollection<int> centreIds)
        {
            var strata = Enum.GetValues(typeof(RandomisationStrata)).Cast<RandomisationStrata>().Where(a => a != RandomisationStrata.NotSet).ToList();
            var now = DateTime.Now;
            var returnVar = new List<BalancedAllocation>(centreIds.Count * strata.Count);
            foreach (int centreId in centreIds)
            {
                int nextId = centreId;
                foreach (RandomisationStrata s in strata)
                {
                    returnVar.Add(new BalancedAllocation { StudyCentreId = centreId, Id = nextId++, RandomisationCategory = s, RecordLastModified = now });
                }
            }
            return returnVar;
        }
        static void ExecuteSqlCommands(Database db, string commands)
        {
            foreach (string s in commands.Split(new string[]{ "go","Go","GO",";" },StringSplitOptions.None))
            {
                if (s.Trim('\r','\n',' ','\t') != "")
                {
                    db.ExecuteSqlCommand(s);
                }
            }
        }
        static void UpdatePapuleData(TrialDataContext context)
        {
            var sb = new StringBuilder();
            string now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.ff");
            var papuleData = PapuleData.GetSeedData();
            //System.Diagnostics.Debug.Assert(papuleData.Length == 344, "Required number of papule data records not correct");
            foreach (PapuleData p in papuleData)
            {
                sb.AppendFormat("UPDATE Participants SET FollowUpBabyBCGReaction = {0}, RecordLastModified='{1}'", p.Scar, now);
                if (p.WeeksPost.HasValue)
                {
                    DateTime vaccineGiven = context.Database.SqlQuery<DateTime>("Select AdministeredAt From VaccinesAdministered Where VaccineId=1 AND ParticipantId=" + p.Id).FirstOrDefault();
                    DateTime contact = (vaccineGiven + p.WeeksPost.Value);
                    if (p.Scar==0)
                    {
                        context.UnsuccessfulFollowUps.Add(new UnsuccessfulFollowUp { AttemptedContact = contact.Date, RecordLastModified = DateTime.Now, ParticipantId = p.Id });
                        sb.Clear();
                        continue;
                    }
                    else
                    {
                        sb.AppendFormat(", FollowUpContactMade = '{0:yyyy-MM-dd}'", contact);
                    }
                    
                }
                sb.Append(" Where Id=" + p.Id);

                int r = context.Database.ExecuteSqlCommand(sb.ToString());
                //System.Diagnostics.Debug.Assert(r==1,"Assertion that 1 record changed failed");
                sb.Clear();
            }
            context.SaveChanges();
            //System.Diagnostics.Debug.Assert(context.Participants.Count(pt => pt.FollowUpBabyBCGReaction != 0) == papuleData.Length,"database updated records");
        }
    }
}
