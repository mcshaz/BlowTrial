using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Providers;
using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Randomising;
using BlowTrial.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Interfaces
{
    [Flags]
    public enum UpdateParticipantViolationType
    {
        NoViolations = 0,
        BlockCriteriaChanged = 1,
        IneligibleWeight = 2,
        MultipleSiblingIdChanged = 4
    }
    public interface IRepository : IDisposable
    {
        event EventHandler<ParticipantEventArgs> ParticipantAdded;
        event EventHandler<ScreenedPatientEventArgs> ScreenedPatientAdded;
        event EventHandler<ParticipantEventArgs> ParticipantUpdated;
        event EventHandler<ProtocolViolationEventArgs> ProtocolViolationAdded;
        //event EventHandler<ScreenedPatientEventArgs> ScreenedPatientUpdated;
        Participant AddParticipant(
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
            int? multipleSiblingId,
            int? envelopeNumber = null);
        void Add(ScreenedPatient patient);
        void Add(Vaccine newVaccine);
        void UpdateParticipant(int id,
            CauseOfDeathOption causeOfDeath,
            string otherCauseOfDeathDetail,
            bool? bcgAdverse,
            string bcgAdverseDetail,
            bool? BcgPapuleAtDischarge,
            int? lastContactWeight,
            DateTime? lastWeightDate,
            DateTime? dischargeDateTime,
            DateTime? deathOrLastContactDateTime,
            OutcomeAt28DaysOption outcomeAt28Days,
            string notes,
            IEnumerable<VaccineAdministered> vaccinesAdministered=null);

        UpdateParticipantViolationType UpdateParticipant(int id,
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
            DateTime registeredAt,
            bool isEnvelopeRandomising);
        void AddOrUpdateVaccinesFor(int participantId, IEnumerable<VaccineAdministered> vaccinesAdministered);
        void Update(IEnumerable<Participant> patients);
        void Update(Participant patient);
        void Update(ScreenedPatient patient);
        void AddOrUpdate(ProtocolViolation violation);
        void AddOrUpdate(IEnumerable<StudyCentre> centres);
        DbQuery<Participant> Participants { get; }
        DbQuery<ScreenedPatient> ScreenedPatients { get; }
        DbQuery<VaccineAdministered> VaccinesAdministered { get; }
        DbQuery<Vaccine> Vaccines { get; }
        DbQuery<ProtocolViolation> ProtocolViolations { get; }
        Participant FindParticipant(int participantId);
        ProtocolViolation FindViolation(int violationId);
        IEnumerable<string> CloudDirectories { get; set; }
        IEnumerable<StudyCentreModel> LocalStudyCentres { get; }
        ParticipantsSummary GetParticipantSummary();
        ScreenedPatientsSummary GetScreenedPatientSummary();
        StudyCentreModel FindStudyCentre(int studyCentreId);
        string BackupLimitedDbTo(string directory, params StudyCentreModel[] studyCentres);
        IEnumerable<KeyValuePair<string, IEnumerable<StudyCentreModel>>> GetFilenamesAndCentres();
        Database Database { get; }
        void Backup();
        void Restore();
    }
}
