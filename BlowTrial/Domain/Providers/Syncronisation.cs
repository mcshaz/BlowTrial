using BlowTrial.Domain.Interfaces;
using BlowTrial.Domain.Tables;
using BlowTrial.Helpers;
using BlowTrial.Infrastructure.Exceptions;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using BlowTrial.Infrastructure.Interfaces;

namespace BlowTrial.Domain.Providers
{
    /*
     * note syncronisation framework 2.1 only works with CE 3.5
    using Microsoft.Synchronization.Data.SqlServerCe;
    using Microsoft.Synchronization.Data;
    using Microsoft.Synchronization;
    using System.Data.SqlServerCe;

        public static void SyncCe(string localCeConnectionString, string remoteCeConnectionString)
        {

            // create a connection to the SyncCompactDB database
            using (SqlCeConnection masterConn = new SqlCeConnection(localCeConnectionString))
            {
                // create the sync orhcestrator
                SyncOrchestrator syncOrchestrator = new SyncOrchestrator();
                // set the remote provider of orchestrator to a server sync provider associated with
                // the ProductsScope in the SyncDB server database
                syncOrchestrator.LocalProvider = new SqlCeSyncProvider("ProductsScope", masterConn);
                using (SqlCeConnection clientConn = new SqlCeConnection(remoteCeConnectionString))
                {
                    // set local provider of orchestrator to a CE sync provider associated with the 
                    // ProductsScope in the SyncCompactDB compact client database
                    syncOrchestrator.LocalProvider = new SqlCeSyncProvider("ProductsScope", clientConn);

                    // set the direction of sync session to Upload and Download
                    syncOrchestrator.Direction = SyncDirectionOrder.Upload;

                    // subscribe for errors that occur when applying changes to the client
                    ((SqlCeSyncProvider)syncOrchestrator.LocalProvider).ApplyChangeFailed += new EventHandler<DbApplyChangeFailedEventArgs>(Program_ApplyChangeFailed);

                    // execute the synchronization process
                    SyncOperationStatistics syncStats = syncOrchestrator.Synchronize();

                    // print statistics
                    Console.WriteLine("Start Time: " + syncStats.SyncStartTime);
                    Console.WriteLine("Total Changes Uploaded: " + syncStats.UploadChangesTotal);
                    Console.WriteLine("Total Changes Downloaded: " + syncStats.DownloadChangesTotal);
                    Console.WriteLine("Complete Time: " + syncStats.SyncEndTime);
                    Console.WriteLine(String.Empty);
                }
            }
        }
       
        static void Program_ApplyChangeFailed(object sender, DbApplyChangeFailedEventArgs e)
        {
            // display conflict type
            Console.WriteLine(e.Conflict.Type);

            // display error message 
            Console.WriteLine(e.Error);
        }
        */

    public class SyncronisationResult
    {
        private SyncronisationResult()
        {
            UpdatedParticipantIds = new List<int>();
            ModifiedVaccineAdministeredIds = new List<int>();
            UpdatedParticipantIds = new List<int>();
            AddedScreenPatientIds = new List<int>();
            AddedParticipantIds = new List<int>();
            ModifiedProtocolViolationIds = new List<int>();
        }
        //public IEnumerable<StudyCentre> AddedCentres { get; private set; }
        public List<int> UpdatedParticipantIds { get; private set; }
        public List<int> AddedParticipantIds { get; private set; }
        public List<int> AddedScreenPatientIds { get; private set; }
        //public IEnumerable<Vaccine> AddedVaccine { get; private set; }
        public List<int> ModifiedVaccineAdministeredIds { get; private set; }
        public List<int> ModifiedProtocolViolationIds { get; private set; }

        public static SyncronisationResult Sync(ITrialDataContext context, IEnumerable<string> dbFileNames)
        {
            List<StudyCentre> knownSites = context.StudyCentres.ToList();

            var returnVar = new SyncronisationResult();
            foreach (string f in dbFileNames)
            {
                IEnumerable<Guid> knownSiteIds = knownSites.Select(s => s.DuplicateIdCheck);
                using (var downloadedDb = context.AttachDb(f))
                {
                    var newSites = (from s in downloadedDb.StudyCentres.AsNoTracking()
                                    where !knownSiteIds.Contains(s.DuplicateIdCheck)
                                    select s).ToList();
                    foreach (StudyCentre s in newSites)
                    {
                        var dup = knownSites.FirstOrDefault(k=>k.Id == s.Id);
                        if (dup != null)
                        {
                            throw new DuplicateDataKeyException(string.Format("Duplicate key for site Id:{0} (using Guids {1} & {2})",s.Id.ToString(),s.DuplicateIdCheck, dup.DuplicateIdCheck));
                        }
                        var overlappingSite = knownSites.FirstOrDefault(k=>s.Id <= k.MaxIdForSite && s.MaxIdForSite >= k.Id);
                        if (overlappingSite != null)
                        {
                            throw new OverlappingDataKeyRangeException(string.Format("Potential for patient data overwrite - existing site Ids ({0}-{1}), new site Ids ({2}-{3})",
                                overlappingSite.Id, overlappingSite.MaxIdForSite,
                                s.Id, s.MaxIdForSite));
                        }
                        context.StudyCentres.Add(s); //because context will have been renewed, this should not cause a duplicate key exception
                        knownSites.Add(s);
                    }
                    context.SaveChanges(false); 

                    List<IntegerRange> newSiteIdRanges = newSites.Select(s => new IntegerRange(s.Id, s.MaxIdForSite)).ToList();
                    DateTime mostRecentBak = MostRecentEntry(context, downloadedDb.StudyCentres.Select(s => new IntegerRange{ Min = s.Id, Max = s.MaxIdForSite}).ToList())
                        ?? System.Data.SqlTypes.SqlDateTime.MinValue.Value;

                    var modifiedVaccines = GetRecordsRequiringUpdating(downloadedDb.Vaccines.AsNoTracking().Where(v => !DataContextInitialiser.SeedVaccineIds.Contains(v.Id)), newSiteIdRanges, mostRecentBak);
                    context.Vaccines.AddOrUpdate(modifiedVaccines);
                    context.SaveChanges(false);

                    var modifiedScreenedPatients = GetRecordsRequiringUpdating(downloadedDb.ScreenedPatients.AsNoTracking(), newSiteIdRanges, mostRecentBak);
                    returnVar.AddedScreenPatientIds.AddRange(modifiedScreenedPatients.Select(va=>va.Id));
                    context.ScreenedPatients.AddOrUpdate(modifiedScreenedPatients);
                    context.SaveChanges(false);

                    var balanceAllocs = GetRecordsRequiringUpdating(downloadedDb.BalancedAllocations.AsNoTracking(), newSiteIdRanges, mostRecentBak);
                    context.BalancedAllocations.AddOrUpdate(balanceAllocs);
                    context.SaveChanges(false);

                    var modifiedBlocks = GetRecordsRequiringUpdating(downloadedDb.AllocationBlocks.AsNoTracking(), newSiteIdRanges, mostRecentBak);
                    context.AllocationBlocks.AddOrUpdate(modifiedBlocks);
                    context.SaveChanges(false);

                    var modifiedParticipants = GetRecordsRequiringUpdating(downloadedDb.Participants.AsNoTracking(), newSiteIdRanges, mostRecentBak);
                    var modifiedParticipantIds = modifiedParticipants.Select(p=>p.Id).ToList();
                    var participantsInDb = (from p in context.Participants
                                            where modifiedParticipantIds.Contains(p.Id)
                                            select p.Id).ToList();
                    returnVar.AddedParticipantIds.AddRange(modifiedParticipantIds
                        .Where(p=>!participantsInDb.Contains(p)));
                    returnVar.UpdatedParticipantIds.AddRange(participantsInDb);
                    context.Participants.AddOrUpdate(modifiedParticipants);
                    context.SaveChanges(false);

                    var modifiedVaccinesAdmin = GetRecordsRequiringUpdating(downloadedDb.VaccinesAdministered.AsNoTracking(), newSiteIdRanges, mostRecentBak);
                    returnVar.ModifiedVaccineAdministeredIds.AddRange(modifiedVaccinesAdmin.Select(va => va.Id));
                    context.VaccinesAdministered.AddOrUpdate(modifiedVaccinesAdmin);
                    context.SaveChanges(false);

                    var modifiedProtocolViolations = GetRecordsRequiringUpdating(downloadedDb.ProtocolViolations.AsNoTracking(), newSiteIdRanges, mostRecentBak);
                    returnVar.ModifiedProtocolViolationIds.AddRange(modifiedProtocolViolations.Select(va => va.Id));
                    context.ProtocolViolations.AddOrUpdate(modifiedProtocolViolations);
                    context.SaveChanges(false);
                }
            }
            return returnVar;
        }
        public static DateTime? MostRecentEntry(ITrialDataContext context)
        {
            return (new DateTime?[]
                    {
                        context.Participants.Max(p=>(DateTime?)p.RecordLastModified),
                        context.ScreenedPatients.Max(s=>(DateTime?)s.RecordLastModified),
                        context.Vaccines.Where(v=>!DataContextInitialiser.SeedVaccineIds.Contains(v.Id)).Max(v=>(DateTime?)v.RecordLastModified),
                        context.VaccinesAdministered.Max(va=>(DateTime?)va.RecordLastModified),
                        context.ProtocolViolations.Max(pv=>(DateTime?)pv.RecordLastModified),
                    }).Max();
        }
        public static DateTime? MostRecentEntry(ITrialDataContext context, IEnumerable<IntegerRange> idRanges)
        {
            if (idRanges.Any())
            {
                return (idRanges.Select(rng=>new DateTime?[]
                    {
                        context.Participants.Where(p=>p.CentreId == rng.Min).Max(p=>(DateTime?)p.RecordLastModified),
                        context.ScreenedPatients.Where(p=>p.CentreId == rng.Min).Max(s=>(DateTime?)s.RecordLastModified),
                        context.Vaccines.Where(p=> !DataContextInitialiser.SeedVaccineIds.Contains(p.Id) && p.Id >= rng.Min && p.Id<= rng.Max).Max(v=>(DateTime?)v.RecordLastModified),
                        context.VaccinesAdministered.Where(p=>p.Id >= rng.Min && p.Id<= rng.Max).Max(va=>(DateTime?)va.RecordLastModified),
                        context.ProtocolViolations.Where(p=>p.Id >= rng.Min && p.Id<= rng.Max).Max(pv=>(DateTime?)pv.RecordLastModified),
                    }).Max()).Max();
            }
            return null;
        }
        static T[] GetRecordsRequiringUpdating<T>(IQueryable<T> dataSet, IEnumerable<IntegerRange> newSiteIdRanges, DateTime mostRecentBak) where T : class, ISharedRecord
        {
            var returnVar = dataSet.Where(p => p.RecordLastModified > mostRecentBak).ToList();
            foreach (IntegerRange rng in newSiteIdRanges)
            {
                var existingEntityIds = returnVar.Select(p=>p.Id);
                returnVar.AddRange(dataSet.Where(p => p.Id >= rng.Min && p.Id <= rng.Max && !existingEntityIds.Contains(p.Id)).ToList());
            }
            return returnVar.ToArray();
            /* 
             * this is a better way to do it, but it is not working, and stopped working after moving to 
             * a generic method
            var predicate = PredicateBuilder.False<T>();
            predicate.Or(p => p.RecordLastModified > mostRecentBak);
            foreach (IntegerRange rng in newSiteIdRanges)
            {
                predicate = predicate.Or(p => p.Id >= rng.Min && p.Id <= rng.Max);
            }
            return dataSet.AsNoTracking().AsExpandable().Where(predicate).ToArray();
             * */
        }
    }
}
