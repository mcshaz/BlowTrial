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
using System.Data.Entity.Migrations;
using BlowTrial.Infrastructure.Exceptions;
using System.Data.Entity.Infrastructure;
using BlowTrial.Helpers;
using LinqKit;

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
        private const string BakExtension = ".sdf"; //obviously change if using non-compact
        #endregion // Members

        #region Properties
        public event EventHandler<ParticipantEventArgs> ParticipantAdded;
        public event EventHandler<ScreenedPatientEventArgs> ScreenedPatientAdded;
        public event EventHandler<ProtocolViolationEventArgs> ProtocolViolationAdded;
        public event EventHandler<ParticipantEventArgs> ParticipantUpdated;
        //public event EventHandler<ScreenedPatientEventArgs> ScreenedPatientUpdated;
        public IEnumerable<string> CloudDirectories
        {
            get;
            set;
        }

        public DbQuery<Vaccine> Vaccines
        {
            get
            {
                return _dbContext.Vaccines.AsNoTracking();
            }
        }
        public DbQuery<Participant> Participants
        {
            get
            {
                return _dbContext.Participants.AsNoTracking();
            }
        }
        public DbQuery<ScreenedPatient> ScreenedPatients
        {
            get
            {
                return _dbContext.ScreenedPatients.AsNoTracking();
            }
        }
        public DbQuery<VaccineAdministered> VaccinesAdministered
        {
            get
            {
                return _dbContext.VaccinesAdministered.AsNoTracking();
            }
        }
        public DbQuery<ProtocolViolation> ProtocolViolations
        {
            get
            {
                return _dbContext.ProtocolViolations.AsNoTracking();
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
                            PhoneMask = s.PhoneMask,
                            MaxIdForSite = s.MaxIdForSite,
                            DuplicateIdCheck = s.DuplicateIdCheck // needed for backup
                        });
                }
                return _localStudyCentres;
            }
        }
        public string ZipExtractionDirectory { get; set; } //set to datadirectory on instantiation
        #endregion // Properties

        #region Methods
        public Participant FindParticipant(int participantId)
        {
            return _dbContext.Participants.Find(participantId);
        }
        public ProtocolViolation FindViolation(int violationId)
        {
            return _dbContext.ProtocolViolations.Find(violationId);
        }
        public void Add(Participant participant)
        {
            var centre= LocalStudyCentres.First(c=>c.Id == participant.CentreId);
            if (participant.Id == 0)
            {
                participant.Id = GetNextId(_dbContext.Participants, participant.CentreId);
            }
            if (participant.Id < centre.Id)
            {
                throw new DataKeyOutOfRangeException("participant id less than id for site");
            }
            else if (participant.Id > centre.MaxIdForSite)
            {
                throw new DataKeyOutOfRangeException("participant id greater than maximum allocations for site");
            }

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
            patient.Id = GetNextId(_dbContext.ScreenedPatients, patient.CentreId);

            _dbContext.SaveChanges();
            if (this.ParticipantAdded != null)
            {
                this.ScreenedPatientAdded(this, new ScreenedPatientEventArgs(patient));
            }
        }
        public void AddOrUpdate(IEnumerable<StudyCentre> centres)
        {
            foreach (StudyCentre s in centres)
            {
                _dbContext.StudyCentres.AddOrUpdate(s);
            }
            _dbContext.SaveChanges();
            _localStudyCentres = null;
        }
        public void AddOrUpdate(ProtocolViolation violation)
        {
            if (violation.ParticipantId == 0)
            {
                throw new ArgumentException("Participant Id must have a value");
            }
            if (violation.Id==0)
            { 
                int centreId = (from p in _dbContext.Participants
                                where p.Id == violation.ParticipantId
                                select p.CentreId).First();
                violation.Id = GetNextId(_dbContext.ProtocolViolations, centreId);
                violation.ReportingTimeLocal = DateTime.Now;
                violation.ReportingInvestigator = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                _dbContext.ProtocolViolations.Add(violation);
                if (this.ProtocolViolationAdded != null)
                {
                    this.ProtocolViolationAdded(this, new ProtocolViolationEventArgs(violation));
                }
            }
            else
            {
                ProtocolViolation attachedViol = _dbContext.ProtocolViolations.Local.FirstOrDefault(v => v.Id == violation.Id);
                if (attachedViol == null)
                {
                    _dbContext.ProtocolViolations.Attach(violation);
                }
                else
                {
                    _dbContext.Entry(attachedViol).CurrentValues.SetValues(violation);
                }
                _dbContext.Entry(violation).State = EntityState.Modified;
            }
            _dbContext.SaveChanges();
        }

        public void UpdateParticipant(int id,
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
            Participant attachedParticipant = _dbContext.Participants.Local.FirstOrDefault(p => p.Id == participant.Id);
            if (attachedParticipant == null)
            {
                _dbContext.Participants.Attach(participant);
            }
            else
            {
                _dbContext.Entry(attachedParticipant).CurrentValues.SetValues(participant);
            }
            _dbContext.Entry(participant).State = EntityState.Modified;
            _dbContext.SaveChanges();
            if (this.ParticipantUpdated != null)
            {
                this.ParticipantUpdated(this, new ParticipantEventArgs(participant));
            }
        }
        /// <summary>
        /// Adds or updates all vaccines attached to participant.VaccinesAdministered
        /// </summary>
        /// <param name="vaccinesAdministered"></param>
        public void AddOrUpdateVaccinesFor(int participantId, IEnumerable<VaccineAdministered> vaccinesAdministered)
        {
            var includedVaccineAdministeredIds = (from v in vaccinesAdministered
                                                  where v.Id != 0
                                                  select v.Id);
            var removeVaccineAdministeredIds = (from v in _dbContext.VaccinesAdministered
                                                where v.ParticipantId == participantId && !includedVaccineAdministeredIds.Contains(v.Id)
                                                select v.Id).ToArray();
            if (removeVaccineAdministeredIds.Any())
            {
                string sqlString = string.Format("DELETE FROM {0} WHERE Id IN ({1})",
                    VaccineAdministered.VaccineAdminTableName,
                    string.Join(",", removeVaccineAdministeredIds));
                _dbContext.Database.ExecuteSqlCommand(sqlString);
            }
            int studyCentreId = (from p in _dbContext.Participants
                                 where p.Id == participantId
                                 select p.CentreId).First();
            foreach (var v in vaccinesAdministered)
            {
                v.ParticipantId = participantId;
                if (v.Id == 0)
                {
                    v.Id = GetNextId(_dbContext.VaccinesAdministered, studyCentreId);
                    _dbContext.VaccinesAdministered.Add(v);
                }
                else
                {
                    VaccineAdministered attachedVA = _dbContext.VaccinesAdministered.Local.FirstOrDefault(vax => vax.Id == v.Id);
                    if (attachedVA == null)
                    {
                        _dbContext.VaccinesAdministered.Attach(v);
                    }
                    else
                    {
                        _dbContext.Entry(attachedVA).CurrentValues.SetValues(v);
                    }
                    _dbContext.Entry(v).State = EntityState.Modified;
                }
            }
            _dbContext.SaveChanges();
        }
        public void Add(Vaccine newVaccine)
        {
            newVaccine.Id = GetNextId(_dbContext.Vaccines, LocalStudyCentres.First().Id);
            _dbContext.Vaccines.Add(newVaccine);
            _dbContext.SaveChanges();
        }
        public void Update(Participant patient)
        {
            Update(new Participant[] { patient });
        }
        public void Update(IEnumerable<Participant> patients)
        {
            if (patients.Any(p=>p.Id==0))
            {
                throw new DataKeyOutOfRangeException("all patients for update must have an Id != 0");
            }
            var allIds = patients.Select(p=>p.Id);
            var allParticipants = (from p in _dbContext.Participants
                                   where allIds.Contains(p.Id)
                                   select p).ToList();
            foreach (Participant p in patients)
            {
                _dbContext.Entry(allParticipants.First(a=>a.Id == p.Id)).CurrentValues.SetValues(p);
            }
            _dbContext.SaveChanges();
        }
        public void Update(ScreenedPatient patient)
        {
            ScreenedPatient attachedPatient = _dbContext.ScreenedPatients.Local.FirstOrDefault(p => p.Id == patient.Id);
            if (attachedPatient == null)
            {
                _dbContext.ScreenedPatients.Attach(patient);
            }
            else
            {
                _dbContext.Entry(attachedPatient).CurrentValues.SetValues(patient);
            }
            _dbContext.Entry(patient).State = EntityState.Modified;
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
            string cloudPathWithoutExtension = Path.GetFileNameWithoutExtension(bakFileName) + '_' + LocalStudyCentres.First().DuplicateIdCheck.ToString("N");
            string cloudZipName = CloudDirectories.First() + '\\' + cloudPathWithoutExtension + ".zip"; 
            
            var cloudFile = new FileInfo(cloudZipName);

            if (cloudFile.Exists)
            {
                DateTime? mostRecentEntry = (new DateTime?[]
                    {
                        _dbContext.Participants.Max(p=>(DateTime?)p.RecordLastModified),
                        _dbContext.ScreenedPatients.Max(s=>(DateTime?)s.RecordLastModified),
                        _dbContext.Vaccines.Max(v=>(DateTime?)v.RecordLastModified),
                        _dbContext.VaccinesAdministered.Max(va=>(DateTime?)va.RecordLastModified),
                        _dbContext.ProtocolViolations.Max(pv=>(DateTime?)pv.RecordLastModified),
                        _dbContext.StudyCentres.Max(s=>(DateTime?)s.RecordLastModified)
                    }).Max();
                if (mostRecentEntry==null || cloudFile.LastWriteTimeUtc >= mostRecentEntry)
                {
                    return;
                }
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
            //DateTime RecordLastModified = _dbContext.DbLastModifiedUtc(); //note this is all assuming user on backup end has no ability to modify data
            IEnumerable<MatchedFilePair> filePairs = GetMatchedCloudAndExtractedFiles().Where(fp => fp.ExtractedBak == null || fp.ExtractedBak.CreationTimeUtc < fp.Zip.LastWriteTimeUtc);
            if (!filePairs.Any()) { return; }
            _dbContext.Dispose();

            foreach (var fp in filePairs)
            {
                using (ZipFile readFile = ZipFile.Read(fp.Zip.FullName))
                {
                    readFile[0].Extract(ZipExtractionDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
                if (fp.ExtractedBak == null)
                {
                    fp.ExtractedBak = new FileInfo(Path.Combine(ZipExtractionDirectory, Path.GetFileNameWithoutExtension(fp.Zip.Name) + BakExtension));
                }
                fp.ExtractedBak.CreationTimeUtc = fp.Zip.LastWriteTimeUtc;
            }

            _dbContext = _createContext.Invoke();
            AddOrUpdateBaks(filePairs.Select(fp=>fp.ExtractedBak));

        }
        void AddOrUpdateBaks(IEnumerable<FileInfo> bakupFiles)
        {
            List<StudyCentre> knownSites = _dbContext.StudyCentres.ToList();
            List<IntegerRange> newSiteIdRanges = new List<IntegerRange>();
            foreach (var f in bakupFiles)
            {
                IEnumerable<Guid> knownSiteIds = knownSites.Select(s => s.DuplicateIdCheck);
                using (var downloadedDb = _dbContext.AttachDb(f.FullName))
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
                        _dbContext.StudyCentres.Add(s); //because context will have been renewed, this should not cause a duplicate key exception
                        knownSites.Add(s);
                        newSiteIdRanges.Add(new IntegerRange(s.Id, s.MaxIdForSite));
                        _localStudyCentres = null;
                    }
                    var newSiteIds = newSiteIdRanges.Select(n => n.Min);
                    DateTime mostRecentBak = f.CreationTimeUtc;
                    _dbContext.Participants.AddOrUpdate((from p in downloadedDb.Participants.AsNoTracking()
                                                         where p.RecordLastModified > mostRecentBak || newSiteIds.Contains(p.CentreId)
                                                         select p).ToArray());
                    _dbContext.ScreenedPatients.AddOrUpdate((from s in downloadedDb.ScreenedPatients.AsNoTracking()
                                                             where s.RecordLastModified > mostRecentBak || newSiteIds.Contains(s.CentreId)
                                                             select s).ToArray());
                    
                    var vaccinePredicate = PredicateBuilder.False<Vaccine>();
                    vaccinePredicate.Or(p => p.RecordLastModified > mostRecentBak);
                    foreach (IntegerRange rng in newSiteIdRanges)
                    {
                        vaccinePredicate = vaccinePredicate.Or(p => p.Id >= rng.Min && p.Id <= rng.Max);
                    }
                    var newVax = _dbContext.Vaccines.AsNoTracking().AsExpandable().Where(vaccinePredicate).ToArray();
                    _dbContext.Vaccines.AddOrUpdate(newVax);

                    var vaccineAdminPredicate = PredicateBuilder.False<VaccineAdministered>();
                    vaccineAdminPredicate.Or(p => p.RecordLastModified > mostRecentBak);
                    foreach (IntegerRange rng in newSiteIdRanges)
                    {
                        vaccineAdminPredicate = vaccineAdminPredicate.Or(p => p.Id >= rng.Min && p.Id <= rng.Max);
                    }
                    var newVaxAdmin = _dbContext.VaccinesAdministered.AsNoTracking().AsExpandable().Where(vaccineAdminPredicate).ToArray();
                    _dbContext.VaccinesAdministered.AddOrUpdate(newVaxAdmin);

                    var protocolViolPredicate = PredicateBuilder.False<ProtocolViolation>();
                    protocolViolPredicate.Or(p => p.RecordLastModified > mostRecentBak);
                    foreach (IntegerRange rng in newSiteIdRanges)
                    {
                        protocolViolPredicate = protocolViolPredicate.Or(p => p.Id >= rng.Min && p.Id <= rng.Max);
                    }
                    var newProtocolViol = _dbContext.ProtocolViolations.AsNoTracking().AsExpandable().Where(protocolViolPredicate).ToArray();
                    _dbContext.ProtocolViolations.AddOrUpdate(newProtocolViol);

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
                MissedCount = _dbContext.ScreenedPatients.Count(s => s.Missed==true),
                RefusedConsentCount = _dbContext.ScreenedPatients.Count(s => s.RefusedConsent==true),
                WasGivenBcgPriorCount = _dbContext.ScreenedPatients.Count(s => s.WasGivenBcgPrior)
            };
        }

        int GetNextId(IQueryable<ISharedRecord> recordSet, int studyCentreId)
        {
            int maxIdForSite = LocalStudyCentres.First(s=>s.Id == studyCentreId).MaxIdForSite;
            int returnVar = (from r in recordSet
                             where r.Id >= studyCentreId && r.Id <= maxIdForSite
                             select r.Id).DefaultIfEmpty().Max();
            if (returnVar == 0) { returnVar = studyCentreId; }
            returnVar++; 
            if (returnVar > maxIdForSite)
            {
                throw new DataKeyOutOfRangeException("Database has exceeded maximum size for site");
            }
            return returnVar;
        }

        ICollection<MatchedFilePair> GetMatchedCloudAndExtractedFiles()
        {
            string filePrefix = _dbContext.DbName;
            int prefixLen = filePrefix.Length;
            int fnLen = prefixLen + 37;

            List<MatchedFilePair> returnVar = new List<MatchedFilePair>();

            DirectoryInfo di;
            foreach (string dirName in CloudDirectories)
            {
                di = new DirectoryInfo(dirName);
                returnVar.AddRange(from f in di.GetFiles()
                                   where f.Name.Length == fnLen && f.Name.Substring(0, prefixLen) == filePrefix
                                   select new MatchedFilePair{ Zip = f });
            }

            di = new DirectoryInfo(ZipExtractionDirectory);
            foreach (FileInfo f in di.GetFiles())
            {
                if (f.Name.Length == fnLen && f.Name.Substring(0, prefixLen) == filePrefix)
                {
                    var matchedPair = returnVar.FirstOrDefault(r => Path.GetFileNameWithoutExtension(r.Zip.Name) == Path.GetFileNameWithoutExtension(f.Name));
                    if (matchedPair != null)
                    {
                        matchedPair.ExtractedBak = f;
                    }
                }
            }

            return returnVar;
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

        #region FileModifiedInfo
        private class MatchedFilePair
        {
            FileInfo _zip;
            FileInfo _extractedBak;
            internal FileInfo Zip 
            {
                get { return _zip; }
                set 
                { 
                    if (!value.Name.EndsWith(".zip"))
                    {
                        throw new InvalidFileTypeException(Path.GetExtension(value.Name),".zip");
                    }
                    _zip = value;
                }
            }
            internal FileInfo ExtractedBak 
            {
                get { return _extractedBak; }
                set
                {
                    if (!value.Name.EndsWith(BakExtension))
                    {
                        throw new InvalidFileTypeException(Path.GetExtension(value.Name), BakExtension);
                    }
                    _extractedBak = value;
                }
            }
        }
        #endregion

    }
}
