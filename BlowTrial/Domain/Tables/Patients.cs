using BlowTrial.Domain.Outcomes;
using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Tables
{
    public abstract class Patient
    {
        [Key]
	    public int Id {get; set;}
        [StringLength(256)]
	    public string Name {get;set;}
        [StringLength(256)]
        public string MothersName { get; set; }
        [StringLength(16)]
        public string PhoneNumber { get; set; }
        [StringLength(128)]
	    public string HospitalIdentifier {get;set;}
	    public int AdmissionWeight {get;set;}
	    public double GestAgeBirth {get;set;}
        [StringLength(512)]
	    public string Abnormalities {get;set;}
	    public bool IsMale {get;set;}
	    public DateTime DateTimeBirth {get;set;}
	    public DateTime RegisteredAt {get; set;}
        public int CentreId { get; set; }
        [StringLength(64)]
        public string RegisteringInvestigator { get; set; }

        /*
        [ForeignKey("RegisteredById")]
        public virtual Investigator RegisteredBy { get; set; }
        */
    }
    public partial class Participant : Patient, IParticipant
    {
        public bool IsInterventionArm { get; set; }
        public bool? BcgAdverse { get; set; }
        [StringLength(2056)]
        public string BcgAdverseDetail { get; set; }
        public bool? BcgPapule { get; set; }
        public int? LastContactWeight { get; set; }
        public DateTime? LastWeightDate { get; set; }
        public virtual DateTime? DischargeDateTime { get; set; }
        public virtual DateTime? DeathOrLastContactDateTime { get; set; }
        [StringLength(2056)]
        public string OtherCauseOfDeathDetail { get; set; }
        public int BlockNumber { get; set; }
        public int BlockSize { get; set; }
        public int? MultipleSiblingId { get; set; }
        public CauseOfDeathOption CauseOfDeath { get; set; }
        public OutcomeAt28DaysOption OutcomeAt28Days { get; set; }

        public virtual ICollection<VaccineAdministered> VaccinesAdministered { get; set; }
        public virtual ICollection<ProtocolViolation> ProtocolViolations { get; set; }
    }
    public class ScreenedPatient : Patient
    {
	    public bool LikelyDie24Hr {get; set;}
	    public bool BadMalform {get; set;}
	    public bool BadInfectnImmune {get; set;}
	    public bool WasGivenBcgPrior {get;set;}
	    public bool? RefusedConsent {get;set;}
        public bool Missed { get; set; }
    }
}
