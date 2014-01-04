using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Providers;
using BlowTrial.Domain.Tables;
using BlowTrial.Helpers;
using BlowTrial.Infrastructure;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace BlowTrial.Models
{
    public partial class ParticipantModel : IDataErrorInfo, IParticipant
    {
        #region Constructors

        #endregion // Constructors

        #region Fields
        const double TicksPerWeek = TimeSpan.TicksPerDay * 7;
        TimeSpan _cgabirth;
        DateTime? _becomes28On;
        #endregion //Fields

        #region Properties

        public int Id { get; set; }
        public string Name { get; set; }
        public string MothersName { get; set; }
        public string PhoneNumber { get; set; }
        public string HospitalIdentifier { get; set; }
        public int AdmissionWeight { get; set; }
        public double GestAgeBirth { get; set; }
        public string Abnormalities { get; set; }
        public bool IsMale { get; set; }
        public DateTime DateTimeBirth { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int CentreId { get; set; }
        public string RegisteringInvestigator { get; set; }
        public bool IsInterventionArm { get; set; }
        public bool? BcgAdverse { get; set; }
        public string BcgAdverseDetail { get; set; }
        public bool? BcgPapule { get; set; }
        public int? LastContactWeight { get; set; }
        public DateTime? LastWeightDate { get; set; }
        public string OtherCauseOfDeathDetail { get; set; }

        public CauseOfDeathOption CauseOfDeath { get; set; }
        public OutcomeAt28DaysOption OutcomeAt28Days { get; set; }
        public StudyCentreModel StudyCentre { get; set; }

        public ICollection<VaccineAdministeredModel> VaccineModelsAdministered { get; set; }

        /// <summary>
        /// To implement IParticipant - mapping Id only at this point
        /// </summary>
        public ICollection<VaccineAdministered> VaccinesAdministered
        {
            get
            {
                return VaccineModelsAdministered.Select(v => new VaccineAdministered { Id = v.Id }).ToList();
            }
        }

        public TimeSpan Age
        {
            get { return (DateTime.Now - DateTimeBirth);  }
        }

        public DateTime Becomes28On
        {
            get
            {
                return (_becomes28On ?? (_becomes28On = DateTimeBirth.AddDays(28))).Value;
            }
        }
        TimeSpan CgaBirth
        {
            get
            {
                if (_cgabirth == default(TimeSpan))
                {
                    _cgabirth = new TimeSpan((long)(TicksPerWeek * GestAgeBirth));
                }
                return _cgabirth;
            }
        }
        public TimeSpan CGA
        {
            get
            {
                return  Age.Add(CgaBirth);
            }
        }
        public string TrialArm
        {
            get
            {
                return IsInterventionArm
                    ? Strings.ParticipantUpdateVM_InterventionArm
                    : Strings.ParticipantUpdateVM_ControlArm;
            }
        }
        public string Gender
        {
            get
            {
                return IsMale
                    ? Strings.NewPatient_Gender_Male
                    : Strings.NewPatient_Gender_Female;
            }
        }

        DateTimeSplitter _dischargeDateTime = new DateTimeSplitter();

        public DateTime? DischargeDateTime 
        {
            get 
            {
                return _dischargeDateTime.DateAndTime;
            }
            set 
            {
                _dischargeDateTime.DateAndTime = value;
            }
        }
        public DateTime? DischargeDate
        {
            get
            {
                return _dischargeDateTime.Date;
            }
            set
            {
                _dischargeDateTime.Date = value;
                DischargeDateTime = _dischargeDateTime.DateAndTime;
            }
        }
        public TimeSpan? DischargeTime
        {
            get 
            { 
                return _dischargeDateTime.Time; 
            }
            set
            {
                _dischargeDateTime.Time = value;
                DischargeDateTime = _dischargeDateTime.DateAndTime;
            }
        }
        DateTimeSplitter _deathOrLastContactDateTime = new DateTimeSplitter();
        public DateTime? DeathOrLastContactDateTime
        {
            get
            {
                return _deathOrLastContactDateTime.DateAndTime;
            }
            set
            {
                _deathOrLastContactDateTime.DateAndTime = value;
            }
        }
        public DateTime? DeathOrLastContactDate
        {
            get
            {
                return _deathOrLastContactDateTime.Date;
            }
            set
            {
                _deathOrLastContactDateTime.Date = value;
                DeathOrLastContactDateTime = _deathOrLastContactDateTime.DateAndTime;
            }
        }
        public TimeSpan? DeathOrLastContactTime
        {
            get
            {
                return _deathOrLastContactDateTime.Time;
            }
            set
            {
                _deathOrLastContactDateTime.Time = value;
                DeathOrLastContactDateTime = _deathOrLastContactDateTime.DateAndTime;
            }
        }
        public bool? IsKnownDead
        {
            get
            {
                switch (OutcomeAt28Days)
                {
                    case OutcomeAt28DaysOption.DiedInHospitalBefore28Days:
                    case OutcomeAt28DaysOption.DischargedAndKnownToHaveDied:
                        return true;
                    case OutcomeAt28DaysOption.InpatientAt28Days:
                    case OutcomeAt28DaysOption.DischargedAndKnownToHaveSurvived:
                        return false;
                    default:
                        return null;
                }
            }
        }

        public DataRequiredOption DataRequired
        {
            get { return GetDataRequired().Compile()(this); }
        }

        #endregion //Properties

        #region IDataErrorInfo Members

        public string Error { get { return null; } }

        public string this[string propertyName]
        {
            get { return this.GetValidationError(propertyName); }
        }

        #endregion // IDataErrorInfo Members

        #region Validation

        public bool IsValid()
        {
                DateTime? now = DateTime.Now;
                return (!ValidatedProperties.Any(v => GetValidationError(v, now) != null));  
        }
        static readonly string[] ValidatedProperties = new string[]
        { 
            "OutcomeAt28Days",
            "LastContactWeight", 
            "LastWeightDate", 
            "DischargeDate",
            "DischargeTime",
            "DeathOrLastContactDate",
            "DeathOrLastContactTime",
            "OtherCauseOfDeathDetail",
            "CauseOfDeath",
            "BcgAdverseDetail"
        };
        public string GetValidationError(string propertyName, DateTime? now=null)
        {
            if (!ValidatedProperties.Contains(propertyName))
            { return null; }
            string error = null;

            switch (propertyName)
            {
                case "OutcomeAt28Days":
                    error = this.ValidateOutcomeAt28Days(now);
                    break;
                case "LastContactWeight":
                    error = this.ValidateWeight();
                    break;
                case "LastWeightDate":
                    error = this.ValidateWeightDate(now);
                    break;
                case "DischargeDate":
                    error = ValidateDischargeDateTime(now).DateError;
                    break;
                case "DischargeTime":
                    error = ValidateDischargeDateTime(now).TimeError;
                    break;
                case "DeathOrLastContactDate":
                    error = this.ValidateDeathOrLastContactDateTime(now).DateError;
                    break;
                case "DeathOrLastContactTime":
                    error = this.ValidateDeathOrLastContactDateTime(now).TimeError;
                    break;
                case "OtherCauseOfDeathDetail":
                    error = ValidateOtherCauseOfDeathDetail();
                    break;
                case "CauseOfDeath":
                    error = ValidateCauseOfDeath();
                    break;
                case "BcgAdverseDetail":
                    error = ValidateBcgAdverseDetail();
                    break;
                default:
                    Debug.Fail("Unexpected property being validated on ParticipantUpdateModel: " + propertyName);
                    break;
            }
            return error;
        }
        string ValidateOutcomeAt28Days(DateTime? now = null)
        {
            if (OutcomeAt28Days==OutcomeAt28DaysOption.InpatientAt28Days)
            {
                var ageDays = ((now ?? DateTime.Now) - DateTimeBirth).Days;
                if (ageDays < 28)
                {
                    return Strings.ParticipantModel_Error_28daysNotElapsed;
                }
            }
            return null;
        }
        string ValidateOtherCauseOfDeathDetail()
        {
            if (CauseOfDeath==CauseOfDeathOption.Other && string.IsNullOrWhiteSpace(OtherCauseOfDeathDetail))
            {
                return Strings.ParticipantModel_Error_CauseOfDeathDataRequired;
            }
            return null;
        }
        string ValidateCauseOfDeath()
        {
            if (IsKnownDead==true && CauseOfDeath==CauseOfDeathOption.Missing)
            {
                return Strings.ParticipantModel_Error_CauseOfDeathRequired;
            }
            return null;
        }
        string ValidateBcgAdverseDetail()
        {
            if (BcgAdverse==true && string.IsNullOrWhiteSpace(BcgAdverseDetail))
            {
                return Strings.ParticipantModel_Error_BcgAdverseDetailRequired;
            }
            return null;
        }
        enum RandomisationCategories { lowestWeight, midWeight, highestWeight}
        string ValidateWeight()
        {
            if (LastContactWeight.HasValue)
            {
                var weightChange = (double)LastContactWeight / (double)AdmissionWeight;
                RandomisationCategories weightCat = LastContactWeight<RandomisingEngine.BlockWeight1?
                    RandomisationCategories.lowestWeight
                    :LastContactWeight<RandomisingEngine.BlockWeight2? 
                        RandomisationCategories.midWeight
                        :RandomisationCategories.highestWeight;
                int? daysOldAtWeight = (LastWeightDate.HasValue)?
                    (LastWeightDate.Value-DateTimeBirth.Date).Days
                    :(int?)null;
                if (weightChange < 0.8 || 
                    weightChange > 1.6 ||
                    (weightCat == RandomisationCategories.midWeight && weightChange >1.5) ||
                    (weightCat == RandomisationCategories.lowestWeight && weightChange >1.4) ||
                    (daysOldAtWeight.HasValue && 
                    (daysOldAtWeight<21 && (weightChange > 1.4 ||
                    (weightCat == RandomisationCategories.midWeight && weightChange >1.35) ||
                    (weightCat == RandomisationCategories.lowestWeight && weightChange >1.3))) ||
                    (daysOldAtWeight<14 && weightChange >1.25)))
                {
                    return string.Format(Strings.ParticipantModel_Error_LastWeightChange, AdmissionWeight);
                }
            }
            else if (LastWeightDate.HasValue)
            {
                return Strings.ParticipantModel_Error_WeightNotFound;
            }
            return null;
        }
        string ValidateWeightDate(DateTime? now=null)
        {
            if (LastWeightDate.HasValue)
            {
                if (LastWeightDate > (now ?? DateTime.Now))
                {
                    return string.Format(Strings.DateTime_Error_Date_MustComeBefore, Strings.DateTime_Today);
                }
                if (LastWeightDate < RegisteredAt.Date)
                {
                    return string.Format(Strings.DateTime_Error_Date_MustComeAfter, Strings.ParticipantModel_Error_RegistrationDate);
                }
                if (LastWeightDate > DeathOrLastContactDate)
                {
                    return string.Format(Strings.DateTime_Error_Date_MustComeBefore,
                        (IsKnownDead==true)
                        ? Strings.ParticipantUpdateView_Label_DeathDateTime
                        : Strings.ParticipantUpdateView_Label_LastContactDateTime);
                }
            }
            else if (LastContactWeight.HasValue)
            {
                return Strings.DateTime_Error_DateEmpty;
            }
            return null;
        }
        DateTimeErrorString ValidateDischargeDateTime(DateTime? now = null)
        {
            DateTimeErrorString error;
            if (OutcomeAt28Days >= OutcomeAt28DaysOption.DischargedBefore28Days)
            {
                error = _dischargeDateTime.ValidateNotEmpty();
                ValidateIsBetweenRegistrationAndNowOr28(ref error, _dischargeDateTime, now);
            }
            else
            {
                error = new DateTimeErrorString();
            }
            return error;
        }
        void ValidateIsBetweenRegistrationAndNowOr28(ref DateTimeErrorString error,DateTimeSplitter splitter, DateTime? now = null)
        {
            splitter.ValidateIsAfter(Strings.ParticipantModel_Error_RegistrationDateTime,
                RegisteredAt,
                ref error);
            DateTime nowVal = now ?? DateTime.Now;
            if (nowVal >= Becomes28On)
            {
                splitter.ValidateIsBefore(
                    string.Format(Strings.TimeInterval_28days, Becomes28On),
                    Becomes28On,
                    ref error);
            }
            else
            {
                splitter.ValidateIsBefore(
                    Strings.DateTime_Now,
                    nowVal,
                    ref error);
            }
        }
        DateTimeErrorString ValidateDeathOrLastContactDateTime(DateTime? now = null)
        {
            switch(OutcomeAt28Days)
            {
                case OutcomeAt28DaysOption.Missing:
                case OutcomeAt28DaysOption.InpatientAt28Days:
                case OutcomeAt28DaysOption.DischargedBefore28Days:
                case OutcomeAt28DaysOption.DischargedAndKnownToHaveSurvived:
                    return new DateTimeErrorString();
            }
            DateTimeErrorString error = _deathOrLastContactDateTime.ValidateNotEmpty();
            ValidateIsBetweenRegistrationAndNowOr28(ref error, _deathOrLastContactDateTime, now);
            switch (OutcomeAt28Days)
            {
                case OutcomeAt28DaysOption.DischargedAndKnownToHaveDied:
                case OutcomeAt28DaysOption.DischargedAndLikelyToHaveDied:
                case OutcomeAt28DaysOption.DischargedAndLikelyToHaveSurvived:
                    var dischargeAt = _dischargeDateTime.DateAndTime;
                    if (dischargeAt.HasValue)
                    {
                        _deathOrLastContactDateTime.ValidateIsAfter(Strings.ParticipantModel_Describe_DateTimeDischarge,
                            dischargeAt.Value,
                            ref error);
                    }
                    break;
            }
            return error;

        }
        #endregion //validation

        #region Methods
        static double RateOfChange(double startingValue,double finishingValue ,double timeUnitsElapsed)
        {
            return Math.Pow(2, Math.Log(finishingValue / startingValue, 2) / timeUnitsElapsed); // work in log 2 as ? performance advantage for binary systems
        }
        #endregion

        #region Static Methods
        static OutcomeAt28DaysOption[] DeathOrLastContactRequiredIf = new OutcomeAt28DaysOption[]
        {
            OutcomeAt28DaysOption.DiedInHospitalBefore28Days,
            OutcomeAt28DaysOption.DischargedAndKnownToHaveDied,
            OutcomeAt28DaysOption.DischargedAndLikelyToHaveDied,
            OutcomeAt28DaysOption.DischargedAndLikelyToHaveSurvived
        };
        #endregion

        #region DataRequired
        internal static Expression<Func<IParticipant, DataRequiredOption>> GetDataRequired()
        {
            DateTime born28Prior = DateTime.Now.AddDays(-28);
            return
                p => ((p.OutcomeAt28Days >= OutcomeAt28DaysOption.DischargedBefore28Days && !p.DischargeDateTime.HasValue)
                            || (DeathOrLastContactRequiredIf.Contains(p.OutcomeAt28Days) && (p.DeathOrLastContactDateTime==null || p.CauseOfDeath == CauseOfDeathOption.Missing)))
                        ? DataRequiredOption.DetailsMissing
                        :(p.IsInterventionArm && !p.VaccinesAdministered.Any(v=>v.Id == DataContextInitialiser.Bcg.Id))
                            ? DataRequiredOption.BcgDataRequired
                            : (p.OutcomeAt28Days == OutcomeAt28DaysOption.Missing)
                                ? (p.DateTimeBirth > born28Prior) //in sql server DbFunctions.DiffDays(DateTimeBirth, DateTime.Now) < 28
                                    ? DataRequiredOption.AwaitingOutcomeOr28
                                    : DataRequiredOption.OutcomeRequired
                                :DataRequiredOption.Complete;
        }
        #endregion
    }
}
