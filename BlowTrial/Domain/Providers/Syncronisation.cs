using BlowTrial.Domain.Interfaces;
using BlowTrial.Domain.Tables;
using BlowTrial.Helpers;
using BlowTrial.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using BlowTrial.Infrastructure.Interfaces;
using ErikEJ.SqlCe;
using System.Data.SqlServerCe;
using BlowTrial.Infrastructure.Extensions;
using System.Diagnostics;
using System.Data;

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
            UpsertedVaccineAdministeredIds = new List<int>();
            UpsertedUnsuccessfulFollowUpIds = new List<int>();
            UpdatedParticipantIds = new List<int>();
            AddedScreenPatientIds = new List<int>();
            AddedParticipantIds = new List<int>();
            UpdatedProtocolViolationIds = new List<int>();
            AddedProtocolViolationIds = new List<int>();
        }
        //public IEnumerable<StudyCentre> AddedCentres { get; private set; }
        public List<int> UpdatedParticipantIds { get; private set; }
        public List<int> AddedParticipantIds { get; private set; }
        public List<int> AddedScreenPatientIds { get; private set; }
        //public IEnumerable<Vaccine> AddedVaccine { get; private set; }
        public List<int> UpsertedVaccineAdministeredIds { get; private set; }
        public List<int> UpsertedUnsuccessfulFollowUpIds { get; private set; }
        public List<int> UpdatedProtocolViolationIds { get; private set; }
        public List<int> AddedProtocolViolationIds { get; private set; }

        void Added(Type t, IEnumerable<int> newIds)
        {
            switch (t.Name)
            {
                case "Participant":
                    AddedParticipantIds.AddRange(newIds);
                    break;
                case "ScreenedPatient":
                    AddedScreenPatientIds.AddRange(newIds);
                    break;
                case "ProtocolViolation":
                    AddedProtocolViolationIds.AddRange(newIds);
                    break;
                case "VaccineAdministered":
                    UpsertedVaccineAdministeredIds.AddRange(newIds);
                    break;
                case "UnsuccessfulFollowUp":
                    UpsertedUnsuccessfulFollowUpIds.AddRange(newIds);
                    break;
            }
        }
        void Updated(Type t, IEnumerable<int> updatedIds)
        {
            switch (t.Name)
            {
                case "Participant":
                    UpdatedParticipantIds.AddRange(updatedIds);
                    break;
                case "ProtocolViolation":
                    UpdatedProtocolViolationIds.AddRange(updatedIds);
                    break;
                case "VaccineAdministered":
                    UpsertedVaccineAdministeredIds.AddRange(updatedIds);
                    break;
                case "UnsuccessfulFollowUp":
                    UpsertedUnsuccessfulFollowUpIds.AddRange(updatedIds);
                    break;
            }
        }

        public static SyncronisationResult Sync(ITrialDataContext destContext, IEnumerable<string> dbFileNames, bool updateResults)
        {
            List<StudyCentre> knownSites = destContext.StudyCentres.ToList();
            var returnVar = updateResults?new SyncronisationResult():null;
            IEnumerable<Constraint> fk_constraints = DropFKConstraints(destContext.Database);
            foreach (string f in dbFileNames)
            {
                IEnumerable<Guid> knownSiteIds = knownSites.Select(s => s.DuplicateIdCheck);
                using (var sourceContext = destContext.AttachDb(f))
                {
                    var sourceSites = sourceContext.StudyCentres.AsNoTracking().ToList();
                    foreach (StudyCentre s in sourceSites)
                    {
                        if (knownSiteIds.Contains(s.DuplicateIdCheck)) { continue; }
                        var dup = knownSites.FirstOrDefault(k => k.Id == s.Id);
                        if (dup != null)
                        {
                            throw new DuplicateDataKeyException(string.Format("Duplicate key for site Id:{0} (using Guids {1} & {2})", s.Id.ToString(), s.DuplicateIdCheck, dup.DuplicateIdCheck));
                        }
                        var overlappingSite = knownSites.FirstOrDefault(k => s.Id <= k.MaxIdForSite && s.MaxIdForSite >= k.Id);
                        if (overlappingSite != null)
                        {
                            throw new OverlappingDataKeyRangeException(string.Format("Potential for patient data overwrite - existing site Ids ({0}-{1}), new site Ids ({2}-{3})",
                                overlappingSite.Id, overlappingSite.MaxIdForSite,
                                s.Id, s.MaxIdForSite));
                        }
                        destContext.StudyCentres.Add(s); //because context will have been renewed, this should not cause a duplicate key exception
                        knownSites.Add(s);
                    }
                    destContext.SaveChanges(false);

                    var sourceCentreIdRanges = sourceSites.Select(k => new IntegerRange(k.Id, k.MaxIdForSite)).ToList();
                    var dbDestContext = (DbContext)destContext;
                    foreach (Type t in new Type[] { typeof(Vaccine), typeof(BalancedAllocation), typeof(AllocationBlock), typeof(ScreenedPatient), typeof(Participant), typeof(VaccineAdministered), typeof(ProtocolViolation), typeof(UnsuccessfulFollowUp) })
                    {
                        WriteTime(t, null);
                        var destAllocations = RemainingAllocationsByCentre(t, destContext.Database, sourceCentreIdRanges);
                        WriteTime(t, "RemainingAllocationsByCentre");
                        var modifiedIds = ModifiedIds(t, destContext.Database, sourceContext.Database, destAllocations.UsedAllocations);
                        WriteTime(t, "ModifiedIds");
                        InsertISharedRecord(t, (SqlCeConnection)sourceContext.Database.Connection, (SqlCeConnection)destContext.Database.Connection, destAllocations.RemainingAllocations, modifiedIds);
                        WriteTime(t, "InsertISharedRecord");
                        //UpdateISharedRecord(t, sourceContext.Database, dbDestContext, modifiedIds);
                        //WriteTime(t, "UpdateISharedRecord");
                        if (updateResults)
                        {
                            returnVar.Updated(t, modifiedIds);
                            WriteTime(t, "returnVar.Updated");
                            returnVar.Added(t, GetNewRecordIds(t, destContext.Database, destAllocations.RemainingAllocations));
                            WriteTime(t, "GetNewRecordIds & returnVar.Added");
                        }
                        StopTimer();
                    }
                }
            }
            ReinstateConstraints((SqlCeConnection)destContext.Database.Connection, fk_constraints);
            return returnVar;
        }
#if DEBUG
        static Stopwatch watch = new Stopwatch();
        static long lastMs = 0;
#endif
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        static void WriteTime(Type type, String executedFunction)
        {
#if DEBUG
            if (watch.IsRunning)
            {
                watch.Stop();
                Debug.WriteLine("Type:'{0}' function {1} completed execution at {2:N0} (difference {3:N0}) milliseconds", type.Name, executedFunction, watch.ElapsedMilliseconds, watch.ElapsedMilliseconds-lastMs);
                lastMs = watch.ElapsedMilliseconds;
            }
            else
            {
                Debug.WriteLine("starting upsert for type:'{0}'", type.Name);
            }
            watch.Start();
#endif
        }
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        static void StopTimer()
        {
#if DEBUG
            watch.Stop();
            watch.Reset();
            lastMs = 0;
#endif
        }

        static System.Text.RegularExpressions.Regex constraintNameComponents = new System.Text.RegularExpressions.Regex(@"FK_dbo\.(?<tableName>[^_]+)_dbo\.(?<referencedTable>[^_])+_?<referencedKey>(\w+)");
        class Constraint
        {
            public string Name {get; set;}
            public string Table {get; set;}
            public string UniqueConstraintTable {get; set;}
            public string ColumnName { get; set; }
        }
        static IEnumerable<Constraint> DropFKConstraints(Database destDb)
        {
            const string getConstraints = "select r.CONSTRAINT_NAME \"Name\", r.CONSTRAINT_TABLE_NAME \"Table\", r.UNIQUE_CONSTRAINT_TABLE_NAME UniqueConstraintTable, k.COLUMN_NAME ColumnName"
                + " from INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS r"
                + " INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE k"
                + " ON r.CONSTRAINT_NAME = k.CONSTRAINT_NAME";

            var returnVar = new List<Constraint>(destDb.SqlQuery<Constraint>(getConstraints));
            foreach (Constraint c in returnVar)
            {
                destDb.ExecuteSqlCommand(string.Format("Alter Table {0} Drop Constraint \"{1}\"", c.Table, c.Name));
            }
            return returnVar;
        }

        static void ReinstateConstraints(SqlCeConnection destConn, IEnumerable<Constraint> constraints)
        {
            const string addConstraints = "Alter Table {0} Add Constraint \"{1}\" Foreign Key ({2}) references {3}(Id);";

            bool wasDestClosed;
            if (destConn.State == System.Data.ConnectionState.Closed)
            {
                wasDestClosed = true;
                destConn.Open();
            }
            else
            {
                wasDestClosed = false;
            }


            using (SqlCeCommand cmd = new SqlCeCommand())
            {
                cmd.Connection = destConn;
                cmd.CommandType = CommandType.Text;
                foreach (var c in constraints)
                {
                    cmd.CommandText = string.Format(addConstraints, c.Table, c.Name, c.ColumnName, c.UniqueConstraintTable);
                    cmd.ExecuteNonQuery();
                }
            }

            if (wasDestClosed) { destConn.Close(); }
        }

        public static DateTime? MostRecentEntry(ITrialDataContext context)
        {
            return (new DateTime?[]
                    {
                        context.Participants.Max(p=>(DateTime?)p.RecordLastModified),
                        context.ScreenedPatients.Max(s=>(DateTime?)s.RecordLastModified),
                        context.Vaccines.Where(v=>v.Id>DataContextInitialiser.MaxReservedVaccineId).Max(v=>(DateTime?)v.RecordLastModified),
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
                        context.Vaccines.Where(p=> p.Id > DataContextInitialiser.MaxReservedVaccineId && p.Id >= rng.Min && p.Id<= rng.Max).Max(v=>(DateTime?)v.RecordLastModified),
                        context.VaccinesAdministered.Where(p=>p.Id >= rng.Min && p.Id<= rng.Max).Max(va=>(DateTime?)va.RecordLastModified),
                        context.ProtocolViolations.Where(p=>p.Id >= rng.Min && p.Id<= rng.Max).Max(pv=>(DateTime?)pv.RecordLastModified),
                    }).Max()).Max();
            }
            return null;
        }

        static void UpdateISharedRecord(Type tableType, Database sourceDb, DbContext destContext, IEnumerable<int> IdsToUpdate) 
        {
            if (!IdsToUpdate.Any()) { return;  }
            string tableName = TableName(tableType);
            string queryForNewRecords = string.Format("select * from {0} where {0}.Id in ({1})", tableName, string.Join(",", IdsToUpdate));
            var q = sourceDb.SqlQuery(tableType, queryForNewRecords);
            WriteTime(tableType, "SqlQuery");
            var qc = q.Cast<ISharedRecord>();
            WriteTime(tableType, "Cast");
            destContext.AttachAndMarkModified(tableType, qc.ToArray());
            WriteTime(tableType, "AttachAndMarkModified");
            destContext.SaveChanges();
        }

        static void InsertISharedRecord(Type tableType, SqlCeConnection sourceConn, SqlCeConnection destConn, IEnumerable<IntegerRange> destDbRemainingAllocations, IEnumerable<int> IdsToUpdate) 
        {
            string tableName = TableName(tableType);
            
            bool wasSourceClosed;
            bool wasDestClosed;
            if (sourceConn.State == System.Data.ConnectionState.Closed)
            {
                wasSourceClosed = true;
                sourceConn.Open();
            }
            else
            {
                wasSourceClosed = false;
            }

            if (destConn.State == System.Data.ConnectionState.Closed)
            {
                wasDestClosed = true;
                destConn.Open();
            }
            else
            {
                wasDestClosed = false;
            }
            string queryForNewRecords = string.Format("select * from {0} where ({1})", tableName, WhereRange(destDbRemainingAllocations, tableName));
            if (IdsToUpdate.Any())
            {
                string updatedRecordWhereClause = string.Format("Id in ({0})", string.Join(",", IdsToUpdate));
                using (SqlCeCommand cmd = new SqlCeCommand(string.Format("delete from {0} where {1}", tableName, updatedRecordWhereClause), destConn))
                {
                    cmd.ExecuteNonQuery();
                }
                queryForNewRecords += " or " + updatedRecordWhereClause;
            }
            using (SqlCeBulkCopy bc = new SqlCeBulkCopy(destConn))
            {
                using (SqlCeCommand cmd = new SqlCeCommand(queryForNewRecords, sourceConn))
                {
                    bc.DestinationTableName = tableName;
                    bc.WriteToServer((System.Data.IDataReader)cmd.ExecuteReader());
                }
            }
            if (wasDestClosed) { destConn.Close(); }
            if (wasSourceClosed) { sourceConn.Close();}
        }

        static IEnumerable<int> GetNewRecordIds(Type tableType, Database destContext, IEnumerable<IntegerRange> destDbPriorRemainingAllocations)
        {
            string tableName = TableName(tableType);
            return destContext.SqlQuery<int>(string.Format("select {0}.Id from {0} where {1}", tableName, WhereRange(destDbPriorRemainingAllocations, tableName)));
        }

        static string WhereRange(IEnumerable<IntegerRange> ranges, string tableName)
        {
            return string.Join(" or ", ranges.Select(a => string.Format("({0}.Id >= {1} and {0}.Id <= {2})", tableName, a.Min, a.Max)));
        }

        static IEnumerable<int> ModifiedIds(Type tableType, Database destDb, Database sourceDb, IEnumerable<IntegerRange> destDbUsedAllocations)
        {
            string tableName = TableName(tableType);
            DateTime mostRecentDestAllocation = destDb.SqlQuery<DateTime?>(string.Format("select max({0}.RecordLastModified) from {0}", tableName)).FirstOrDefault()
                ?? (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;
            string whereRange = destDbUsedAllocations.Any()
                ? ("and ("  + WhereRange(destDbUsedAllocations, tableName) +')')
                : "";
            var modifiedDateParam = new SqlCeParameter("@modified", System.Data.SqlDbType.DateTime);
            modifiedDateParam.Value = mostRecentDestAllocation;
            return sourceDb.SqlQuery<int>(string.Format("select {0}.Id from {0} where {0}.RecordLastModified > @modified {1}",
                tableName, 
                whereRange), modifiedDateParam).ToList();
        }
        class AllocationRanges
        {
            public IEnumerable<IntegerRange> UsedAllocations { get; set; }
            public IEnumerable<IntegerRange> RemainingAllocations { get; set; }
        }
        static AllocationRanges RemainingAllocationsByCentre(Type tableType, Database destDb, IEnumerable<IntegerRange> sourceCentreRanges)
        {
            string tableName = TableName(tableType);
            const string queryStringTemplate = "select centreId Min, max(sr.Id) Max from {0} sr group by sr.CentreId";

            string studyCentreCases = string.Join(" ", sourceCentreRanges.Select(a => string.Format("when ({0}.Id >= {1} and {0}.Id <= {2}) then {1}", tableName, a.Min, a.Max)));
            string internalQuery = string.Format("(select {0}.Id, (case {1} end) centreId from {0} where ({2}))", tableName, studyCentreCases, WhereRange(sourceCentreRanges, tableName));
            string queryString = string.Format(queryStringTemplate, internalQuery);

            var usedAllocationDictionary = destDb.SqlQuery<IntegerRange>(queryString).ToDictionary(k=>k.Min);
            var usedAllocations = new IntegerRange[usedAllocationDictionary.Count];
            usedAllocationDictionary.Values.CopyTo(usedAllocations, 0);
            return new AllocationRanges
            {
                UsedAllocations = usedAllocations,
                RemainingAllocations = sourceCentreRanges.Select(sourceRng =>
                {
                    IntegerRange returnVar;
                    if (usedAllocationDictionary.TryGetValue(sourceRng.Min, out returnVar))
                    {
                        return new IntegerRange(returnVar.Max + 1, sourceRng.Max);
                    }
                    return sourceRng;
                }).ToList()
            };
        }

        static string TableName(Type entityType)
        {
            if (entityType==typeof(VaccineAdministered))
            {
                return "VaccinesAdministered";
            }
            return entityType.Name + 's';//nasty - should use search for dataannotation and then use entity framework pleuralisation
        }
        static T[] GetRecordsRequiringUpdating<T>(IQueryable<T> dataSet, IEnumerable<IntegerRange> newSiteIdRanges, DateTime mostRecentBak) where T : class, ISharedRecord
        {
            var returnList = dataSet.Where(p => p.RecordLastModified > mostRecentBak).ToList();
            foreach (IntegerRange rng in newSiteIdRanges)
            {
                var existingEntityIds = returnList.Select(p=>p.Id);
                returnList.AddRange(dataSet.Where(p => p.Id >= rng.Min && p.Id <= rng.Max && !existingEntityIds.Contains(p.Id)).ToList());
            }
            T[] returnVar = new T[returnList.Count];
            returnList.CopyTo(returnVar);
            return returnVar;
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
