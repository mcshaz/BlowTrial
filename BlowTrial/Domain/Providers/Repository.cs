using BlowTrial.Domain.Interfaces;
using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Data.Entity.Migrations;
using BlowTrial.Infrastructure.Exceptions;
using System.Configuration;

namespace BlowTrial.Domain.Providers
{
    public class Repository : IRepository
    {
        #region Constructors
        public Repository(Type iDataContextType) :this(TypeToConstructor(iDataContextType))
        {
            
#if DEBUG
            if (iDataContextType.GetInterface("ITrialDataContext")==null)
            {
                throw new ArgumentException("Argument iDataContextType must implement ITrialDataContext");
            }
#endif
        }
        public Repository(Func<ITrialDataContext> createContext)
        {
            _createContext = createContext;
            _dbContext = _createContext.Invoke(); //keep connection alive
            ZipExtractionDirectory = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
        }
        static Func<ITrialDataContext> TypeToConstructor(Type iDataContextType)
        {
            ConstructorInfo typeConstructor = iDataContextType.GetConstructor(System.Type.EmptyTypes);
#if DEBUG
            if (typeConstructor == null)
            {
                throw new ArgumentException("iDataContextType must have a constructor which takes no arguments");
            }
#endif
            object[] _emptyObjects = new object[0];
            return delegate { return (ITrialDataContext)typeConstructor.Invoke(_emptyObjects); };
        }
        #endregion // Constructors
        #region Members
        private readonly Func<ITrialDataContext> _createContext;

        private ITrialDataContext _dbContext;
        #endregion // Members

        #region Properties
        public event EventHandler<ParticipantEventArgs> ParticipantAdded;
        public event EventHandler<ScreenedPatientEventArgs> ScreenedPatientAdded;
        public event EventHandler<ParticipantEventArgs> ParticipantUpdated;
        //public event EventHandler<ScreenedPatientEventArgs> ScreenedPatientUpdated;
        public IEnumerable<string> CloudDirectories
        {
            get;
            set;
        }

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
        int _studyCentreIdIncrement;
        int StudyCentreIdIncrement
        {
            get
            {
                if (_studyCentreIdIncrement == 0)
                {
                    _studyCentreIdIncrement = int.Parse(ConfigurationManager.AppSettings["StudyCentreIdIncrement"]);
                }
                return _studyCentreIdIncrement;
            }
        }
        IEnumerable<StudyCentreModel> _localStudyCentres;
        public IEnumerable<StudyCentreModel> LocalStudyCentres 
        {
            get 
            {
                if (_localStudyCentres == null)
                {
                    var studyCentres = _dbContext.StudyCentres.ToArray();
                        _localStudyCentres = studyCentres.Select(s => new StudyCentreModel
                         {
                             Id = s.Id,
                             Name = s.Name,
                             ArgbTextColour = s.ArgbTextColour,
                             ArgbBackgroundColour = s.ArgbBackgroundColour,
                             HospitalIdentifierMask = s.HospitalIdentifierMask,
                             PhoneMask = s.PhoneMask
                         });
                }
                return _localStudyCentres;
            }
        }
        public string ZipExtractionDirectory { get; set; } //set to datadirectory on instantiation
        #endregion // Properties

        #region Methods
        public void Add(Participant participant)
        {
            participant.Id = GetNextId(_dbContext.Participants, participant.CentreId);
            _dbContext.Participants.Add(participant);

            _dbContext.SaveChanges();
            if (this.ParticipantAdded != null)
            {
                this.ParticipantAdded(this, new ParticipantEventArgs(participant));
            }
        }
        public void Add(ScreenedPatient patient)
        {
            _dbContext.ScreenedPatients.Add(patient);
            patient.Id = GetNextId(_dbContext.Participants, patient.CentreId);

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
            participant.RecordLastModified = DateTime.UtcNow;
            _dbContext.Participants.Attach(participant);
            _dbContext.SaveChanges();
            if (this.ParticipantUpdated != null)
            {
                this.ParticipantUpdated(this, new ParticipantEventArgs(participant));
            }
        }

        public void AddOrUpdate(IEnumerable<VaccineAdministered> vaccinesAdministered)
        {
            foreach (var v in vaccinesAdministered)
            {
                if (v.Id == 0)
                {
                    v.Id = GetNextId(_dbContext.VaccinesAdministered, (v.ParticipantId / StudyCentreIdIncrement) * StudyCentreIdIncrement);
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
            string bakFileName = _dbContext.BackupDb();
            FileInfo bakFile = new FileInfo(bakFileName);
            if (!bakFile.Exists) { return; }
#if DEBUG
            if (!(CloudDirectories.Count() == 1))
            {
                throw new InvalidOperationException("Backup called without cloud directories set to a single directory");
            }
#endif
            string cloudPathWithoutExtension = CloudDirectories.First() + '\\' + Path.GetFileNameWithoutExtension(bakFileName) + '_' + LocalStudyCentres.First().Id.ToString();
            string cloudZipName = cloudPathWithoutExtension + ".zip"; 
            
            var cloudFile = new FileInfo(cloudZipName);
            if (cloudFile.Exists && cloudFile.LastWriteTimeUtc >= _dbContext.DbLastModifiedUtc())
            {
                return;
            }
            _dbContext.Dispose(); // only necessary for ce
            string cloudBakName = cloudPathWithoutExtension + Path.GetExtension(bakFileName);
            using (ZipFile zip = new ZipFile())
            {
                zip.AddFile(bakFile.FullName, "").FileName = cloudBakName;
                zip.Save(cloudFile.FullName);
            }
            _dbContext = _createContext.Invoke();
        }

        public void Restore()
        {
            DateTime RecordLastModified = _dbContext.DbLastModifiedUtc(); //note this is all assuming user on backup end has no ability to modify data
            var zipFiles = GetFilesUpdatedAfter(RecordLastModified, _dbContext.DbName);
            if (!zipFiles.Any()) { return; }
            _dbContext.Dispose();

            List<string> bakFileNames = new List<string>(zipFiles.Count);
            foreach (var zipFile in zipFiles)
            {
                using (ZipFile readFile = ZipFile.Read(zipFile.FullName))
                {
                    //may want to check lastaccessed to check against as well but not sure if UTC - imperative for international data
                    readFile[0].Extract(ZipExtractionDirectory, ExtractExistingFileAction.OverwriteSilently);
                    bakFileNames.Add(readFile[0].FileName);
                }
            }

            _dbContext = _createContext.Invoke();
            AddOrUpdateBaks(bakFileNames, RecordLastModified);

        }
        void AddOrUpdateBaks(IEnumerable<string> bakFileNames, DateTime addOrUpdateAfter)
        {
            List<StudyCentre> knownSites = _dbContext.StudyCentres.ToList();
            foreach (string fn in bakFileNames)
            {
                using (var downloadedDb = _dbContext.AttachDb(ZipExtractionDirectory + '\\' + fn))
                {
                    var knownSiteIds = knownSites.Select(s=>s.DuplicateIdCheck);
                    var newSites = downloadedDb.StudyCentres.Where(s=>!knownSiteIds.Contains(s.DuplicateIdCheck));
                    foreach (StudyCentre s in newSites)
                    {
                        if (knownSites.Any(k=>k.Id == s.Id))
                        {
                            throw new DuplicateDataKeyException("Duplicate key for site Id:" + s.Id.ToString());
                        }
                        _dbContext.StudyCentres.Attach(s);
                        knownSites.Add(s);
                    }
                    _dbContext.Participants.AddOrUpdate(downloadedDb.Participants.Where(p => p.RecordLastModified > addOrUpdateAfter).ToArray());
                    _dbContext.ScreenedPatients.AddOrUpdate(downloadedDb.ScreenedPatients.Where(p => p.RecordLastModified > addOrUpdateAfter).ToArray());
                    _dbContext.Vaccines.AddOrUpdate(downloadedDb.Vaccines.Where(p => p.RecordLastModified > addOrUpdateAfter).ToArray());
                    _dbContext.VaccinesAdministered.AddOrUpdate(downloadedDb.VaccinesAdministered.Where(p => p.RecordLastModified > addOrUpdateAfter).ToArray());
                    _dbContext.ProtocolViolations.AddOrUpdate(downloadedDb.ProtocolViolations.Where(p => p.RecordLastModified > addOrUpdateAfter).ToArray());
                    _dbContext.SaveChanges();
                }
            }
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
                RefusedConsentCount = _dbContext.ScreenedPatients.Count(s => s.RefusedConsent==true),
                WasGivenBcgPriorCount = _dbContext.ScreenedPatients.Count(s => s.WasGivenBcgPrior)
            };
        }

        int GetNextId(IQueryable<ISharedRecord> recordSet, int startingIndex)
        {
            int endIndex = startingIndex + StudyCentreIdIncrement;
            int returnVar = (from r in recordSet
                             where r.Id >= startingIndex && r.Id < endIndex
                             select r.Id).DefaultIfEmpty().Max();
            if (returnVar == 0) { returnVar = startingIndex; }
            else 
            { 
                returnVar++; 
                if (returnVar == endIndex)
                {
                    throw new DataKeyOutOfRangeException("Database has exceeded maximum size for site");
                }
            }
            return returnVar;
        }

        #endregion // Methods

        ICollection<FileInfo> GetFilesUpdatedAfter(DateTime afterUtc, string filePrefix)
        {
            int prefixLen = filePrefix.Length;
            List<FileInfo> returnVar = new List<FileInfo>();
            foreach (string dirName in CloudDirectories)
            {
                DirectoryInfo di = new DirectoryInfo(dirName);
                foreach (FileInfo f in di.GetFiles())
                {
                    if (f.LastWriteTimeUtc > afterUtc && f.Name.Substring(0,prefixLen)==filePrefix)
                    {
                        returnVar.Add(f);
                    }
                }
            }
            return returnVar;
        }
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
