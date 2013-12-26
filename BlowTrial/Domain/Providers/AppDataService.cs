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
        public static void StopEnvelopeRandomising()
        {
            using (var a = new MembershipContext())
            {
                StopEnvelopeRandomising(a);
            }
        }
        public static void StopEnvelopeRandomising(IBackupData appData)
        {
            var data = appData.BackupDataSet.First();
            data.IsEnvelopeRandomising = false;
            appData.SaveChanges();
        }
        public static bool IsEnvelopeRandomising()
        {
            using (var a = new MembershipContext())
            {
                return IsEnvelopeRandomising(a);
            }
        }
        public static bool IsEnvelopeRandomising(IBackupData appData)
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
            catch (Exception) {throw;}

            foreach (string p in paths)
            {
                if (!Directory.Exists(p))
                {
                    throw new ArgumentException("The path does not exist:" + p, "path");
                }
                appDataProvider.CloudDirectories.Add(new CloudDirectory { Path = p });
            }
            var data = GetBackupDetails(appDataProvider);
            if (data.BackupData == null)
            {
                if (isTobackupToCloud == null)
                {
                    throw new InvalidOperationException("IsToBackupToCloud must be set if there is no database entry as yet");
                }
                if (isEnvelopeRandomising == null)
                {
                    throw new InvalidOperationException("IsEnvelopeRandomising must be set if there is no database entry as yet");
                }
                appDataProvider.BackupDataSet.Add(new BackupData
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
                t.SaveChanges();
            }
        }
    }
    public class BackupDataSet
    {
        public BackupData BackupData {get; set;}
        public IEnumerable<string> CloudDirectories { get; set; }
    }
}
