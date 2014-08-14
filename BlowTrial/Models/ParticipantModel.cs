using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Providers;
using BlowTrial.Domain.Tables;
using BlowTrial.Helpers;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using BlowTrial.Infrastructure.Randomising;
using System.Data.Entity;
using System.Reflection;

namespace BlowTrial.Models
{
    public class ParticipantBaseModel : IParticipant
    {
        #region Fields
        DateTime _dateTimeBirth;
        #endregion

        #region Static Constructor
        static ParticipantBaseModel()
        {
            DataRequiredExpression = p => 
                ((p.OutcomeAt28Days >= OutcomeAt28DaysOption.DischargedBefore28Days && !p.DischargeDateTime.HasValue)
                            || (DeathOrLastContactRequiredIf.Contains(p.OutcomeAt28Days) && (p.DeathOrLastContactDateTime == null || (KnownDeadOutcomes.Contains(p.OutcomeAt28Days) && p.CauseOfDeath == CauseOfDeathOption.Missing))))
                        ? DataRequiredOption.DetailsMissing
                        : (p.TrialArm != RandomisationArm.Control && !p.VaccinesAdministered.Any(v => DataContextInitialiser.BcgVaccineIds.Contains(v.VaccineId)))
                            ? DataRequiredOption.BcgDataRequired
                            : (p.OutcomeAt28Days == OutcomeAt28DaysOption.Missing)
                                ? DbFunctions.DiffDays(p.DateTimeBirth, DateTime.Now) < 28
                                    ? DataRequiredOption.AwaitingOutcomeOr28
                                    : DataRequiredOption.OutcomeRequired
                                : DataRequiredOption.Complete;
            var visitor = new ReplaceMethodCallVisitor(
                typeof(DbFunctions).GetMethod("DiffDays", BindingFlags.Static | BindingFlags.Public, null, new Type[]{ typeof(DateTime?), typeof(DateTime?)},null),
                args => 
                    Expression.Property(Expression.Subtract(args[1], args[0]), "Days"));
            DataRequiredFunc = ((Expression<Func<IParticipant, DataRequiredOption>>)visitor.Visit(DataRequiredExpression)).Compile();
        }
        #endregion


        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsMale { get; set; }
        public string HospitalIdentifier { get; set; }
        public DateTime Becomes28On { get; private set; }
        public DateTime RegisteredAt { get; set; }
        public int CentreId { get; set; }
        public RandomisationArm TrialArm { get; set; }
        public StudyCentreModel StudyCentre { get; set; }
        public CauseOfDeathOption CauseOfDeath { get; set; }
        public OutcomeAt28DaysOption OutcomeAt28Days { get; set; }
        public virtual ICollection<VaccineAdministered> VaccinesAdministered { get; set; }
        public bool UserMarkedFinished { get; set; }

        public DateTime DateTimeBirth 
        { 
            get
            {
                return _dateTimeBirth;
            }
            set
            {
                _dateTimeBirth = value;
                Becomes28On = _dateTimeBirth.AddDays(28);
            }
        }

        public int AgeDays { get; set; }

        internal static string GetTrialArmDescription(RandomisationArm arm)
        {
            switch (arm)
                {
                    case RandomisationArm.DanishBcg:
                        return Strings.Vaccine_DanishBcg;
                    case RandomisationArm.RussianBCG:
                        return Strings.Vaccine_RussianBcg;
                    case RandomisationArm.Control:
                        return Strings.ParticipantUpdateVM_ControlArm;

                }
                throw new InvalidEnumArgumentException(arm.ToString());
        }
        public string TrialArmDescription
        {
            get
            {
                return GetTrialArmDescription(TrialArm);
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

        string ValidateName()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return Strings.Field_Error_Empty;
            }
            return null;
        }

        public DataRequiredOption DataRequired
        {
            get { return DataRequiredFunc(this); }
        }

        internal DateTimeSplitter _deathOrLastContactDateTime = new DateTimeSplitter();
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
                //DeathOrLastContactDateTime = _deathOrLastContactDateTime.DateAndTime;
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
                //DeathOrLastContactDateTime = _deathOrLastContactDateTime.DateAndTime;
            }
        }

        internal DateTimeSplitter _dischargeDateTime = new DateTimeSplitter();

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
                //DischargeDateTime = _dischargeDateTime.DateAndTime;
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
                //DischargeDateTime = _dischargeDateTime.DateAndTime;
            }
        }

        public bool? IsKnownDead
        {
            get
            {
                if (KnownDeadOutcomes.Contains(OutcomeAt28Days))
                {
                    return true;
                }
                switch (OutcomeAt28Days)
                {
                    case OutcomeAt28DaysOption.InpatientAt28Days:
                    case OutcomeAt28DaysOption.DischargedAndKnownToHaveSurvived:
                        return false;
                    default:
                        return null;
                }
            }
        }

        #region Methods
        /*
        public TimeSpan GetAge(DateTime? now = null)
        {
            return ((now ?? DateTime.Now) - DateTimeBirth);
        }
         * */
        #region Static Methods
        protected static OutcomeAt28DaysOption[] DeathOrLastContactRequiredIf = new OutcomeAt28DaysOption[]
        {
            OutcomeAt28DaysOption.DiedInHospitalBefore28Days,
            OutcomeAt28DaysOption.DischargedAndKnownToHaveDied,
            OutcomeAt28DaysOption.DischargedAndLikelyToHaveDied,
            OutcomeAt28DaysOption.DischargedAndLikelyToHaveSurvived,
            OutcomeAt28DaysOption.DischargedAndOutcomeCompletelyUnknown
        };
        protected static OutcomeAt28DaysOption[] KnownDeadOutcomes = new OutcomeAt28DaysOption[]
        {
            OutcomeAt28DaysOption.DiedInHospitalBefore28Days,
            OutcomeAt28DaysOption.DischargedAndKnownToHaveDied
        };

        internal readonly static Expression<Func<IParticipant, DataRequiredOption>> DataRequiredExpression;
        internal readonly static Func<IParticipant, DataRequiredOption> DataRequiredFunc;

        #endregion //Static Methds
        #endregion //Methods
    }

    public class ParticipantProgressModel : ParticipantBaseModel, IDataErrorInfo
    {
        #region Constructors

        #endregion // Constructors

        #region Fields
        const double TicksPerWeek = TimeSpan.TicksPerDay * 7;
        TimeSpan _cgabirth;
        
        #endregion //Fields

        #region Properties

        public string MothersName { get; set; }
        public string PhoneNumber { get; set; }

        public int AdmissionWeight { get; set; }
        public double GestAgeBirth { get; set; }
        public string AdmissionDiagnosis { get; set; }
        
        public string RegisteringInvestigator { get; set; }
        public bool? BcgAdverse { get; set; }
        public string BcgAdverseDetail { get; set; }
        public bool? BcgPapuleAtDischarge { get; set; }
        public bool? BcgPapuleAt28days { get; set; }
        public int? LastContactWeight { get; set; }
        public DateTime? LastWeightDate { get; set; }
        public string OtherCauseOfDeathDetail { get; set; }
        public string Notes { get; set; }

        public ICollection<VaccineAdministeredModel> VaccineModelsAdministered { get; set; }
        

        //purely to implement Ipatient
        public override ICollection<VaccineAdministered> VaccinesAdministered 
        {
            get
            {
                return (VaccineModelsAdministered==null)
                    ?null
                    :VaccineModelsAdministered.Select(v => new VaccineAdministered { VaccineId = v.VaccineId }).ToList();
            }
            set
            {
                //possibly a mistake - will create stack overflow if already inside automapper method.
                VaccineModelsAdministered = AutoMapper.Mapper.Map<List<VaccineAdministeredModel>>(value);
            }
        }
        

        /// <summary>
        /// To implement IParticipant - mapping Id only at this point
        /// </summary>
        /*
        public TimeSpan GetCGA(DateTime? now = null)
        {
            return GetAge(now).Add(CgaBirth);
        }
        */
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
            "BcgAdverse",
            "BcgAdverseDetail",
            "Notes",
            "BcgPapuleAt28days",
        };
        public string GetValidationError(string propertyName, DateTime? now=null)
        {
            if (!ValidatedProperties.Contains(propertyName))
            { return null; }
            string error = null;

            switch (propertyName)
            {
                case "OutcomeAt28Days":
                    error = this.ValidateOutcomeAt28Days();
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
                case "BcgAdverse":
                    error = ValidateBcgAdverse();
                    break;
                case "BcgAdverseDetail":
                    error = ValidateBcgAdverseDetail();
                    break;
                case "Notes":
                    error = ValidateNotes();
                    break;
                case "BcgPapuleAt28days":
                    error = ValidateBcgPapuleAt28d();
                    break;
                /* - not used - reasonable to save info and get further info such as time of discharge later
                case "BcgPapuleAtDischarge":
                    error = ValidateBcgPapuleAtDischarge();
                    break; 
                 */
                default:
                    Debug.Fail("Unexpected property being validated on ParticipantUpdateModel: " + propertyName);
                    break;
            }
            return error;
        }

        string ValidateOutcomeAt28Days()
        {
            if (IsKnownDead==false && AgeDays < 27) //27 not 28 as reasonable to call mother & hear child is doing well just before 28 days
            {
                return Strings.ParticipantModel_Error_Alive28daysNotElapsed;
            }
            return null;
        }
        string ValidateBcgPapuleAt28d()
        {
            if (BcgPapuleAt28days.HasValue)
            {
                if (AgeDays < 28)
                {
                    return Strings.ParticipantModel_Error_Bcg28daysNotElapsed;
                }
                if (!IsKnownDead.HasValue)
                {
                    return Strings.ParticipantModel_Error_Bcg28daysNoOutcome;
                }
                if (IsKnownDead == true)
                {
                    return Strings.ParticipantModel_Error_Bcg28daysDead;
                }
            }
            return null;
        }
        string ValidateBcgPapuleAtDischarge()
        {
            if (BcgPapuleAtDischarge.HasValue && OutcomeAt28Days == OutcomeAt28DaysOption.InpatientAt28Days)
            {
                return Strings.ParticipantModel_Error_BcgNeitherDischOrDead;
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
        string ValidateBcgAdverse()
        {
            if (BcgAdverse.HasValue && !HasBcgRecorded())
            {
                return Strings.ParticipantModel_Error_BcgRequiredForAdverse;
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

        string ValidateWeight()
        {
            if (LastContactWeight.HasValue)
            {
                var weightChange = (double)LastContactWeight / (double)AdmissionWeight;
                var weightCat = RandomisingExtensions.RandomisationCategory(AdmissionWeight, true); // a slight hack making everone male to test weight cats!
                int? daysOldAtWeight = (LastWeightDate.HasValue)?
                    (LastWeightDate.Value-DateTimeBirth.Date).Days
                    :(int?)null;
                if (weightChange < 0.8 || 
                    weightChange > 1.6 ||
                    (weightCat == RandomisationStrata.MidWeightMale && weightChange >1.5) ||
                    (weightCat == RandomisationStrata.SmallestWeightMale && weightChange > 1.4) ||
                    (daysOldAtWeight.HasValue && 
                    (daysOldAtWeight<21 && (weightChange > 1.4 ||
                    (weightCat == RandomisationStrata.MidWeightMale && weightChange > 1.35) ||
                    (weightCat == RandomisationStrata.SmallestWeightMale && weightChange > 1.3))) ||
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
        const int noteLength = 160;
        string ValidateNotes()
        {
            if (Notes!=null && Notes.Length > noteLength)
            {
                return string.Format(Strings.Field_Error_TooLong, noteLength);
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
        public bool HasBcgRecorded()
        {
            return VaccineModelsAdministered.Any(vm => vm.IsBcg);
        }
        #endregion

    }
}
