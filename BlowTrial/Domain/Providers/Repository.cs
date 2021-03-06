﻿using BlowTrial.Domain.Interfaces;
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
using LinqKit;
using BlowTrial.Infrastructure;
using BlowTrial.Infrastructure.Randomising;
using BlowTrial.Migrations;
using BlowTrial.Infrastructure.Extensions;
using BlowTrial.Helpers;
using System.Threading;
using System.ComponentModel;

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
        private bool _isBackingUp;
        private List<string> _baksForgoingMigration;
        #endregion // Members

        #region EventHandlers
        public event EventHandler<ParticipantEventArgs> ParticipantAdded;
        public event EventHandler<ScreenedPatientEventArgs> ScreenedPatientAdded;
        public event EventHandler<ProtocolViolationEventArgs> ProtocolViolationAddOrUpdate;
        public event EventHandler<ParticipantEventArgs> ParticipantUpdated;
        public event EventHandler<LastUpdatedChangedEventAgs> AnyParticipantChange;
        public event EventHandler<FailedRestoreEvent> FailedDbRestore;
        public event EventHandler<DatabaseUpdatingEventAgs> DatabaseUpdating;
        public event EventHandler StudySiteAddOrUpdate;
        public event ProgressChangedEventHandler UpdateProgress;
        //public event EventHandler<ScreenedPatientEventArgs> ScreenedPatientUpdated;
        #endregion // EventHandlers

        #region Properties
        public Database Database
        {
            get { return _dbContext.Database; }
        }

        public IEnumerable<string> CloudDirectories
        {
            get;
            set;
        }
        List<Vaccine> _vaccines;
        public IEnumerable<Vaccine> Vaccines
        {
            get
            {
                return _vaccines ?? (_vaccines = _dbContext.Vaccines.AsNoTracking().ToList());
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
        public DbQuery<UnsuccessfulFollowUp> UnsuccessfulFollowUps
        {
            get
            {
                return _dbContext.UnsuccessfulFollowUps.AsNoTracking();
            }
        }
        public DbQuery<ProtocolViolation> ProtocolViolations
        {
            get
            {
                return _dbContext.ProtocolViolations.AsNoTracking();
            }
        }
        Dictionary<int, StudyCentreModel> _localStudyCentreDictionary;
        Dictionary<int, StudyCentreModel> LocalStudyCentreDictionary 
        {
            get 
            {
                if (_localStudyCentreDictionary == null)
                {
                    _localStudyCentreDictionary = (from s in _dbContext.StudyCentres.AsEnumerable() 
                                                   select new StudyCentreModel
                                                    {
                                                        Id = s.Id,
                                                        Name = s.Name,
                                                        IsCurrentlyEnrolling = s.IsCurrentlyEnrolling,
                                                        ArgbTextColour = s.ArgbTextColour,
                                                        ArgbBackgroundColour = s.ArgbBackgroundColour,
                                                        HospitalIdentifierMask = s.HospitalIdentifierMask,
                                                        PhoneMask = s.PhoneMask,
                                                        MaxIdForSite = s.MaxIdForSite,
                                                        DuplicateIdCheck = s.DuplicateIdCheck, // needed for backup
                                                        RandomisedMessage = new RandomisingMessages(s.IsOpvInIntervention,s.IsToHospitalDischarge),
                                                        DefaultAllocation = s.DefaultAllocation
                                                    }).ToDictionary(scm=>scm.Id);
                }
                return _localStudyCentreDictionary;
            }
        }

        public ICollection<StudyCentreModel> LocalStudyCentres
        {
            get { return LocalStudyCentreDictionary.Values; }
        }

        public StudyCentreModel FindStudyCentre(int studyCentreId)
        {
            return LocalStudyCentreDictionary[studyCentreId];
        }

        #endregion // Properties

        #region Methods
        public Participant FindParticipantAndCollections(int participantId)
        {
            var returnVar = _dbContext.Participants.Find(participantId);
            returnVar.VaccinesAdministered = _dbContext.VaccinesAdministered.Where(va => va.ParticipantId == participantId).ToList();
            returnVar.ProtocolViolations = _dbContext.ProtocolViolations.Where(pv => pv.ParticipantId == participantId).ToList();
            return returnVar;
        }
        public Participant FindParticipant(int participantId)
        {
            return _dbContext.Participants.Find(participantId);
        }
        public ProtocolViolation FindViolation(int violationId)
        {
            return _dbContext.ProtocolViolations.Find(violationId);
        }
        public Participant AddParticipant(
            string name,
            string mothersName,
            string hospitalIdentifier,
            int admissionWeight,
            double gestAgeBirth,
            DateTime dateTimeBirth,
            string AdmissionDiagnosis,
            string phoneNumber,
            bool isMale,
            bool? inborn,
            DateTime registeredAt,
            int centreId,
            MaternalBCGScarStatus maternalBCGScar,
            int? multipleSiblingId,
            int? envelopeNumber = null)
        {
            Participant newParticipant = new Participant
            {
                Name = name,
                MothersName = mothersName,
                HospitalIdentifier = hospitalIdentifier.Trim(),
                AdmissionWeight = admissionWeight,
                GestAgeBirth = gestAgeBirth,
                DateTimeBirth = dateTimeBirth,
                AdmissionDiagnosis = AdmissionDiagnosis,
                PhoneNumber =  phoneNumber,
                IsMale = isMale,
                Inborn = inborn,
                RegisteredAt = registeredAt,
                RegisteringInvestigator = System.Threading.Thread.CurrentPrincipal.Identity.Name,
                CentreId = centreId,
                WasEnvelopeRandomised = envelopeNumber.HasValue,
                MaternalBCGScar = maternalBCGScar,
                AppVersionAtEnrollment = App.CurrentAppVersion,
                VaccinesAdministered = new List<VaccineAdministered>()
            };
            if (multipleSiblingId.HasValue)
            {
                var multipleSibling = _dbContext.Participants.Find(multipleSiblingId.Value);
                if (multipleSibling == null)
                {
                    throw new ArgumentException("Participant Not Found", "multipleSiblingId");
                }
                if (multipleSibling.IsMale == newParticipant.IsMale)
                {
                    newParticipant.TrialArm = multipleSibling.TrialArm;
                    newParticipant.MultipleSiblingId = multipleSiblingId;
                    if (envelopeNumber.HasValue)
                    {
                        newParticipant.Id = (from p in _dbContext.Participants
                                             where p.Id > EnvelopeDetails.MaxEnvelopeNumber
                                             orderby p.Id descending
                                             select p.Id).FirstOrDefault();
                        if (newParticipant.Id == 0)
                        {
                            newParticipant.Id = EnvelopeDetails.MaxEnvelopeNumber;
                        }
                        newParticipant.Id += 1;
                    }
                    else
                    {
                        newParticipant.Id = GetNextId(_dbContext.Participants, centreId);
                        Engine.ForceAllocationToArm(newParticipant, _dbContext);
                    }
                }
            }
            if (!newParticipant.MultipleSiblingId.HasValue)
            {
                if (envelopeNumber.HasValue)
                {
                    Envelope envelope = EnvelopeDetails.GetEnvelope(envelopeNumber.Value);
                    if (!_dbContext.AllocationBlocks.Any(a=>a.Id==envelope.BlockNumber))
                    {
                        _dbContext.AllocationBlocks.Add(new AllocationBlock 
                        {
                            Id = envelope.BlockNumber, 
                            GroupRepeats = (byte)(envelope.BlockSize/2), 
                            AllocationGroup = AllocationGroups.India2Arm, 
                            RandomisationCategory = envelope.RandomisationCategory
                        });
                    }
                    newParticipant.AllocationBlockId = envelope.BlockNumber;
                    newParticipant.TrialArm = envelope.IsInterventionArm ? RandomisationArm.RussianBCG : RandomisationArm.Control;
                    newParticipant.Id = envelopeNumber.Value;
                }
                else
                {
                    newParticipant.Id = GetNextId(_dbContext.Participants, centreId);
                    Engine.CreateAllocation(newParticipant, _dbContext);
                }
            }
            Add(newParticipant);
            return newParticipant;
        }

        void Add(Participant participant)
        {
            var centre = FindStudyCentre(participant.CentreId);
            if (participant.Id < centre.Id)
            {
                throw new DataKeyOutOfRangeException("participant id less than id for site");
            }
            else if (participant.Id > centre.MaxIdForSite)
            {
                throw new DataKeyOutOfRangeException("participant id greater than maximum allocations for site");
            }
            if (participant.VaccinesAdministered == null)
            {
                participant.VaccinesAdministered = new List<VaccineAdministered>();
            }
            if (participant.UnsuccessfulFollowUps==null)
            {
                participant.UnsuccessfulFollowUps = new List<UnsuccessfulFollowUp>();
            }
            _dbContext.Participants.Add(participant);

            _dbContext.SaveChanges(true);
            if (this.ParticipantAdded != null)
            {
                participant.ProtocolViolations = new List<ProtocolViolation>();
                this.ParticipantAdded(this, new ParticipantEventArgs(participant));
            }
        }
        public void Add(ScreenedPatient patient)
        {
            _dbContext.ScreenedPatients.Add(patient);
            patient.Id = GetNextId(_dbContext.ScreenedPatients, patient.CentreId);
            patient.AppVersionAtEnrollment = App.CurrentAppVersion;

            _dbContext.SaveChanges(true);
            ScreenedPatientAdded?.Invoke(this, new ScreenedPatientEventArgs(patient));
        }
        public void AddOrUpdate(IEnumerable<StudyCentre> centres)
        {
            foreach (StudyCentre s in centres)
            {
                _dbContext.StudyCentres.AddOrUpdate(s);
            }
            _dbContext.SaveChanges(true);
            _localStudyCentreDictionary = null;
            StudySiteAddOrUpdate?.Invoke(this, EventArgs.Empty);
        }
        public void AddOrUpdate(ProtocolViolation violation)
        {
            if (violation.ParticipantId == 0)
            {
                throw new ArgumentException("Participant Id must have a value");
            }
            bool isToAdd = violation.Id==0;
            if (isToAdd)
            { 
                int centreId = (from p in _dbContext.Participants
                                where p.Id == violation.ParticipantId
                                select p.CentreId).First();
                violation.Id = GetNextId(_dbContext.ProtocolViolations, centreId);
                violation.ReportingTimeLocal = DateTime.Now;
                violation.ReportingInvestigator = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                _dbContext.ProtocolViolations.Add(violation);
            }
            else
            {
                ((DbContext)_dbContext).AttachAndMarkModified(violation);
            }
            _dbContext.SaveChanges(true);
            this.ProtocolViolationAddOrUpdate?.Invoke(this, new ProtocolViolationEventArgs(violation, isToAdd ? CRUD.Created : CRUD.Updated));
        }
        const string BlockRandomisationViolation = "Alteration to data which would have affected randomisation:";
        static string GenderString(bool isMale)
        {
            return isMale ? "Male" : "Female";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="isMale"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="AdmissionDiagnosis"></param>
        /// <param name="admissionWeight"></param>
        /// <param name="dateTimeBirth"></param>
        /// <param name="gestAgeBirth"></param>
        /// <param name="hospitalIdentifier"></param>
        /// <param name="isInborn"></param>
        /// <param name="multipleSibblingId"></param>
        /// <param name="registeredAt"></param>
        /// <returns>Whether a protocol violation has been logged due to randomising criteria being altered</returns>
        public UpdateParticipantViolationType UpdateParticipant(int id,
            string name,
            bool isMale,
            string phoneNumber,
            string AdmissionDiagnosis,
            int admissionWeight,
            DateTime dateTimeBirth,
            double gestAgeBirth,
            string hospitalIdentifier,
            bool? isInborn,
            int? multipleSibblingId,
            MaternalBCGScarStatus maternalBCGScar,
            DateTime registeredAt,
            bool isEnvelopeRandomising)
        {
            Participant participant = _dbContext.Participants.Find(id);
            var pv = new List<ProtocolViolation>();
            UpdateParticipantViolationType returnVar = UpdateParticipantViolationType.NoViolations;
            if (!RandomisingExtensions.IsSameRandomisingCategory(participant.IsMale, isMale, participant.AdmissionWeight, admissionWeight))
            {
                pv.Add(new ProtocolViolation
                {
                    Details = string.Format(BlockRandomisationViolation + "Block randomising information changed from {0}g {1} to {2}g {3}", 
                        participant.AdmissionWeight, 
                        GenderString(participant.IsMale),
                        admissionWeight,
                        GenderString(isMale))
                });
                Engine.RemoveAllocationFromArm(participant);
                returnVar |= UpdateParticipantViolationType.BlockCriteriaChanged;
            }
            if (admissionWeight > Engine.MaxBirthWeightGrams)
            {
                pv.Add(new ProtocolViolation
                {
                    Details = string.Format("Retrospectively inelligible participant!: Weight changed from {0} to {1}", participant.AdmissionWeight, admissionWeight)
                });

                participant.AdmissionWeight = admissionWeight;
                returnVar |= UpdateParticipantViolationType.IneligibleWeight;
            }
            if (participant.MultipleSiblingId != multipleSibblingId)
            {
                pv.Add(new ProtocolViolation
                {
                    Details = string.Format(BlockRandomisationViolation + "Twin/tripplet ID changed from '{0}' to '{1}';", participant.MultipleSiblingId, multipleSibblingId)
                });
                participant.MultipleSiblingId = multipleSibblingId;
                returnVar |= UpdateParticipantViolationType.MultipleSiblingIdChanged;
            }
            int nextPvId = GetNextId(_dbContext.ProtocolViolations, participant.CentreId);
            foreach (var p in pv)
            {
                p.ParticipantId = participant.Id;
                p.Id = nextPvId++;
                p.ViolationType = ViolationTypeOption.MajorWrongAllocation;
                p.ReportingTimeLocal = DateTime.Now;
                p.ReportingInvestigator = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                _dbContext.ProtocolViolations.Add(p);
            }
            participant.Name = name;
            participant.PhoneNumber = phoneNumber;
            participant.AdmissionDiagnosis = AdmissionDiagnosis;
            participant.DateTimeBirth = dateTimeBirth;
            participant.GestAgeBirth = gestAgeBirth;
            participant.HospitalIdentifier = hospitalIdentifier;
            participant.Inborn = isInborn;
            participant.RegisteredAt = registeredAt;
            participant.MaternalBCGScar = maternalBCGScar;
            if ((returnVar & UpdateParticipantViolationType.BlockCriteriaChanged) != 0)
            {
                Engine.ForceAllocationToArm(participant, _dbContext);
            }
            _dbContext.SaveChanges(true);
            this.ParticipantUpdated?.Invoke(this, new ParticipantEventArgs(participant));
            return returnVar;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="causeOfDeath"></param>
        /// <param name="otherCauseOfDeathDetail"></param>
        /// <param name="bcgAdverse"></param>
        /// <param name="bcgAdverseDetail"></param>
        /// <param name="bcgPapuleAtDischarge"></param>
        /// <param name="lastContactWeight"></param>
        /// <param name="lastWeightDate"></param>
        /// <param name="dischargeDateTime"></param>
        /// <param name="deathOrLastContactDateTime"></param>
        /// <param name="outcomeAt28Days"></param>
        /// <param name="notes"></param>
        /// <param name="participantVaccines">if null (or ommitted) the vaccines administered will not be altered</param>
        public void UpdateParticipant(int id,
            CauseOfDeathOption causeOfDeath,
            string otherCauseOfDeathDetail,
            bool? bcgAdverse,
            string bcgAdverseDetail,
            bool? bcgPapuleAtDischarge,
            bool? bcgPauleAt28days,
            int? lastContactWeight,
            DateTime? lastWeightDate,
            DateTime? dischargeDateTime,
            DateTime? deathOrLastContactDateTime,
            OutcomeAt28DaysOption outcomeAt28Days,
            MaternalBCGScarStatus maternalBCGScar,
            FollowUpBabyBCGReactionStatus followUpBabyBCGReaction,
            DateTime? followUpContactMade,
            bool permanentlyUncontactable,
            string notes,
            IEnumerable<VaccineAdministered> participantVaccines=null,
            IEnumerable<UnsuccessfulFollowUp> unsuccesfulFollowUps = null)
        {
            Participant participant = _dbContext.Participants.Find(id);
            participant.CauseOfDeath = causeOfDeath;
            participant.OtherCauseOfDeathDetail = otherCauseOfDeathDetail;
            participant.BcgAdverse = bcgAdverse;
            participant.BcgAdverseDetail = bcgAdverseDetail;
            participant.BcgPapuleAtDischarge = bcgPapuleAtDischarge;
            participant.BcgPapuleAt28days = bcgPauleAt28days;
            participant.LastContactWeight = lastContactWeight;
            participant.LastWeightDate = lastWeightDate;
            participant.DischargeDateTime = dischargeDateTime;
            participant.DeathOrLastContactDateTime = deathOrLastContactDateTime;
            participant.OutcomeAt28Days = outcomeAt28Days;
            participant.RecordLastModified = DateTime.UtcNow;
            participant.MaternalBCGScar = maternalBCGScar;
            participant.FollowUpBabyBCGReaction = followUpBabyBCGReaction;
            participant.FollowUpContactMade = followUpContactMade;
            participant.PermanentlyUncontactable = permanentlyUncontactable;
            participant.Notes = notes;
            
            ((DbContext)_dbContext).AttachAndMarkModified(participant);
            if (participantVaccines!=null)
            {
                AddOrUpdateVaccinesAdministered(participant.Id, participantVaccines);
            }
            if (unsuccesfulFollowUps != null)
            {
                AddOrUpdateUnsuccessfulFollowUps(participant.Id, unsuccesfulFollowUps);
            }
            _dbContext.SaveChanges(true);
            if (this.ParticipantUpdated != null)
            {
                if (participant.VaccinesAdministered == null)
                {
                    participant.VaccinesAdministered = _dbContext.VaccinesAdministered.Where(v => v.ParticipantId == participant.Id).ToList();
                }
                else
                {
                    foreach (var v in participant.VaccinesAdministered)
                    {
                        if (v.VaccineGiven== null) { v.VaccineGiven = _dbContext.Vaccines.Find(v.VaccineId); }
                    }
                }
                if (participant.UnsuccessfulFollowUps == null)
                {
                    participant.UnsuccessfulFollowUps = _dbContext.UnsuccessfulFollowUps.Where(u => u.ParticipantId == participant.Id).ToList();
                }
                this.ParticipantUpdated(this, new ParticipantEventArgs(participant));
            }
        }
        /*
        /// <summary>
        /// Adds or updates all vaccines attached to participant.VaccinesAdministered
        /// </summary>
        /// <param name="vaccinesAdministered"></param>
        public void AddOrUpdateVaccinesFor(int participantId, IEnumerable<VaccineAdministered> vaccinesAdministered)
        {
            AddOrUpdateVaccinesAdministered(participantId, vaccinesAdministered);
            _dbContext.SaveChanges(true);
            if (this.ParticipantUpdated != null)
            {
                var part = FindParticipantAndCollections(participantId);
                part.VaccinesAdministered = new List<VaccineAdministered>(vaccinesAdministered);
                this.ParticipantUpdated(this, new ParticipantEventArgs(part));
            }
        }
        */
        void AddOrUpdateVaccinesAdministered(int participantId, IEnumerable<VaccineAdministered> givenParticipantVaccines)
        {
            HashSet<int> vaccineIds = new HashSet<int>();
            foreach (var v in givenParticipantVaccines)
            {

                if(v.ParticipantId != participantId)
                {
                    throw new ArgumentException("Participant Id differs from vaccine.ParticipantId");
                }
                if (v.ParticipantId == 0)
                {
                    v.ParticipantId = participantId;
                }
                if(v.VaccineId==0)
                {
                    throw new InvalidOperationException("Attempted to add VaccineAdministered with vaccine Id 0");
                }
                if(!vaccineIds.Add(v.VaccineId))
                {
                    throw new InvalidOperationException("Attempted to add VaccineAdministered with repeat vaccine Id");
                }
            }
            var includedVaccineAdministeredIds = (from v in givenParticipantVaccines
                                                  where v.Id != 0
                                                  select v.Id);
            _dbContext.VaccinesAdministered.RemoveRange(from v in _dbContext.VaccinesAdministered
                                                        where v.ParticipantId == participantId && !includedVaccineAdministeredIds.Contains(v.Id)
                                                        select v);
            

            var newVaccines = givenParticipantVaccines.ToLookup(va => va.Id == 0);
            if (newVaccines.Contains(false))
            {
                ((DbContext)_dbContext).AttachAndMarkModified(newVaccines[false]);
            }
            if (newVaccines.Contains(true))
            {
                int studyCentreId = (from p in _dbContext.Participants
                                     where p.Id == participantId
                                     select p.CentreId).First();
                int nextId = GetNextId(_dbContext.VaccinesAdministered, studyCentreId);
                foreach (var v in newVaccines[true])
                {
                    v.Id = nextId++;
                    _dbContext.VaccinesAdministered.Add(v);
                }
            }
        }
        void AddOrUpdateUnsuccessfulFollowUps(int participantId, IEnumerable<UnsuccessfulFollowUp> givenParticipantFollowUps)
        {
            foreach (var ufu in givenParticipantFollowUps)
            {
                if (ufu.ParticipantId != participantId)
                {
                    throw new ArgumentException("Participant Id differs from givenParticipantFollowUps.ParticipantId");
                }
                if (ufu.ParticipantId == 0)
                {
                    ufu.ParticipantId = participantId;
                }
            }
            var includedUnsuccessfulFollowUpIds = (from v in givenParticipantFollowUps
                                                  where v.Id != 0
                                                  select v.Id);
            _dbContext.UnsuccessfulFollowUps.RemoveRange(from v in _dbContext.UnsuccessfulFollowUps
                                                        where v.ParticipantId == participantId && !includedUnsuccessfulFollowUpIds.Contains(v.Id)
                                                        select v);


            var newFollowUps = givenParticipantFollowUps.ToLookup(va => va.Id == 0);
            if (newFollowUps.Contains(false))
            {
                ((DbContext)_dbContext).AttachAndMarkModified(newFollowUps[false]);
            }
            if (newFollowUps.Contains(true))
            {
                int studyCentreId = (from p in _dbContext.Participants
                                     where p.Id == participantId
                                     select p.CentreId).First();
                int nextId = GetNextId(_dbContext.UnsuccessfulFollowUps, studyCentreId);
                foreach (var u in newFollowUps[true])
                {
                    u.Id = nextId++;
                    _dbContext.UnsuccessfulFollowUps.Add(u);
                }
            }
        }
        public void Add(Vaccine newVaccine)
        {
            newVaccine.Id = GetNextId(_dbContext.Vaccines, LocalStudyCentres.First().Id);
            if (newVaccine.Id <= DataContextInitialiser.MaxReservedVaccineId)
            {
                newVaccine.Id = DataContextInitialiser.MaxReservedVaccineId + 1;
            }
            _dbContext.Vaccines.Add(newVaccine);
            _dbContext.SaveChanges(true);
            _vaccines.Add(newVaccine);
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
            _dbContext.SaveChanges(true);
        }
        public DateTime? LastCreateModifyParticipant()
        {
            var returnVar = (new DateTime?[] { _dbContext.Participants.Max(p => (DateTime?)p.RecordLastModified),
                                        _dbContext.VaccinesAdministered.Max(va=>(DateTime?)va.RecordLastModified),
                                        _dbContext.UnsuccessfulFollowUps.Max(uf=>(DateTime?)uf.RecordLastModified)}).Max();
            if (returnVar.HasValue) { DateTime.SpecifyKind(returnVar.Value, DateTimeKind.Utc); }
            return returnVar;
        }
        public void Update(ScreenedPatient patient)
        {
            ((DbContext)_dbContext).AttachAndMarkModified(patient);
            _dbContext.SaveChanges(true);
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
            string uniqueFileNameSuffix = '_' + LocalStudyCentres.First().DuplicateIdCheck.ToString("N");
            string cloudDir = CloudDirectories.First();
            string cloudZipName = cloudDir + '\\' + Path.GetFileNameWithoutExtension(bakFileName) + uniqueFileNameSuffix + ".zip"; 
            
            var cloudFile = new FileInfo(cloudZipName);

            if (cloudFile.Exists)
            {
                DateTime? mostRecentEntry = SyncronisationResult.MostRecentEntry(_dbContext);
                if (mostRecentEntry==null || cloudFile.LastWriteTimeUtc >= mostRecentEntry)
                {
                    return;
                }
            }
            _dbContext.Dispose(); // only necessary for ce
            SyncronisationResult.RepairDb(bakFileName);
            int dotPos = bakFileName.LastIndexOf('.');
            string copiedFileName = bakFileName.Insert(dotPos, uniqueFileNameSuffix);
            File.Copy(bakFileName, copiedFileName, true);

            void work()
            {
                BackupHelper.ZipVerifyAndPutInCloudDir(copiedFileName, cloudDir);
            }
            new Thread(work).Start();
            _dbContext = _createContext.Invoke();
        }

        public void Restore()
        {
            //DateTime RecordLastModified = _dbContext.DbLastModifiedUtc(); //note this is all assuming user on backup end has no ability to modify data
            IEnumerable<MatchedFilePair> filePairs = GetMatchedCloudAndExtractedFiles()
                .Where(fp => fp.ExtractedBak == null || fp.ExtractedBak.CreationTimeUtc < fp.Zip.LastWriteTimeUtc)
                .ToList();
            
            if (!filePairs.Any()) { return; }
            List<BakFileDetails> bakDetails = new List<BakFileDetails>();
            _dbContext.Dispose();

            foreach (var fp in filePairs)
            {
                using (ZipFile readFile = ZipFile.Read(fp.Zip.FullName))
                {
                    try
                    {
                        readFile[0].Extract(App.DataDirectory, ExtractExistingFileAction.OverwriteSilently);
                    }
                    catch(Exception e)
                    {
                        FailedDbRestore?.Invoke(this, new FailedRestoreEvent(fp.Zip.FullName, e));
                    }
                }
                BakFileDetails newBak = new BakFileDetails();
                bakDetails.Add(newBak);
                if (fp.ExtractedBak == null)
                {
                    fp.ExtractedBak = new FileInfo(Path.Combine(App.DataDirectory, Path.GetFileNameWithoutExtension(fp.Zip.Name) + BakExtension));
                }
                else
                {
                    newBak.LastBackupUtc = fp.ExtractedBak.CreationTimeUtc;
                }
                newBak.FullFilename = fp.ExtractedBak.FullName;
                fp.ExtractedBak.CreationTimeUtc = fp.Zip.LastWriteTimeUtc;
            }

            _dbContext = _createContext.Invoke();
            AddOrUpdateBaks(bakDetails.Select(b=>b.FullFilename));

        }
        public IEnumerable<KeyValuePair<string,IEnumerable<StudyCentreModel>>> GetFilenamesAndCentres()
        {
            Restore();
            var returnVar = new List<KeyValuePair<string,IEnumerable<StudyCentreModel>>>();
            foreach (var p in GetMatchedCloudAndExtractedFiles())
            {
                if (p.ExtractedBak!=null)
                {
                    MigrateIfRequired(p.ExtractedBak.FullName);
                    using (var db = _dbContext.AttachDb(p.ExtractedBak.FullName))
                    {
                        returnVar.Add(new KeyValuePair<string,IEnumerable<StudyCentreModel>>(p.Zip.FullName,db.StudyCentres.Select(s=>s.Id).ToList().Select(s=>LocalStudyCentreDictionary[s]).ToList()));
                    }
                }
            }
            return returnVar;
        }
        const string _bakExtension = ".sdf";
        public string BackupLimitedDbTo(string directory, params StudyCentreModel[] studyCentres)
        {
            
            //this is a hack, and will not work if moving to a full sql server instance
            Restore();
            string destination = Path.Combine(directory, _dbContext.DbName + '_' + studyCentres.First().DuplicateIdCheck.ToString("N")) + _bakExtension;
            File.Copy(App.DataDirectory + '\\' + _dbContext.DbName + _bakExtension, destination, true);
            string whereString = "WHERE NOT (" + studyCentres.Select(s => string.Format("(Id BETWEEN '{0}' AND '{1}')", s.Id, s.MaxIdForSite)).Aggregate((c, n) => c + " OR " + n) + ')';
            using (var db = _dbContext.AttachDb(destination))
            {
                foreach (string table in new string[] { "StudyCentres", "Participants", "ProtocolViolations", "ScreenedPatients", "VaccinesAdministered" })
                {
                    db.Database.ExecuteSqlCommand("Delete from [" + table + "] " + whereString);
                }
            }
            return destination;
        }
        void MigrateIfRequired(string fullFilename)
        {
            if (_baksForgoingMigration == null) { _baksForgoingMigration = new List<string>(); }
            if (!_baksForgoingMigration.Contains(fullFilename))
            {
                if (!CodeBasedMigration.ApplyPendingMigrations<BlowTrial.Migrations.TrialData.TrialDataConfiguration>(TrialDataContext.GetConnectionString(fullFilename), ContextCeConfiguration.ProviderInvariantName))
                {
                    _baksForgoingMigration.Add(fullFilename);
                }
            }
        }

        void AddOrUpdateBaks(IEnumerable<string> bakupFilePaths)
        {
            if (_isBackingUp) { return; }
            _isBackingUp = true;
            foreach (var f in bakupFilePaths)
            {
                MigrateIfRequired(f);
            }

#pragma warning disable IDE0067 // Dispose objects before losing scope
            BackgroundWorker worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true
            };
#pragma warning restore IDE0067 // Dispose objects before losing scope
            worker.DoWork += SyncronisationResult.Sync;

            if (UpdateProgress != null)
            {
                worker.ProgressChanged += UpdateProgress;
            }
            bool isForUpdate = ParticipantAdded != null || ParticipantUpdated != null || ScreenedPatientAdded != null;
            if (isForUpdate)
            {
                worker.RunWorkerCompleted += WhenSyncronisationResultsAvailable;
            }
            if (AnyParticipantChange != null)
            {
                worker.RunWorkerCompleted += (o, e) => AnyParticipantChange(this, new LastUpdatedChangedEventAgs(LastCreateModifyParticipant()));
            }
            if (DatabaseUpdating != null)
            {
                DatabaseUpdating(this, new DatabaseUpdatingEventAgs(true));
                worker.RunWorkerCompleted += (o, e) => DatabaseUpdating(this, new DatabaseUpdatingEventAgs(false));
            }
            worker.RunWorkerCompleted += (o, e) => _isBackingUp = false;
            worker.RunWorkerCompleted += (o, e) =>
            {
                string fn = App.DataDirectory + '\\' + _dbContext.DbName + _bakExtension;
                _dbContext.Dispose();
                SyncronisationResult.RepairDb(fn);
                _dbContext = _createContext.Invoke();
                worker.Dispose();
            };
            worker.RunWorkerAsync(new SyncronisationResult.SyncArgs
                {   DestContext = _dbContext,
                    DbFileNames = bakupFilePaths.ToList(),
                    UpdateResults = isForUpdate
                });
        }

        private void WhenSyncronisationResultsAvailable(object sender, RunWorkerCompletedEventArgs e)
        {
            SyncronisationResult syncResults = (SyncronisationResult)e.Result;

            if (ParticipantAdded != null)
            {
                foreach (var p in (from part in _dbContext.Participants.Include("VaccinesAdministered").Include("ProtocolViolations").Include("UnsuccessfulFollowUps")
                                   where syncResults.AddedParticipantIds.Contains(part.Id)
                                   select part))
                {
                    ParticipantAdded(this, new ParticipantEventArgs(p));
                }
            }
            if (ParticipantUpdated != null)
            {
                foreach (var p in (from part in _dbContext.Participants.Include("VaccinesAdministered").Include("ProtocolViolations").Include("UnsuccessfulFollowUps")
                                   where syncResults.UpdatedParticipantIds.Contains(part.Id)
                                        || ((part.VaccinesAdministered.Any(v => syncResults.UpsertedVaccineAdministeredIds.Contains(v.Id))
                                                || part.UnsuccessfulFollowUps.Any(v => syncResults.UpsertedUnsuccessfulFollowUpIds.Contains(v.Id))
                                            && !syncResults.AddedParticipantIds.Contains(part.Id)))
                                   select part))
                {
                    ParticipantUpdated(this, new ParticipantEventArgs(p));
                }
            }
            if (ScreenedPatientAdded != null)
            {
                foreach (var s in (from screen in _dbContext.ScreenedPatients
                                   where syncResults.AddedScreenPatientIds.Contains(screen.Id)
                                   select screen))
                {
                    ScreenedPatientAdded(this, new ScreenedPatientEventArgs(s));
                }
            }
            if (ProtocolViolationAddOrUpdate != null)
            {
                foreach (var pv in (from v in _dbContext.ProtocolViolations
                                    where syncResults.AddedProtocolViolationIds.Contains(v.Id)
                                    select v))
                {
                    ProtocolViolationAddOrUpdate(this, new ProtocolViolationEventArgs(pv, CRUD.Created));
                }
                foreach (var pv in (from v in _dbContext.ProtocolViolations
                                    where syncResults.UpdatedProtocolViolationIds.Contains(v.Id)
                                    select v))
                {
                    ProtocolViolationAddOrUpdate(this, new ProtocolViolationEventArgs(pv, CRUD.Updated));
                }
            }
        }

        public ParticipantsSummary GetParticipantSummary()
        {
            return new ParticipantsSummary(from p in _dbContext.Participants.AsExpandable()
                                           select new ParticipantStage 
                                           { 
                                               Id = p.Id, 
                                               Arm = p.TrialArm,
                                               DataRequired = ParticipantBaseModel.DataRequiredExpression.Invoke(p)
                                           });
        }
        public ICollection<StudyCentreModel> GetCentresRequiringData()
        {
            return new List<StudyCentreModel>((from p in _dbContext.Participants.AsExpandable()
                                               let dataRequired = ParticipantBaseModel.DataRequiredExpression.Invoke(p)
                                               where dataRequired != DataRequiredOption.Complete
                                               select p.CentreId)
                                                    .GroupBy(c => c)
                                                    .Select(g=>g.Key)
                                                    .AsEnumerable()
                                                    .Select(id => LocalStudyCentreDictionary[id]));
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
            return recordSet.GetNextId(studyCentreId,LocalStudyCentreDictionary[studyCentreId].MaxIdForSite);
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
                                   where f.Name.Length == fnLen && f.Name.Substring(0, prefixLen) == filePrefix && f.Extension==".zip"
                                   select new MatchedFilePair{ Zip = f });
            }

            di = new DirectoryInfo(App.DataDirectory);
            foreach (FileInfo f in di.GetFiles())
            {
                if (f.Name.EndsWith(".sdf.tmp"))
                {
                    try
                    {
                        f.Delete();
                    }
                    catch (Exception) { }
                    continue;

                }
                if (f.Name.Length == fnLen && f.Name.Substring(0, prefixLen) == filePrefix && f.Extension == BakExtension)
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

        #region MatchedFilePair
        private class MatchedFilePair
        {
            FileInfo _zip;
            FileInfo _extractedBak;
            internal FileInfo Zip 
            {
                get { return _zip; }
                set 
                { 
                    if (value.Extension!=".zip")
                    {
                        throw new InvalidFileTypeException(value.Extension,".zip");
                    }
                    _zip = value;
                }
            }
            internal FileInfo ExtractedBak 
            {
                get { return _extractedBak; }
                set
                {
                    if (value.Extension!=BakExtension)
                    {
                        throw new InvalidFileTypeException(value.Extension, BakExtension);
                    }
                    _extractedBak = value;
                }
            }
        }
        #endregion

        #region BakFileDetails
        private class BakFileDetails
        {
            internal string FullFilename { get; set; }
            internal DateTime LastBackupUtc { get; set; }
        }
        #endregion
    }
}
