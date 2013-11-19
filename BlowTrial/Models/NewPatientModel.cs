using BlowTrial.Domain.Tables;
using BlowTrial.Helpers;
using BlowTrial.Infrastructure;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlowTrial.Models
{
    public class NewPatientModel : ValidationBase
    {
        #region Constants
        public const int MaxAgeDaysEnrol = 21;
        public const int MaxAgeDaysScreen = 180;
        public const int MinGestAgeBirth = 23;
        public const int MaxGestAgeBirth = 43;
        public const int MinBirthWeightGrams = 350;
        public const int MaxBirthWeightGrams = 1999;
        public const int MinEnrolAgeMins = 60;
        #endregion

        #region Constructors
         
        public NewPatientModel()
        {
            _validatedProperties = new string[]
            { 
                "StudyCentre",
                "Name",
                "MothersName",
                "HospitalIdentifier", 
                "AdmissionWeight", 
                "GestAgeWeeks",
                "GestAgeDays",
                "DateOfBirth",
                "TimeOfBirth",
                "IsMale",
                "LikelyDie24Hr",
                "BadMalform",
                "BadInfectnImmune",
                "WasGivenBcgPrior",
                "RefusedConsent",
                "PhoneNumber",
                "Abnormalities",
                "EnvelopeNumber"
            };
        }
        #endregion //Constructors

        #region Fields
        string _hospitalIdentifier;
        #endregion // Fields

        #region Properties
        public int Id {get; set;}
        public StudyCentreModel StudyCentre { get; set; }
	    public string Name {get;set;}
        public string MothersName { get; set; }
        public string HospitalIdentifier
        {
            get { return _hospitalIdentifier; }
            set
            {
                _hospitalIdentifier = value.ToUpper();
            }
        }
        public string PhoneNumber { get; set; }
	    public int? AdmissionWeight {get;set;}
        int? _envelopeNumber;
        public int? EnvelopeNumber {
            get {return _envelopeNumber;}
            set
            {
                if (value.HasValue && !IsEnvelopeRandomising)
                {
                    throw new InvalidOperationException("Data integrity under threat - envelope value assigned when not envelope randomising");
                }
                _envelopeNumber = value;
            }
        }
        public double GestAgeBirth
        {
            get { return (double)GestAgeWeeks + (double)GestAgeDays / 7; }
            set 
            {
                GestAgeWeeks = (int?)value;
                GestAgeDays = (int?)((value - Math.Floor(value))*7);
            }
        }
	    public int? GestAgeWeeks {get;set;}
        private int? _gestAgeDays;
        public int? GestAgeDays 
        { 
            get {return _gestAgeDays;} 
            set 
            {
                if (value>=7)
                {
                    int weeks=value.Value/7;
                    value = value.Value - weeks * 7;
                    GestAgeWeeks += weeks;
                }
                _gestAgeDays = value;
            }
        }
	    public string Abnormalities {get;set;}
	    public bool? IsMale {get;set;}
        DateTimeSplitter _dateTimeBirthSplitter = new DateTimeSplitter();
	    public DateTime? DateTimeBirth 
        {
            get { return _dateTimeBirthSplitter.DateAndTime; }
            set { _dateTimeBirthSplitter.DateAndTime = value; }
        }
        public DateTime? DateOfBirth
        {
            get { return _dateTimeBirthSplitter.Date; }
            set { _dateTimeBirthSplitter.Date = value; }
        }
        public TimeSpan? TimeOfBirth
        {
            get { return _dateTimeBirthSplitter.Time; }
            set { _dateTimeBirthSplitter.Time = value; }
        }
        public DateTime? DateOfAdmission { get; set; }
        public bool? LikelyDie24Hr { get; set; }
        public bool? BadMalform { get; set; }
        public bool? BadInfectnImmune { get; set; }
        public bool? WasGivenBcgPrior { get; set; }
        public bool? RefusedConsent { get; set; }
        public bool? Missed { get; set; }
        public int? MultipleSiblingId { get; set; }
        bool? _isEnvelopeRandomising;
        public bool IsEnvelopeRandomising
        {
            get
            {
                return (_isEnvelopeRandomising ??
                    (_isEnvelopeRandomising = BlowTrialDataService.IsEnvelopeRandomising())).Value;
            }
        }
        #endregion

        #region Methods

        public bool OkToRandomise()
        {
            return (LikelyDie24Hr == false &&
                BadMalform == false &&
                WasGivenBcgPrior == false &&
                RefusedConsent == false &&
                BadInfectnImmune == false &&
                AgeOkToRandomise());
        }
        public bool OkToScreen()
        {
            return (LikelyDie24Hr==true ||
                BadMalform==true ||
                WasGivenBcgPrior==true ||
                BadInfectnImmune == true);
        }
        private bool AgeOkToRandomise()
        {
            if (DateTimeBirth == null) { return false; }
            var now = DateTime.Now;
            var age = now -this.DateTimeBirth.Value;
            return age.Days < MaxAgeDaysEnrol && age.TotalMinutes >= MinEnrolAgeMins;
        }
        #endregion

        #region Validation

        public override string GetValidationError(string propertyName)
        {
            if (!_validatedProperties.Contains(propertyName))
            { return null; }

            string error = null;

            switch (propertyName)
            {
                case "StudyCentre":
                    error = ValidateStudyCentre();
                    break;
                case "Name":
                    error = ValidateName();
                    break;
                case "MothersName":
                    error = ValidateMothersName();
                    break;
                case "HospitalIdentifier":
                    error = ValidateHospitalId();
                    break;
                case "AdmissionWeight":
                    error = this.ValidateWeight();
                    break;
               case "GestAgeDays":
                    error = this.ValidateGestAgeDays();
                    break;
                case "GestAgeWeeks":
                    error = this.ValidateGestAgeWeeks();
                    break;
                case "TimeOfBirth":
                    error = this.ValidateDob().TimeError;
                    break;
                case "DateOfBirth":
                    error = this.ValidateDob().DateError;
                    break;
                case "IsMale":
                    error = ValidateDDLNotNull(IsMale);
                    break;
                case "LikelyDie24Hr":
                    error = ValidateDDLNotNull(LikelyDie24Hr);
                    break;
                case "BadMalform":
                    error = ValidateDDLNotNull(BadMalform);
                    break;
                case "BadInfectnImmune":
                    error = ValidateDDLNotNull(BadInfectnImmune);
                    break;
                case "WasGivenBcgPrior":
                    error = ValidateDDLNotNull(WasGivenBcgPrior);
                    break;
                case "RefusedConsent":
                    error = ValidateConsent();
                    break;
                case "PhoneNumber":
                    error = ValidatePhoneNumber();
                    break;
                case "Abnormalities":
                    error = ValidateAbnormalities();
                    break;
                case "EnvelopeNumber":
                    error = ValidateEnvelopeNumber();
                    break;
                default:
                    Debug.Fail("Unexpected property being validated on NewPatient: " + propertyName);
                    break;
            }

            return error;
        }
        string ValidateName()
        {
            if (OkToRandomise())
            {
                return ValidateFieldNotEmpty(Name);
            }
            return null;
        }
        string ValidateMothersName()
        {
            if (OkToRandomise())
            {
                return ValidateFieldNotEmpty(MothersName);
            }
            return null;
        }
        string ValidateEnvelopeNumber()
        {
            if (!(IsEnvelopeRandomising && OkToRandomise())) { return null; }
            if (EnvelopeNumber==null)
            {
                return Strings.Field_Error_Empty;
            }
            else if (EnvelopeNumber > StudyCentre.MaxIdForSite || EnvelopeNumber < StudyCentre.Id)
            {
                return string.Format(Strings.NewPatientModel_Error_IdOutOfRangeForSite,  StudyCentre.Id, StudyCentre.MaxIdForSite);
            }
            else
            {
                var envelope = EnvelopeDetails.GetEnvelope(EnvelopeNumber.Value);
                if (envelope==null)
                {
                    return Strings.NewPatientModel_Error_EnvelopeNotFound;
                }
                if (AdmissionWeight >= envelope.WeightLessThan ||
                    (envelope.WeightLessThan != 1000 && AdmissionWeight < envelope.WeightLessThan - 500))
                {
                    if (envelope.IsMale != IsMale)
                    {
                        return Strings.NewPatientModel_Error_EnvelopeDualIncorrect;
                    }
                    return Strings.NewPatientModel_Error_EnvelopeWeightIncorrect;
                }
                if (envelope.IsMale != IsMale)
                {
                    return Strings.NewPatientModel_Error_EnvelopeGenderIncorrect;
                }
            }
            return null;
        }
        string ValidateStudyCentre()
        {
            if (StudyCentre == null)
            {
                return Strings.NewPatientModel_Error_NoStudyCentre;
            }
            return null;
        }
        string ValidateAbnormalities()
        {
            const int maxLength = 512;
            if (Abnormalities != null && Abnormalities.Length>maxLength)
            {
                return string.Format(Strings.Field_Error_TooLong, maxLength);
            }
            return null;
        }
        string ValidatePhoneNumber()
        {
            const int maxLength = 16;
            if (string.IsNullOrEmpty(PhoneNumber))
            {
                return (OkToRandomise())?Strings.Field_Error_Empty:null;
            }
            
            if (PhoneNumber.Length > maxLength)
            {
                return string.Format(Strings.Field_Error_TooLong, maxLength);
            }
            return null;
            //to do - put in hospital specific regex & regex messages
        }
        string ValidateHospitalId()
        {
            string error = ValidateFieldNotEmpty(HospitalIdentifier);
            /*
            //not relevant now mask is in use
            if (!Regex.IsMatch(HospitalIdentifier, HospitalIdFormat))
            {
                return Strings.NewPatient_Error_InvalidHospitalIdentifier;
            }
            */
            return error;
        }

        string ValidateWeight()
        {
            string returnVal = ValidateFieldNotEmpty(AdmissionWeight);
            if (this.AdmissionWeight < MinBirthWeightGrams || this.AdmissionWeight >= MaxBirthWeightGrams)
            {
                returnVal = Strings.NewPatient_Error_InvalidWeight;
            }
            return returnVal;
        }

        string ValidateGestAgeWeeks()
        {
            if (GestAgeWeeks==null)
            {
                return Strings.NewPatient_Error_NoGA;
            }
            if (this.GestAgeWeeks.Value < MinGestAgeBirth || this.GestAgeWeeks.Value > MaxGestAgeBirth)
            {
                return Strings.NewPatient_Error_InvalidGestation;
            }
            return null;
        }
        string ValidateGestAgeDays()
        {
            if (GestAgeDays == null)
            {
                if (GestAgeWeeks == null)
                {
                    return Strings.NewPatient_Error_NoGAdaysSuffix;
                }
                else 
                {
                    return Strings.NewPatient_Error_NoGAdaysOnly;
                }
            }
            return null;
        }
        DateTimeErrorString ValidateDob()
        {
            var error = _dateTimeBirthSplitter.ValidateNotEmpty();
            var now = DateTime.Now;
            _dateTimeBirthSplitter.ValidateIsBefore(Strings.DateTime_Now, now, ref error);
            if (error.DateError == null && DateTimeBirth.HasValue)
            {
                var age = now - DateTimeBirth.Value;
                if (age.Days > MaxAgeDaysScreen)
                {
                    error.DateError = Strings.NewPatient_Error_DOBtooOld;
                }
            }
            return error;
        }
        string ValidateConsent()
        {
            if (OkToRandomise() && RefusedConsent==null)
            {
                return Strings.NewPatient_Error_ConsentRequired;
            }
            if(OkToScreen() && RefusedConsent.HasValue)
            {
                return Strings.NewPatient_Error_ConsentIrrelevant;
            }
            return null;
        }
        #endregion // Validation
    }
}
