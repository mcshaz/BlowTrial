using BlowTrial.Domain.Outcomes;
using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlowTrial.Domain.Tables
{
    public abstract class Patient : ISharedRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
	    public int Id {get; set;}
        [StringLength(128)]
	    public string HospitalIdentifier {get;set;}
        [StringLength(512)]
        public string AdmissionDiagnosis { get; set; }
	    public int AdmissionWeight {get;set;}
        public bool? Inborn { get; set; }
	    public double GestAgeBirth {get;set;}
	    public bool IsMale {get;set;}
	    public DateTime DateTimeBirth {get;set;}
	    public DateTime RegisteredAt {get; set;}
        public int AppVersionAtEnrollment { get; set; }
        [ForeignKey("Centre")]
        public int CentreId { get; set; }
        [StringLength(64)]
        public string RegisteringInvestigator { get; set; }

        public DateTime RecordLastModified { get; set; }

        public virtual StudyCentre Centre { get; set; }
    }
    public partial class Participant : Patient, IParticipant
    {
        [StringLength(256)]
        public string Name { get; set; }
        [StringLength(256)]
        public string MothersName { get; set; }
        [StringLength(16)]
        public string PhoneNumber { get; set; }
        public RandomisationArm TrialArm { get; set; }
        public bool? BcgAdverse { get; set; }
        [StringLength(2056)]
        public string BcgAdverseDetail { get; set; }
        public bool? BcgPapuleAtDischarge { get; set; }
        public bool? BcgPapuleAt28days { get; set; }
        public int? LastContactWeight { get; set; }
        public DateTime? LastWeightDate { get; set; }
        public virtual DateTime? DischargeDateTime { get; set; }
        public virtual DateTime? DeathOrLastContactDateTime { get; set; }
        [StringLength(2056)]
        public string OtherCauseOfDeathDetail { get; set; }
        [ForeignKey("Block")]
        public int AllocationBlockId { get; set; }
        public int? MultipleSiblingId { get; set; }
        public CauseOfDeathOption CauseOfDeath { get; set; }
        public OutcomeAt28DaysOption OutcomeAt28Days { get; set; }
        [StringLength(160)]
        public string Notes { get; set; }
        public bool WasEnvelopeRandomised { get; set; }
        //[Column(TypeName = "Date")] NA in compact
        public DateTime? FollowUpContactMade { get; set; }
        [StringLength(512)]
        public String FollowUpComment { get; set; }
        public bool PermanentlyUncontactable { get; set; }
        public MaternalBCGScarStatus MaternalBCGScar {get;set;}
        public FollowUpBabyBCGReactionStatus FollowUpBabyBCGReaction { get; set; }

        public virtual AllocationBlock Block { get; set; }
        public virtual ICollection<VaccineAdministered> VaccinesAdministered { get; set; }
        public virtual ICollection<ProtocolViolation> ProtocolViolations { get; set; }
        public virtual ICollection<UnsuccessfulFollowUp> UnsuccesfulFollowUps { get; set; }
    }
    public class ScreenedPatient : Patient
    {
	    public bool LikelyDie24Hr {get; set;}
	    public bool BadMalform {get; set;}
	    public bool BadInfectnImmune {get; set;}
	    public bool WasGivenBcgPrior {get;set;}
	    public bool? RefusedConsent {get;set;}
        public bool? Missed { get; set; }
    }
}
