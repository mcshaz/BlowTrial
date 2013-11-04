using BlowTrial.Domain.Providers;
using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlowTrial.Helpers
{
    public static class ApplicationDataService
    {
        public static BackupDataSet GetBackupDetails()
        {
            using (var a = new MembershipContext())
            {
                return GetBackupDetails(a);
            }
        }
        public static BackupDataSet GetBackupDetails(IBackupData appData)
        {
            return new BackupDataSet
            {
                BackupData = (from a in appData.BackupDataSet
                    select a).FirstOrDefault(),
                CloudDirectories = appData.CloudDirectories.Select(c=>c.Path).ToList()
            };
        }
        public static void SetBackupDetails(IEnumerable<string> cloudDirectories, int intervalMins, bool? isTobackupToCloud = null, bool? isEnvelopeRandomising=null)
        {
            using (var a = new MembershipContext())
            {
                SetBackupDetails(cloudDirectories,intervalMins, isTobackupToCloud,isEnvelopeRandomising,a);
            }
        }
        internal static void SetBackupDetails(IEnumerable<string> paths, int intervalMins, bool? isTobackupToCloud, bool? isEnvelopeRandomising,IBackupData appDataProvider)
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
            var data = GetBackupDetails(appDataProvider);
            if (data == null)
            {
                if (isTobackupToCloud == null)
                {
                    throw new InvalidOperationException("IsToBackupToCloud must be set if there is no database entry as yet");
                }
                if (isEnvelopeRandomising == null)
                {
                    throw new InvalidOperationException("IsEnvelopeRandomising must be set if there is no database entry as yet");
                }
                appDataProvider.BackupDataSet.Attach(new BackupData
                {
                    BackupIntervalMinutes = intervalMins, 
                    IsBackingUpToCloud = isTobackupToCloud.Value,
                    IsEnvelopeRandomising = isEnvelopeRandomising.Value
                });
            }
            else
            {
                data.BackupData.BackupIntervalMinutes = intervalMins;
                appDataProvider.BackupDataSet.Attach(data.BackupData);
            }

            appDataProvider.SaveChanges();
        }
    }
    public class BackupDataSet
    {
        public BackupData BackupData {get; set;}
        public IEnumerable<string> CloudDirectories { get; set; }
    }
}
