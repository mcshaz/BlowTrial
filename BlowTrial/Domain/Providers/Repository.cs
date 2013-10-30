using BlowTrial.Domain.Interfaces;
using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BlowTrial.Domain.Providers
{
    public class Repository : IRepository
    {
        #region Constructors
        public Repository(Type iDataContextType) 
        {
            _typeConstructor = iDataContextType.GetConstructor(System.Type.EmptyTypes);
            _emptyObjects = new object[0];
            _createContext = delegate { return (ITrialDataContext)_typeConstructor.Invoke(_emptyObjects); };
            _dbContext = _createContext.Invoke();
            
#if DEBUG
            if (iDataContextType.GetInterface("ITrialDataContext")==null)
            {
                throw new ArgumentException("Argument iDataContextType must implement ITrialDataContext");
            }
            if (_typeConstructor==null)
            {
                throw new ArgumentException("iDataContextType must have a constructor which takes no arguments");
            }
#endif
        }
        public Repository(Func<ITrialDataContext> createContext)
        {
            _createContext = createContext;
            _dbContext = _createContext.Invoke();
            _dbBackupPath = _dbContext.DbBackupPath;
        }
        #endregion // Constructors
        #region Members
        private readonly ConstructorInfo _typeConstructor;
        private readonly Object[] _emptyObjects;
        private readonly Func<ITrialDataContext> _createContext;
        private string _dbBackupPath;
        private string _zipPath;
        private ITrialDataContext _dbContext;
        #endregion // Members

        #region Properties
        public event EventHandler<ParticipantEventArgs> ParticipantAdded;
        public event EventHandler<ScreenedPatientEventArgs> ScreenedPatientAdded;
        public event EventHandler<ParticipantEventArgs> ParticipantUpdated;
        //public event EventHandler<ScreenedPatientEventArgs> ScreenedPatientUpdated;
        public string BackupDirectory 
        { 
            get
            {
                return Path.GetDirectoryName(_zipPath);
            }
            set 
            {
                if (!Directory.Exists(value)) {throw new ArgumentException("specified directory does not exist");}
                _zipPath = value + '\\' + Path.GetFileNameWithoutExtension(_dbBackupPath) + ".zip";
            } 
        }
        public string BackupFilePath { get { return _zipPath; } }
        public DbSet<Vaccine> Vaccines
        {
            get
            {
                return _dbContext.Vaccines;
            }
        }
        public DbSet<Participant> Participants
        {
            get
            {
                return _dbContext.Participants;
            }
        }
        public DbSet<ScreenedPatient> ScreenedPatients
        {
            get
            {
                return _dbContext.ScreenedPatients;
            }
        }
        public DbSet<VaccineAdministered> VaccinesAdministered
        {
            get
            {
                return _dbContext.VaccinesAdministered;
            }
        }
        #endregion // Properties

        #region Methods
        public void Add(Participant patient)
        {
            _dbContext.Participants.Add(patient);
            _dbContext.SaveChanges();
            if (this.ParticipantAdded != null)
            {
                this.ParticipantAdded(this, new ParticipantEventArgs(patient));
            }
        }
        public void Add(ScreenedPatient patient)
        {
            _dbContext.ScreenedPatients.Add(patient);
            _dbContext.SaveChanges();
            if (this.ParticipantAdded != null)
            {
                this.ScreenedPatientAdded(this, new ScreenedPatientEventArgs(patient));
            }
        }
        public void Update(int id,
                CauseOfDeathOption causeOfDeath,
                String bcgAdverseDetail,
                bool? bcgAdverse,
                bool? bcgPapule,
                int? lastContactWeight,
                DateTime? lastWeightDate,
                DateTime? dischargeDateTime,
                DateTime? deathOrLastContactDateTime,
                OutcomeAt28DaysOption outcomeAt28Days)
        {
            Participant participant = _dbContext.Participants.Find(id);
            participant.CauseOfDeath = causeOfDeath;
            participant.BcgAdverseDetail = bcgAdverseDetail;
            participant.BcgAdverse = bcgAdverse;
            participant.BcgPapule = bcgPapule;
            participant.LastContactWeight = lastContactWeight;
            participant.LastWeightDate = lastWeightDate;
            participant.DischargeDateTime = dischargeDateTime;
            participant.DeathOrLastContactDateTime = deathOrLastContactDateTime;
            participant.OutcomeAt28Days = outcomeAt28Days;
            _dbContext.Participants.Attach(participant);
            _dbContext.SaveChanges();
            if (this.ParticipantUpdated != null)
            {
                this.ParticipantUpdated(this, new ParticipantEventArgs(participant));
            }
        }
        public void ClearParticipantVaccines(int participantId)
        {
            try
            {
                _dbContext.Database.ExecuteSqlCommand(
                    string.Format("Delete from {0} where ParticipantId={1}", VaccineAdministered.VaccineAdminTableName ,participantId));
            }
            catch(System.Data.Common.DbException e)
            {
                if (e.Message.IndexOf("table does not exist", 0, StringComparison.InvariantCultureIgnoreCase)==-1)
                {
                    throw;
                }
            }
        }
        public void AddOrUpdate(IEnumerable<VaccineAdministered> vaccinesAdministered)
        {
            foreach(var v in vaccinesAdministered)
            {
                if (v.Id==0)
                {
                    _dbContext.VaccinesAdministered.Add(v);
                }
                else
                {
                    _dbContext.VaccinesAdministered.Attach(v);
                }
            }
            _dbContext.SaveChanges();
        }
        public void Update(IEnumerable<Participant> patients)
        {
            foreach (Participant p in patients)
            {
                _dbContext.Participants.Attach(p);
            }
            _dbContext.SaveChanges();
        }
        public void Update(ScreenedPatient patient)
        {
            _dbContext.ScreenedPatients.Attach(patient);
            _dbContext.SaveChanges();
        }
        public void Backup()
        {
            //to do - use static classes to check if not ce & use sql to create .bak file
            BackupCe();
        }
        private void BackupCe()
        {
            if (string.IsNullOrEmpty(BackupFilePath))
            {
                throw new InvalidOperationException("Cannot call backup method without setting backup directory property");
            }
            var dbFile = new FileInfo(_dbBackupPath);
            if (!dbFile.Exists) { return; }
            var backupFile = new FileInfo(BackupFilePath);
            if (backupFile.Exists && backupFile.LastWriteTime >= dbFile.LastWriteTime)
            {
                return;
            }
            _dbContext.Dispose();
            using (ZipFile zip = new ZipFile())
            {
                zip.AddFile(dbFile.FullName);
                zip.Save(backupFile.FullName);
            }
            _dbContext = _createContext.Invoke();
        }
        public void Restore()
        {
            if (string.IsNullOrEmpty(BackupFilePath))
            {
                throw new InvalidOperationException("Cannot call backup method without setting backup directory property");
            }
            var backupFile = new FileInfo(BackupFilePath);
            if (!backupFile.Exists) { throw new FileNotFoundException("Database file not found", _dbBackupPath); }
            var dbFile = new FileInfo(_dbBackupPath);
            if (dbFile.Exists && backupFile.LastWriteTime <= dbFile.LastWriteTime)
            {
                return;
            }
            _dbContext.Dispose();
            using (ZipFile zip1 = ZipFile.Read(backupFile.FullName))
            {
                // here, we extract every entry, but we could extract conditionally
                // based on entry name, size, date, checkbox status, etc.  
                zip1[0].Extract(dbFile.FullName, ExtractExistingFileAction.OverwriteSilently);
            }
            _dbContext = _createContext.Invoke();
        }
        public ParticipantsSummary GetParticipantSummary()
        {
            return new ParticipantsSummary
            {
                TotalCount = _dbContext.Participants.Count(),
                InterventionArmCount = _dbContext.Participants.Count(p => p.IsInterventionArm),
                CompletedRecordCount = _dbContext.Participants.Select(ParticipantModel.GetDataRequired()).Count(d => d == DataRequiredOption.Complete)
            };
        }
        public ScreenedPatientsSummary GetScreenedPatientSummary()
        {
            return new ScreenedPatientsSummary
            {
                TotalCount = _dbContext.ScreenedPatients.Count(),
                BadInfectnImmuneCount = _dbContext.ScreenedPatients.Count(s => s.BadInfectnImmune),
                BadMalformCount = _dbContext.ScreenedPatients.Count(s => s.BadMalform),
                LikelyDie24HrCount = _dbContext.ScreenedPatients.Count(s => s.LikelyDie24Hr),
                MissedCount = _dbContext.ScreenedPatients.Count(s => s.Missed),
                RefusedConsentCount = _dbContext.ScreenedPatients.Count(s => s.RefusedConsent),
                WasGivenBcgPriorCount = _dbContext.ScreenedPatients.Count(s => s.WasGivenBcgPrior)
            };
        }
        #endregion // Methods

        #region IDisposable Implementation
        //http://msdn.microsoft.com/en-us/library/vstudio/b1yfkh5e%28v=vs.100%29.aspx
        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }
        ~Repository()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_dbContext != null) { _dbContext.Dispose(); }
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
        #endregion // IDiposable

    }
}
