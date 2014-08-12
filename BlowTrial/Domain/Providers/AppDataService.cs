using BlowTrial.Domain.Providers;
using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BlowTrial.Infrastructure.Extensions;

namespace BlowTrial.Helpers
{
    public static class BlowTrialDataService
    {
        public static Exception TestConnection()
        {
            try
            {
                using (var a = new MembershipContext())
                {
                    a.CloudDirectories.Any();
                }
            }
            catch(Exception ex)
            {
                return ex;
            }
            return null;
        }
        public static void ChangeEnvelopeRandomising(bool newVal)
        {
            using (var a = new MembershipContext())
            {
                ChangeEnvelopeRandomising(newVal, a);
            }
        }
        public static void ChangeEnvelopeRandomising(bool newVal, IAppData appData)
        {
            var data = appData.BackupDataSet.First();
            data.IsEnvelopeRandomising = newVal;
            appData.SaveChanges();
        }
        public static RandomisingMessage GetRandomisingMessage()
        {
            using (var a = new MembershipContext())
            {
                return GetRandomisingMessage(a);
            }
        }
        public static RandomisingMessage GetRandomisingMessage(IAppData appData)
        {
            return appData.RandomisingMessages.FirstOrDefault();
        }
        public static bool IsEnvelopeRandomising()
        {
            using (var a = new MembershipContext())
            {
                return IsEnvelopeRandomising(a);
            }
        }
        public static bool IsEnvelopeRandomising(IAppData appData)
        {
            return (from a in appData.BackupDataSet
                    select a.IsEnvelopeRandomising).First();
        }
        public static AllocationGroups GetDefaultAllocationGroup()
        {
            using (var a = new MembershipContext())
            {
                return GetDefaultAllocationGroup(a);
            }
        }
        public static AllocationGroups GetDefaultAllocationGroup(IAppData appData)
        {
            return (from a in appData.BackupDataSet
                    select a.DefaultAllocation).First();
        }
        public static void SetDefaultAllocationGroup(AllocationGroups group)
        {
            using (var a = new MembershipContext())
            {
                SetDefaultAllocationGroup(group, a);
            }
        }
        public static void SetDefaultAllocationGroup(AllocationGroups group, IAppData appData)
        {
            var a = appData.BackupDataSet.First();
            a.DefaultAllocation = group;
            appData.SaveChanges();
        }
        public static AllocationGroups DefaultAllocationGroup()
        {
            using (var a = new MembershipContext())
            {
                return DefaultAllocationGroup(a);
            }
        }
        public static AllocationGroups DefaultAllocationGroup(IAppData appData)
        {
            return (from a in appData.BackupDataSet
                    select a.DefaultAllocation).First();
        }
        public static BackupDataSet GetBackupDetails()
        {
            using (var a = new MembershipContext())
            {
                return GetBackupDetails(a);
            }
        }
        public static BackupDataSet GetBackupDetails(IAppData appData)
        {
            return new BackupDataSet
            {
                BackupData = (from a in appData.BackupDataSet
                    select a).FirstOrDefault(),
                CloudDirectories = appData.CloudDirectories.Select(c=>c.Path).ToList()
            };
        }
        public static void SetRandomisingMessages(string interventionMessage, string controlMessage, string dischargeExplanation)
        {
            using (var a = new MembershipContext())
            {
                SetRandomisingMessages(interventionMessage, controlMessage, dischargeExplanation, a);
            }
        }
        internal static void SetRandomisingMessages(string interventionMessage, string controlMessage, string dischargeExplanation, IAppData appDataProvider)
        {
            try
            {
                appDataProvider.Database.ExecuteSqlCommand("Delete from RandomisingMessages");
            }
            catch (System.Data.SqlClient.SqlException) { }
            appDataProvider.RandomisingMessages.Add(new RandomisingMessage
                {
                    InterventionInstructions = interventionMessage,
                    ControlInstructions = controlMessage,
                    DischargeExplanation = dischargeExplanation
                });
            appDataProvider.SaveChanges();
        }
        public static void SetAppData(string interventionMessage, string controlMessage, string dischargeExplanation, IEnumerable<string> cloudDirectories, int intervalMins, AllocationGroups defaultAllocation, bool isToBackupToCloud, bool isEnvelopeRandomising)
        {
            using (var a = new MembershipContext())
            {
                SetBackupPaths(cloudDirectories, a);
                SetBackupDetails(a, intervalMins, isToBackupToCloud, isEnvelopeRandomising, defaultAllocation);
                SetRandomisingMessages(interventionMessage, controlMessage, dischargeExplanation,a);
            }
        }
        public static void SetBackupDetails(IEnumerable<string> cloudDirectories, int intervalMins)
        {
            using (var a = new MembershipContext())
            {
                SetBackupPaths(cloudDirectories, a);
                a.BackupDataSet.First().BackupIntervalMinutes = intervalMins;
                a.SaveChanges();
            }
        }
        public static void SetBackupDetails(IEnumerable<string> cloudDirectories, int intervalMins, bool isToBackupToCloud, bool isEnvelopeRandomising)
        {
            using (var a = new MembershipContext())
            {
                SetBackupPaths(cloudDirectories, a);
                SetBackupDetails(a, intervalMins, isToBackupToCloud, isEnvelopeRandomising);
            }
        }
        internal static void SetBackupDetails(IAppData appDataProvider, int intervalMins, bool isTobackupToCloud, bool isEnvelopeRandomising, AllocationGroups defaultAllocation = AllocationGroups.India2Arm)
        {
            var data = GetBackupDetails(appDataProvider);
            if (data.BackupData == null)
            {
                appDataProvider.BackupDataSet.Add(new BackupData
                {
                    BackupIntervalMinutes = intervalMins,
                    IsBackingUpToCloud = isTobackupToCloud,
                    IsEnvelopeRandomising = isEnvelopeRandomising,
                    DefaultAllocation = defaultAllocation
                });
            }
            else
            {
                data.BackupData.BackupIntervalMinutes = intervalMins;
            }
            appDataProvider.SaveChanges();
        }
        internal static void SetBackupPaths(IEnumerable<string> paths, IAppData appDataProvider)
        {
            try
            {
                appDataProvider.Database.ExecuteSqlCommand("Delete from CloudDirectories");
            }
            catch (System.Data.SqlClient.SqlException) { }

            foreach (string p in paths)
            {
                if (!Directory.Exists(p))
                {
                    throw new ArgumentException("The path does not exist:" + p, "path");
                }
                appDataProvider.CloudDirectories.Add(new CloudDirectory { Path = p });
            }
            
            appDataProvider.SaveChanges();
        }
        public static bool AnyStudyCentres()
        {
            using (var t = new TrialDataContext())
            {
                return t.StudyCentres.Any();
            }
        }
        public static void DefineNewStudyCentres(IEnumerable<StudySiteItemModel> newStudyCentres)
        {
            using (var t = new TrialDataContext())
            {
                if (t.StudyCentres.Any()) { throw new InvalidOperationException("Study Centres cannot be modified after creation"); }
                foreach (StudySiteItemModel s in newStudyCentres)
                {
                    t.StudyCentres.Add(new Domain.Tables.StudyCentre
                    {
                        Id = s.Id.Value,
                        DuplicateIdCheck = Guid.NewGuid(),
                        ArgbBackgroundColour = s.SiteBackgroundColour.Value.ToInt(),
                        ArgbTextColour = s.SiteTextColour.ToInt(),
                        Name = s.SiteName,
                        HospitalIdentifierMask = s.HospitalIdentifierMask,
                        PhoneMask = s.PhoneMask,
                        MaxIdForSite = s.MaxIdForSite().Value
                    });
                }
                t.SaveChanges(true);
            }
        }
    }
    public class BackupDataSet
    {
        public BackupData BackupData {get; set;}
        public IEnumerable<string> CloudDirectories { get; set; }
    }
}
