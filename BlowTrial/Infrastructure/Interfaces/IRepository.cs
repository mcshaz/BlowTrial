using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Providers;
using BlowTrial.Domain.Tables;
using BlowTrial.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Interfaces
{
    public interface IRepository : IDisposable
    {
        event EventHandler<ParticipantEventArgs> ParticipantAdded;
        event EventHandler<ScreenedPatientEventArgs> ScreenedPatientAdded;
        event EventHandler<ParticipantEventArgs> ParticipantUpdated;
        event EventHandler<ProtocolViolationEventArgs> ProtocolViolationAdded;
        //event EventHandler<ScreenedPatientEventArgs> ScreenedPatientUpdated;
        void Add(Participant patient);
        void Add(ScreenedPatient patient);
        void Add(Vaccine newVaccine);
        void UpdateParticipant(int id,
                CauseOfDeathOption causeOfDeath,
                String bcgAdverseDetail,
                bool? bcgAdverse,
                bool? bcgPapule,
                int? lastContactWeight,
                DateTime? lastWeightDate,
                DateTime? dischargeDateTime,
                DateTime? deathOrLastContactDateTime,
                OutcomeAt28DaysOption outcomeAt28Days);
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
        void Backup();
        void Restore();
    }
}
