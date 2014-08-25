using BlowTrial.Domain.Outcomes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Tables
{
    public abstract class PatientCsvModel
    {
	    public int Id {get; set;}
	    public int AdmissionWeight {get;set;}
	    public double GestAgeBirth {get;set;}
	    public string AdmissionDiagnosis {get;set;}
	    public bool IsMale {get;set;}
	    public DateTime DateTimeBirth {get;set;}
	    public DateTime RegisteredAt {get; set;}
        public int CentreId { get; set; }
        public bool? Inborn { get; set; }
        public string RegisteringInvestigator { get; set; }

        /*
        [ForeignKey("RegisteredById")]
        public virtual Investigator RegisteredBy { get; set; }
        */
    }
    public partial class ParticipantCsvModel : PatientCsvModel
    {
        public string Name { get; set; }
        public string MothersName { get; set; }
        public string PhoneNumber { get; set; }
        public RandomisationArm TrialArm { get; set; }
        public bool? BcgAdverse { get; set; }
        public string BcgAdverseDetail { get; set; }
        public bool? BcgPapuleAtDischarge { get; set; }
        public bool? BcgPapuleAt28days { get; set; }
        public int? LastContactWeight { get; set; }
        [DataType(DataType.Date)]
        public DateTime? LastWeightDate { get; set; }
        public virtual DateTime? DischargeDateTime { get; set; }
        public virtual DateTime? DeathOrLastContactDateTime { get; set; }
        public string OtherCauseOfDeathDetail { get; set; }
        public int? AllocationBlockId { get; set; }
        public int? MultipleSiblingId { get; set; }
        public int CauseOfDeathId { get; set; }
        [Display(Name = "28 Day Outcome Code")]
        public int OutcomeAt28Id { get; set; }
        public string Notes { get; set; }
        public bool WasEnvelopeRandomised { get; set; }
        
        public virtual ICollection<VaccineAdministered> VaccinesAdministered { get; set; }
    }
    public class ScreenedPatientCsvModel : PatientCsvModel
    {
	    public bool LikelyDie24Hr {get; set;}
	    public bool BadMalform {get; set;}
	    public bool BadInfectnImmune {get; set;}
	    public bool WasGivenBcgPrior {get;set;}
	    public bool RefusedConsent {get;set;}
        public bool Missed { get; set; }
    }
}
