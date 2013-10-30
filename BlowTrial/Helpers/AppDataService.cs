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
        public static void SetBackupDetails(string path, int intervalMins, bool backupToCloud)
        {
            using (var a = new MembershipContext())
            {
                SetBackupDetails(path,intervalMins, backupToCloud,a);
            }
        }
        public static void SetBackupDetails(string path, int intervalMins, bool backupToCloud,IAppDataSet appData)
        {
            if (!Directory.Exists(path))
            {
                throw new ArgumentException("the specified path does not exist", "path");
            }
            try
            {
                appData.Database.ExecuteSqlCommand("delete from AppDataSet");
            }
            catch { }
            appData.AppDataSet.Add(new AppData 
            { CloudDirectory = path, 
                BackupIntervalMinutes = intervalMins,
                BackupToCloud = backupToCloud
            });
            appData.SaveChanges();
        }
        public static FileInfo BackupFileName()
        {
            //var x= (new DataContext).Database.Connection.ConnectionString;
            return new FileInfo(DataContext.ParticipantDbName);
        }

    }
}
