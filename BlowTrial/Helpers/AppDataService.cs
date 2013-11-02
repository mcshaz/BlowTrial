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
    public static class AppDataService
    {
        public static AppData GetBackupDetails()
        {
            using (var a = new MembershipContext())
            {
                return GetBackupDetails(a);
            }
        }
        public static AppData GetBackupDetails(IAppDataSet appData)
        {
            return (from a in appData.AppDataSet
                    select a).FirstOrDefault();
        }
        public static void SetBackupDetails(string path, int intervalMins, bool? backupToCloud = null)
        {
            using (var a = new MembershipContext())
            {
                SetBackupDetails(path,intervalMins, backupToCloud,a);
            }
        }
        public static void SetBackupDetails(string path, int intervalMins, bool? backupToCloud,IAppDataSet appDataProvider)
        {
            if (!Directory.Exists(path))
            {
                throw new ArgumentException("the specified path does not exist", "path");
            }
            var data = GetBackupDetails(appDataProvider);
            if (data == null)
            {
                if (backupToCloud == null)
                {
                    throw new InvalidOperationException("BackupToCloud must be set if there is no database entry as yet");
                }
                appDataProvider.AppDataSet.Attach(new AppData
                    {
                        CloudDirectory = path, BackupIntervalMinutes = intervalMins, BackupToCloud = backupToCloud.Value
                    });
            }
            else
            {
                data.BackupIntervalMinutes = intervalMins;
                data.CloudDirectory = path;
                appDataProvider.AppDataSet.Attach(data);
            }
            appDataProvider.SaveChanges();
        }

    }
}
