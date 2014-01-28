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
        public const int MinGestAgeBirth = 24;
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
                "DateOfEnrollment",
                "TimeOfEnrollment",
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
                if (string.IsNullOrWhiteSpace(value)) { _hospitalIdentifier = null; }
                else { _hospitalIdentifier = value.ToUpper(); }
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
        DateTimeSplitter _dateTimeEnrollmentSplitter = new DateTimeSplitter();
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
        public DateTime? DateTimeOfEnrollment
        {
            get { return _dateTimeEnrollmentSplitter.DateAndTime; }
            set { _dateTimeEnrollmentSplitter.DateAndTime = value; }
        }
        public DateTime? DateOfEnrollment
        {
            get { return _dateTimeEnrollmentSplitter.Date; }
            set { _dateTimeEnrollmentSplitter.Date = value; }
        }
        public TimeSpan? TimeOfEnrollment
        {
            get { return _dateTimeEnrollmentSplitter.Time; }
            set { _dateTimeEnrollmentSplitter.Time = value; }
        }
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
        private bool AgeOkToRandomise(DateTime? now = null)
        {
            if (DateTimeBirth == null) { return false; }
            var enrol = DateTimeOfEnrollment ?? now ?? DateTime.Now;
            var age = enrol -this.DateTimeBirth.Value;
            return age.Days < MaxAgeDaysEnrol && age.TotalMinutes >= MinEnrolAgeMins;
        }
        #endregion

        #region Validation

        public override bool IsValid()
        {
            DateTime now = DateTime.Now;
            foreach (string property in _validatedProperties)
            {
                if (GetValidationError(property, true, now) != null)
                {
                    return false;
                }
            }
            return true;
        }
        public override string GetValidationError(string propertyName)
        {
            return GetValidationError(propertyName, false);
        }

        public string GetValidationError(string propertyName, bool hardErrorsOnly, DateTime? now=null)
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
                    error = this.ValidateDob(now).TimeError;
                    break;
                case "DateOfBirth":
                    error = this.ValidateDob(now).DateError;
                    break;
                case "TimeOfEnrollment":
                    error = this.ValidateEnrollment(now).TimeError;
                    break;
                case "DateOfEnrollment":
                    error = this.ValidateEnrollment(now).DateError;
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
                    error = ValidateEnvelopeNumber(hardErrorsOnly);
                    break;
                default:
                    Debug.Fail("Unexpected property being validated on NewPatient: " + propertyName);
                    break;
            }

            return error;
        }
        string ValidateName()
        {
            if (RefusedConsent==false) //changed from oktorandmise
            {
                return ValidateFieldNotEmpty(Name);
            }
            return null;
        }
        string ValidateMothersName()
        {
            if (RefusedConsent == false)
            {
                return ValidateFieldNotEmpty(MothersName);
            }
            return null;
        }
        string ValidateEnvelopeNumber(bool hardErrorsOnly)
        {
            if (!(IsEnvelopeRandomising && OkToRandomise())) 
            {
                return null; 
            }
            if (MultipleSiblingId==null && EnvelopeNumber==null)
            {
                return Strings.Field_Error_Empty;
            }
            if (MultipleSiblingId.HasValue)
            {
                return EnvelopeNumber.HasValue?Strings.NewPatient_Error_TwinAndEnvelope:null;
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
                if (!hardErrorsOnly)
                {
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
                return (RefusedConsent==false)?Strings.Field_Error_Empty:null; //had been OkToRandomise
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
            if (this.AdmissionWeight < MinBirthWeightGrams || this.AdmissionWeight > MaxBirthWeightGrams)
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
        DateTimeErrorString ValidateDob(DateTime? now=null)
        {
            var error = _dateTimeBirthSplitter.ValidateNotEmpty();
            DateTime nowVal = now ?? DateTime.Now;
            _dateTimeBirthSplitter.ValidateIsBefore(Strings.DateTime_Now, nowVal, ref error);
            if (error.DateError == null && DateTimeBirth.HasValue)
            {
                var age = nowVal - DateTimeBirth.Value;
                if (age.Days > MaxAgeDaysScreen)
                {
                    error.DateError = Strings.NewPatient_Error_DOBtooOldToScreen;
                }
                else if (RefusedConsent==false && !AgeOkToRandomise(nowVal))
                {
                    error.DateError = Strings.NewPatient_Error_DOBtooOldToRandomise;
                }
            }
            return error;
        }
        DateTimeErrorString ValidateEnrollment(DateTime? now = null)
        {
            if (!(IsEnvelopeRandomising && OkToRandomise())) 
            { 
                return new DateTimeErrorString(); 
            }
            var error = _dateTimeEnrollmentSplitter.ValidateNotEmpty();
            _dateTimeEnrollmentSplitter.ValidateIsAfter(Strings.DateOfBirth, DateTimeBirth.Value, ref error);
            _dateTimeEnrollmentSplitter.ValidateIsBefore(Strings.DateTime_Now, now ?? DateTime.Now, ref error);
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
