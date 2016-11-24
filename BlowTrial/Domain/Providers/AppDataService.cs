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

        public static void SetAppData(IEnumerable<string> cloudDirectories, int intervalMins, bool isToBackupToCloud, bool isEnvelopeRandomising)
        {
            using (var a = new MembershipContext())
            {
                SetBackupPaths(cloudDirectories, a);
                SetBackupDetails(a, intervalMins, isToBackupToCloud, isEnvelopeRandomising);
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
        internal static void SetBackupDetails(IAppData appDataProvider, int intervalMins, bool isTobackupToCloud, bool isEnvelopeRandomising)
        {
            var data = GetBackupDetails(appDataProvider);
            if (data.BackupData == null)
            {
                appDataProvider.BackupDataSet.Add(new BackupData
                {
                    BackupIntervalMinutes = intervalMins,
                    IsBackingUpToCloud = isTobackupToCloud,
                    IsEnvelopeRandomising = isEnvelopeRandomising,
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
                    t.StudyCentres.Add(ViewModel.StudySitesViewModel.MapToStudySite(s));
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
