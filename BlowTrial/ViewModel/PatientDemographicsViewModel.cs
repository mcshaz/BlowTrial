using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using BlowTrial.Properties;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using BlowTrial.Helpers;
using StatsForAge.DataSets;
using System.Windows.Threading;
using BlowTrial.Infrastructure.Extensions;
using BlowTrial.Domain.Outcomes;

namespace BlowTrial.ViewModel
{
    public class PatientDemographicsViewModel:CrudWorkspaceViewModel, IDataErrorInfo
    {        
        #region Fields

        private PatientDemographicsModel _patient;
        private bool _isEnrollmentDateTimeAssigned;

        #endregion // Fields

        #region Constructor

        public PatientDemographicsViewModel(IRepository repository, PatientDemographicsModel patient) : base(repository)
        {
            base.DisplayName = Strings.NewPatientVM_DisplayName;
            if (patient == null)
            {
                throw new ArgumentNullException("patient");
            }

            _patient = patient;
            HasSiblingEnrolled = patient.MultipleSiblingId.HasValue;
            RandomiseCmd = new RelayCommand(Randomise, CanRandomise);
            ClearAllCmd = new RelayCommand(ClearAllFields, CanClear);
            AddScreenCmd = new RelayCommand(AddScreen, CanScreen);
            CloseWindowCmd = new RelayCommand(param=>OnRequestClose(), param=>true);
            UpdateDemographicsCmd = new RelayCommand(UpdateDemographics, obj => WasValidOnLastNotify);
            _repository.StudySiteAddOrUpdate += repository_StudySiteAddOrUpdate;
        }

        void repository_StudySiteAddOrUpdate(object sender, EventArgs e)
        {
            _studyCentreOptions = null;
            NotifyPropertyChanged("StudyCentreOptions");
        }
        #endregion

        #region Properties
        public bool RecordAltered
        {
            get;
            private set;
        }
        public bool WasEnvelopeRandomised 
        { 
            get
            {
                return _patient.WasEnvelopeRandomised;
            }
        }
        public int? EnvelopeNumber
        {
            get
            {
                return _patient.EnvelopeNumber;
            }
            set
            {
                if (value == _patient.EnvelopeNumber) { return; }
                _patient.EnvelopeNumber = value;
                RecordAltered = true;
                NotifyPropertyChanged("EnvelopeNumber");
            }
        }
        
        public StudyCentreModel StudyCentre
        {
            get
            {
                return _patient.StudyCentre;
            }
            set
            {
                if (value == _patient.StudyCentre) { return; }
                _patient.StudyCentre = value;
                RecordAltered = true;
                NotifyPropertyChanged("StudyCentre", "BackgroundBrush", "TextBrush", "HospitalIdentifierMask", "PhoneMask", "IsInborn", "AdmissionDiagnosis");
            }
        }
        public string HospitalIdentifierMask
        {
            get { return (StudyCentre==null)?string.Empty:StudyCentre.HospitalIdentifierMask; }
        }
        public string PhoneMask
        {
            get { return (StudyCentre==null)?string.Empty:StudyCentre.PhoneMask; }
        }
        static readonly Brush _defaultBackground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        static readonly Brush _defaultText = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        public Brush BackgroundBrush
        {
            get 
            { 
                if (StudyCentre==null)
                {
                    return _defaultBackground;
                }
                return StudyCentre.BackgroundColour;
            }
        }
        public Brush TextBrush
        {
            get
            {
                if (StudyCentre == null)
                {
                    return _defaultText;
                }
                return StudyCentre.TextColour;
            }
        }
        public string Name
        {
	        get
	        {
		        return _patient.Name;
	        }
	        set
	        {
		        if (value == _patient.Name) { return; }
                _patient.Name = value;
                RecordAltered = true;
                NotifyPropertyChanged("Name", "DisplayName", "OkToRandomise", "EnvelopeNumber");
	        }
        }
        public string HospitalIdentifier
        {
	        get
	        {
		        return _patient.HospitalIdentifier;
	        }
	        set
	        {
		        if (value == _patient.HospitalIdentifier) { return; }
                _patient.HospitalIdentifier = value;
                RecordAltered = true;
                NotifyPropertyChanged("HospitalIdentifier", "DisplayName", "OkToRandomise", "EnvelopeNumber");
	        }
        }
        public string PhoneNumber
        {
            get
            {
                return _patient.PhoneNumber;
            }
            set
            {
                if (value == _patient.PhoneNumber) { return; }
                _patient.PhoneNumber = value;
                RecordAltered = true;
                NotifyPropertyChanged("PhoneNumber");
            }
        }
        public MaternalBCGScarStatus MaternalBCGScar
        {
            get
            {
                return _patient.MaternalBCGScar;
            }
            set
            {
                if (value == _patient.MaternalBCGScar) { return; }
                _patient.MaternalBCGScar = value;
                RecordAltered = true;
                NotifyPropertyChanged("MaternalBCGScar");
            }
        }

        public bool HasNoPhone
        {
            get
            {
                return _patient.HasNoPhone;
            }
            set
            {
                if (value == _patient.HasNoPhone) { return; }
                _patient.HasNoPhone = value;
                NotifyPropertyChanged("PhoneNumber", "HasNoPhone");
            }
        }
        public string MothersName
        {
            get
            {
                return _patient.MothersName;
            }
            set
            {
                if (value == _patient.MothersName) { return; }
                _patient.MothersName = value;
                RecordAltered = true;
                NotifyPropertyChanged("MothersName");
            }
        }
        public int? AdmissionWeight
        {
	        get
	        {
		        return _patient.AdmissionWeight;
	        }
	        set
	        {
		        if (value == _patient.AdmissionWeight) { return; }
                _patient.AdmissionWeight = value;
                RecordAltered = true;
                NotifyPropertyChanged("AdmissionWeight", "OkToRandomise", "EnvelopeNumber", "IsConsentRequired");
                CalculateWtCentile();
	        }
        }

        public bool? IsInborn
        {
            get
            {
                return _patient.IsInborn;
            }
            set
            {
                if (value == _patient.IsInborn) { return; }
                _patient.IsInborn = value;
                RecordAltered = true;
                NotifyPropertyChanged("IsInborn");
            }
        }

        public int? GestAgeWeeks
        {
	        get
	        {
		        return _patient.GestAgeWeeks;
	        }
	        set
	        {
		        if (value == _patient.GestAgeWeeks) { return; }
                _patient.GestAgeWeeks = value;
                RecordAltered = true;
                NotifyPropertyChanged("GestAgeWeeks", "GestAgeDays", "OkToRandomise", "MultipleSiblingId");
                CalculateWtCentile();
	        }
        }
        public int? GestAgeDays
        {
	        get
	        {
		        return _patient.GestAgeDays;
	        }
	        set
	        {
		        if (value == _patient.GestAgeDays) { return; }
                _patient.GestAgeDays = value;
                RecordAltered = true;
                NotifyPropertyChanged("GestAgeWeeks", "GestAgeDays", "MultipleSiblingId");
                CalculateWtCentile();
	        }
        }

        public string AdmissionDiagnosis
        {
	        get
	        {
		        return _patient.AdmissionDiagnosis;
	        }
	        set
	        {
		        if (value == _patient.AdmissionDiagnosis) { return; }
                _patient.AdmissionDiagnosis = value;
                RecordAltered = true;
                NotifyPropertyChanged("AdmissionDiagnosis");
	        }
        }

        public bool? IsMale
        {
	        get
	        {
		        return _patient.IsMale;
	        }
	        set
	        {
		        if (value == _patient.IsMale) { return; }
                _patient.IsMale = value;
                RecordAltered = true;
                NotifyPropertyChanged("IsMale", "OkToRandomise");
                CalculateWtCentile();
                ValidateSiblingId();
	        }
        }
        public DateTime? DateOfBirth
        {
	        get
	        {
                return _patient.DateOfBirth;
	        }
	        set
	        {
                if (value == _patient.DateOfBirth) { return; }
                _patient.DateOfBirth = value;
                RecordAltered = true;
                NotifyPropertyChanged("DateOfBirth", "EnvelopeNumber", "TimeOfBirth", "MultipleSiblingId");
                if (WasEnvelopeRandomised) 
                {
                    if (!_isEnrollmentDateTimeAssigned)
                    {
                        _patient.DateOfEnrollment = value.Value;
                    }
                    NotifyPropertyChanged("DateOfEnrollment", "TimeOfEnrollment");
                }
                NotifyPropertyChanged("OkToRandomise");
	        }
        }
        public TimeSpan? TimeOfBirth
        {
            get
            {
                return _patient.TimeOfBirth;
            }
            set
            {
                if (value == _patient.TimeOfBirth) { return; }
                _patient.TimeOfBirth = value;
                RecordAltered = true;
                NotifyPropertyChanged("TimeOfBirth", "IsYoungerThanMinEnrolTime", "MultipleSiblingId");
                if (WasEnvelopeRandomised)
                {
                    var dob = _patient.DateTimeBirth;
                    var enrol = _patient.DateTimeOfEnrollment;
                    if (dob.HasValue && !_isEnrollmentDateTimeAssigned)
                    {
                        DateTime defaultEnrol = new DateTime[] {dob.Value.AddHours(2), DateTime.Now}.Min();
                        _patient.DateTimeOfEnrollment = defaultEnrol;
                    }
                    NotifyPropertyChanged("DateOfEnrollment", "TimeOfEnrollment");
                }
                NotifyPropertyChanged("OkToRandomise");
            }
        }
        public DateTime? DateOfEnrollment
        {
            get
            {
                return _patient.DateOfEnrollment;
            }
            set
            {
                if (value == _patient.DateOfEnrollment) { return; }
                _patient.DateOfEnrollment = value;
                RecordAltered = true;
                _isEnrollmentDateTimeAssigned = true;
                NotifyPropertyChanged("DateOfEnrollment", "TimeOfEnrollment", "OkToRandomise");
            }
        }
        public TimeSpan? TimeOfEnrollment
        {
            get
            {
                return _patient.TimeOfEnrollment;
            }
            set
            {
                if (value == _patient.TimeOfEnrollment) { return; }
                _patient.TimeOfEnrollment = value;
                RecordAltered = true;
                _isEnrollmentDateTimeAssigned = true;
                NotifyPropertyChanged("DateOfEnrollment", "TimeOfEnrollment", "DateOfBirth", "TimeOfBirth");
            }
        }
        public bool IsYoungerThanMinEnrolTime
        {
            get
            {
                if (_patient.DateTimeBirth==null) {return false;}
                var now = DateTime.Now;
                var age = (now - _patient.DateTimeBirth.Value);
                if (age.Ticks < PatientDemographicsModel.MinEnrolAgeTicks)
                {
                    if (age.Ticks >= 0)
                    {
                        var timer = new DispatcherTimer(DispatcherPriority.Normal);
                        timer.Interval = new TimeSpan(_patient.DateTimeBirth.Value.Ticks + PatientDemographicsModel.MinEnrolAgeTicks - now.Ticks);
                        timer.Tick += timer_Tick;
                        timer.Start();
                        return true;
                    }
                }
                return false;
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            NotifyPropertyChanged("IsYoungerThanMinEnrolTime");
        }

        public bool? LikelyDie24Hr
        {
	        get
	        {
		        return _patient.LikelyDie24Hr;
	        }
	        set
	        {
		        if (value == _patient.LikelyDie24Hr) { return; }
                _patient.LikelyDie24Hr = value;
                RecordAltered = true;
                NotifyPropertyChanged("LikelyDie24Hr", "IsConsentRequired");
	        }
        }
        public bool? BadMalform
        {
	        get
	        {
		        return _patient.BadMalform;
	        }
	        set
	        {
		        if (value == _patient.BadMalform) { return; }
                _patient.BadMalform = value;
                RecordAltered = true;
                NotifyPropertyChanged("BadMalform", "IsConsentRequired");
	        }
        }
        public bool? BadInfectnImmune
        {
	        get
	        {
		        return _patient.BadInfectnImmune;
	        }
	        set
	        {
		        if (value == _patient.BadInfectnImmune) { return; }
                _patient.BadInfectnImmune = value;
                RecordAltered = true;
                NotifyPropertyChanged("BadInfectnImmune", "IsConsentRequired");
	        }
        }
        public bool? WasGivenBcgPrior
        {
	        get
	        {
		        return _patient.WasGivenBcgPrior;
	        }
	        set
	        {
		        if (value == _patient.WasGivenBcgPrior) { return; }
                _patient.WasGivenBcgPrior = value;
                RecordAltered = true;
		        NotifyPropertyChanged("WasGivenBcgPrior", "IsConsentRequired");
	        }
        }
        public bool IsConsentRequired
        {
            get
            {
                NotifyPropertyChanged("OkToRandomise", "EnvelopeNumber");
                return (LikelyDie24Hr==false &&
                    BadMalform==false && 
                    BadInfectnImmune==false &&
                    WasGivenBcgPrior==false &&
                    GetValidationError("AdmissionWeight")==null &&
                    GetValidationError("DateOfBirth")==null &&
                    GetValidationError("TimeOfBirth")==null);
            }
        }
        public bool? RefusedConsent
        {
	        get
	        {
		        return _patient.RefusedConsent;
	        }
	        set
	        {
		        if (value == _patient.RefusedConsent) { return; }
                _patient.RefusedConsent = value;
                RecordAltered = true;
                NotifyPropertyChanged("RefusedConsent", "OkToRandomise", "EnvelopeNumber", "Name", "MothersName", "PhoneNumber", "DateOfBirth");
	        }
        }
        public int? MultipleSiblingId
        {
            get
            {
                return _patient.MultipleSiblingId;
            }
            set
            {
                if (value == _patient.MultipleSiblingId) { return; }
                _patient.MultipleSiblingId = value;
                _multipleSibling = null;
                RecordAltered = true;
                NotifyPropertyChanged("MultipleSiblingId", "EnvelopeNumber");
            }
        }
        bool _hasSiblingEnrolled;
        public bool HasSiblingEnrolled
        {
            get
            {
                return _hasSiblingEnrolled;
            }
            set
            {
                if (value == _hasSiblingEnrolled) { return; }
                _hasSiblingEnrolled = value;
                NotifyPropertyChanged("HasSiblingEnrolled", "MultipleSiblingId");
            }
        }
        public bool OkToRandomise
        {
            get
            {
                return _patient.OkToRandomise();
            }
        }
        public override string DisplayName
        {
            get
            {
                if (!(string.IsNullOrWhiteSpace(_patient.Name) && string.IsNullOrWhiteSpace(_patient.HospitalIdentifier)))
                {
                    return string.Format("{0}:{1}({2})", Strings.MainWindowViewModel_Command_RegisterNewPatient, _patient.Name, _patient.HospitalIdentifier);
                }
                return Strings.MainWindowViewModel_Command_RegisterNewPatient;
            }
        }
        private string _wtForAgeCentile;
        public string WtForAgeCentile
        {
            get
            {
                return _wtForAgeCentile;
            }
            private set
            {
                if (value == _wtForAgeCentile) { return; }
                _wtForAgeCentile = value;
                NotifyPropertyChanged("WtForAgeCentile");
            }
        }
        string _siblingNote;
        public string SiblingNote
        {
            get
            {
                return _siblingNote;
            }
            private set
            {
                if (value == _siblingNote) { return; }
                _siblingNote = value;
                NotifyPropertyChanged("SiblingNote");
            }
        }
        Participant _multipleSibling;
        Participant MultipleSibling
        {
            get
            {
                if (_multipleSibling == null && MultipleSiblingId.HasValue)
                {
                    _multipleSibling = _repository.FindParticipant(MultipleSiblingId.Value);
                }
                return _multipleSibling;
            }
        }

        public DateTime Today
        {
            get { return DateTime.Today; }
        }
        public DateTime MinScreeningDate
        {
            get { return DateTime.Today.AddDays(-PatientDemographicsModel.MaxAgeDaysScreen); }
        }

        #endregion // Properties

        #region Private Methods
        void CalculateWtCentile()
        {
            if (AdmissionWeight == null || AdmissionWeight < PatientDemographicsModel.MinBirthWeightGrams || IsMale==null || GestAgeWeeks==null || GestAgeWeeks < PatientDemographicsModel.MinGestAgeBirth)
            {
                WtForAgeCentile = string.Empty;
            }
            else
            {
                var weightData = new UKWeightData();
                WtForAgeCentile = string.Format(Strings.NewPatientVM_Centile ,
                    weightData.CumSnormForAge((double)AdmissionWeight.Value / 1000, 0, IsMale.Value, (double)GestAgeWeeks.Value + (double)(GestAgeDays ?? 0)/7));
            }
        }
        void UpdateDateNotified(object args)
        {
            NotifyPropertyChanged("Tomorrow", "MinScreeningDate");
        }
        #endregion // Private Methods

        #region Listbox Options
        KeyDisplayNamePair<bool?>[] _requiredBoolOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public KeyDisplayNamePair<bool?>[] RequiredBoolOptions
        {
            get
            {
                return _requiredBoolOptions ?? (_requiredBoolOptions = CreateBoolPairs());
            }
        }

        KeyDisplayNamePair<bool?>[] _isInbornOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public KeyDisplayNamePair<bool?>[] IsInbornOptions
        {
            get
            {
                return _isInbornOptions ?? (_isInbornOptions = CreateBoolPairs(Strings.NewPatientViewModel_Option_InbornTrue,Strings.NewPatientViewModel_Option_InbornFalse));
            }
        }

        KeyDisplayNamePair<bool?>[] _isMaleOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public KeyDisplayNamePair<bool?>[] IsMaleOptions
        {
            get
            {
                return _isMaleOptions ?? (_isMaleOptions = CreateBoolPairs("Male","Female"));
            }
        }
        KeyDisplayNamePair<bool?>[] _likelyDie24HrOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public KeyDisplayNamePair<bool?>[] LikelyDie24HrOptions
        {
            get
            {
                return _likelyDie24HrOptions ?? (_likelyDie24HrOptions = CreateBoolPairs(Strings.NewPatient_IsLikelyDie24, Strings.NewPatient_NotLikelyDie24));
            }
        }
        KeyDisplayNamePair<bool?>[] _refusedConsentOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public KeyDisplayNamePair<bool?>[] RefusedConsentOptions
        {
            get
            {
                return _refusedConsentOptions ?? (_refusedConsentOptions = CreateBoolPairs(Strings.NewPatient_ConsentRefused, Strings.NewPatient_ConsentObtained));
            }
        }
        KeyDisplayNamePair<bool?>[] _wasGivenBcgPriorOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public KeyDisplayNamePair<bool?>[] WasGivenBcgPriorOptions
        {
            get
            {
                return _wasGivenBcgPriorOptions ?? (_wasGivenBcgPriorOptions = CreateBoolPairs(Strings.NewPatient_BCGgiven, Strings.NewPatient_BCGnotGiven));
            }
        }

        IEnumerable<KeyDisplayNamePair<MaternalBCGScarStatus>> _maternalBCGScarOptions;
        public IEnumerable<KeyDisplayNamePair<MaternalBCGScarStatus>> MaternalBCGScarOptions
        {
            get
            {
                if (_maternalBCGScarOptions == null)
                {
                    var returnVar = EnumToListOptions<MaternalBCGScarStatus>();
                    returnVar[0].Value = Strings.OptionMissing;
                    _maternalBCGScarOptions = returnVar;
                }
                return _maternalBCGScarOptions;
            }
        }

        IEnumerable<KeyDisplayNamePair<StudyCentreModel>> _studyCentreOptions;
        public IEnumerable<KeyDisplayNamePair<StudyCentreModel>> StudyCentreOptions
        {
            get
            {
                if (_studyCentreOptions==null)
                {
                    var returnVar = new List<KeyDisplayNamePair<StudyCentreModel>>(
                        from s in _repository.LocalStudyCentres 
                        where s.IsCurrentlyEnrolling
                        select new KeyDisplayNamePair<StudyCentreModel>(s, s.Name));
                    if (returnVar.Skip(1).Any())
                    {
                        returnVar.Insert(0,
                            new KeyDisplayNamePair<StudyCentreModel>(null, Strings.DropDownList_PleaseSelect));
                    }
                    else
                    {
                        StudyCentre = returnVar.First().Key;
                    }
                    _studyCentreOptions = returnVar;
                }
                return _studyCentreOptions;
            }
        }
        #endregion // Listbox options

        #region Commands
        public RelayCommand RandomiseCmd{get; private set;}
        public bool IsNewRecord
        {
	        get 
	        { 
		         return _patient.IsNewRecord;
            }
        }
        public bool CanRandomise(object parameter)
        {
            return IsNewRecord && WasValidOnLastNotify && _patient.OkToRandomise();
        }
        const string CancelString = "userCancelled";
        string ValidateEnvelopeSoftErrors()
        {
            if (!IsValid())
            {
                throw new InvalidOperationException("Can only validate soft errors after hard errors have been removed");
            }
            if (!WasEnvelopeRandomised)
            {
                return null;
            }
            string envelopeSoftError = ((IDataErrorInfo)_patient)["EnvelopeNumber"];
            if (envelopeSoftError != null)
            {
                string userMsg = string.Format(Strings.NewPatientViewModel_SoftError_Envelope, envelopeSoftError);
                var result = MessageBox.Show(userMsg, Strings.NewPatientViewModel_SoftError_WarningMsg, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Cancel)
                {
                    return CancelString;
                }
                Envelope envelope = EnvelopeDetails.GetEnvelope(EnvelopeNumber.Value);
                int bottomWeightRange = envelope.WeightLessThan == 1000 ?
                    0 :
                    envelope.WeightLessThan - 500;
                return string.Format("Randomised incorrectly - envelope {0} was designated for a {1}-{2}g {3}, but patient is a {4}g {5}",
                    EnvelopeNumber,
                    bottomWeightRange,
                    envelope.WeightLessThan - 1,
                    envelope.IsMale ? "Male" : "Female",
                    AdmissionWeight,
                    IsMale.Value ? "Male" : "Female");
            }
            return null;
        }
        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        /// 
        public void Randomise(object parameter)
        {
#if DEBUG
            if (!IsValid() || !_patient.OkToRandomise())
            {
                throw new InvalidOperationException("Underlying NewPatientModel does not validate");
            }
#endif
            string violationString = null;
            if (WasEnvelopeRandomised) 
            {
                violationString = ValidateEnvelopeSoftErrors();
                if (violationString == CancelString)
                {
                    return;
                }
            }

            var newParticipant = _repository.AddParticipant(
                _patient.Name,
                _patient.MothersName,
                _patient.HospitalIdentifier,
                _patient.AdmissionWeight.Value,
                _patient.GestAgeBirth.Value,
                _patient.DateTimeBirth.Value,
                _patient.AdmissionDiagnosis,
                _patient.HasNoPhone?null:_patient.PhoneNumber,
                _patient.IsMale.Value,
                _patient.IsInborn,
                _patient.DateTimeOfEnrollment ?? DateTime.Now,
                StudyCentre.Id,
                _patient.MaternalBCGScar,
                HasSiblingEnrolled?_patient.MultipleSiblingId:null,
                WasEnvelopeRandomised?_patient.EnvelopeNumber:null
                );
            if (violationString !=null)
            {
                var violation = new ProtocolViolation
                {
                    ViolationType = ViolationTypeOption.MajorWrongAllocation,
                    Details = violationString,
                    ParticipantId = newParticipant.Id,
                };
                _repository.AddOrUpdate(violation);
            }
            if (!false.Equals(parameter)) // for testing purposes, supress dialog
            {
                string userMsg;
                if (newParticipant.TrialArm == Domain.Outcomes.RandomisationArm.Control)
                {
                    userMsg = string.Format(Strings.NewPatient_ToControl + ' ', 
                        newParticipant.Name + '(' + newParticipant.HospitalIdentifier + ')', 
                        newParticipant.Id);
                }
                else
                {
                    userMsg = string.Format(Strings.NewPatient_ToIntervention + ' ', 
                        newParticipant.Name + '(' + newParticipant.HospitalIdentifier + ')',
                        newParticipant.Id);
                }
                userMsg += Environment.NewLine + StudyCentre.RandomisedMessage.InstructionsFor(newParticipant.TrialArm);
                MessageBox.Show(userMsg, Strings.NewPatient_SuccesfullyRandomised, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            ClearAllFields();
        }
        public RelayCommand UpdateDemographicsCmd { get; private set; }
        public void UpdateDemographics(object parameter)
        {
            _repository.UpdateParticipant(id: _patient.Id, 
                    name: _patient.Name, 
                    isMale: _patient.IsMale.Value,
                    phoneNumber : _patient.PhoneNumber, 
                    AdmissionDiagnosis: _patient.AdmissionDiagnosis, 
                    admissionWeight: _patient.AdmissionWeight.Value, 
                    dateTimeBirth: _patient.DateTimeBirth.Value, 
                    gestAgeBirth: _patient.GestAgeBirth.Value, 
                    hospitalIdentifier: _patient.HospitalIdentifier, 
                    isInborn: _patient.IsInborn, 
                    multipleSibblingId:_patient.MultipleSiblingId, 
                    maternalBCGScar: _patient.MaternalBCGScar,
                    registeredAt:_patient.DateTimeOfEnrollment.Value,
                    isEnvelopeRandomising: WasEnvelopeRandomised);
            OnRequestClose();
        }
        public RelayCommand CloseWindowCmd { get; private set; }
        public RelayCommand AddScreenCmd { get; private set; }
        public bool CanScreen(object parameter)
        {
            var okToScreen = _patient.OkToScreen();
            if (okToScreen) { RefusedConsent = null; }
            else { okToScreen = RefusedConsent == true; }
            return okToScreen && WasValidOnLastNotify;
        }
        
        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        /// 
        public void AddScreen(object parameter)
        {
            var screenedPt = new ScreenedPatient
            {
                HospitalIdentifier = _patient.HospitalIdentifier.Trim(),
                AdmissionWeight = _patient.AdmissionWeight.Value,
                GestAgeBirth = _patient.GestAgeBirth.Value,
                DateTimeBirth = _patient.DateTimeBirth.Value,
                AdmissionDiagnosis = _patient.AdmissionDiagnosis,
                IsMale = _patient.IsMale.Value,
                Inborn = _patient.IsInborn,
                RegisteredAt = DateTime.Now,
                RegisteringInvestigator = GetCurrentPrincipal().Identity.Name,
                CentreId = StudyCentre.Id,
                BadInfectnImmune = _patient.BadInfectnImmune.Value,
                BadMalform = _patient.BadMalform.Value,
                LikelyDie24Hr = _patient.LikelyDie24Hr.Value,
                RefusedConsent = _patient.RefusedConsent,
                Missed = _patient.Missed,
                WasGivenBcgPrior = _patient.WasGivenBcgPrior.Value
            };
            /*
            if (GetValidationError("HospitalIdentifier",true)!=null)
            {
                log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType).Error(
                    string.Format("duplicate value got through on Id:{0}, okToScreen:{1} , wasValid:{2}, isValid:{3}",
                    screenedPt.HospitalIdentifier,
                    _newPatient.OkToScreen(), WasValidOnLastNotify, IsValid()));
            }
             * */
            _repository.Add(screenedPt);
            //NotifyPropertyChanged("DisplayName");
            ClearAllFields();
        }
        public RelayCommand ClearAllCmd { get; private set; }
        public bool CanClear(object parameter)
        {
            return true;
        }
        /// <summary>
        /// Clears all the fields by creating a new newPatientModel instance.
        /// </summary>
        /// 
        public void ClearAllFields(object parameter)
        {
            MessageBoxResult result = MessageBox.Show(Strings.MessageBox_ConfirmClear, Strings.MessageBox_Warning, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                ClearAllFields();
            }
        }
        private void ClearAllFields()
        {
            _patient = new PatientDemographicsModel { WasEnvelopeRandomised = BlowTrialDataService.IsEnvelopeRandomising() };
            StudyCentre = StudyCentreOptions.First().Key;
            _multipleSibling = null;
            _hasSiblingEnrolled = false;

            _wtForAgeCentile = null;
            NotifyPropertyChanged("Name", "HospitalIdentifier", "AdmissionWeight", "GestAgeDays", "GestAgeWeeks", "IsMale", "DateOfBirth", "TimeOfBirth", "DateOfEnrollment", "TimeOfEnrollment", "LikelyDie24Hr", "BadMalform", "BadInfectnImmune", "WasGivenBcgPrior", "RefusedConsent", "MothersName", "WtForAgeCentile", "PhoneNumber", "IsYoungerThanMinEnrolTime", "EnvelopeNumber", "OkToRandomise", "IsConsentRequired", "HasSiblingEnrolled", "MultipleSiblingId",  "HasNoPhone", "AdmissionDiagnosis", "StudyCentre",  "BackgroundBrush", "TextBrush", "HospitalIdentifierMask", "PhoneMask", "IsInborn");
            RecordAltered = false;
            _isEnrollmentDateTimeAssigned = false;
        }
        #endregion

        #region IDataErrorInfo Members

        public string Error { get { return null; } }

        public string this[string propertyName]
        {
            get 
            { 
                string error = this.GetValidationError(propertyName, false);
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }

        #endregion // IDataErrorInfo Members

        #region Validation

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public override bool IsValid()
        {
            bool returnVal = _patient.IsValid() && !ValidatedProperties.Any(v => this.GetValidationError(v, true) != null);
            CommandManager.InvalidateRequerySuggested();
            return returnVal;
        }

        readonly string[] ValidatedProperties = 
        { 
            "HospitalIdentifier",
            "MultipleSiblingId",
            "EnvelopeNumber"
        };
        protected string GetValidationError(string propertyName, bool hardErrorsOnly = true)
        {
            string error = _patient.GetValidationError(propertyName, hardErrorsOnly);

            if ((error == null && ValidatedProperties.Contains(propertyName)) || (!hardErrorsOnly && propertyName == "EnvelopeNumber"))
            {
                switch (propertyName)
                {
                    case "HospitalIdentifier":
                        error = this.ValidateNewIdentifier();
                        break;
                    case "MultipleSiblingId":
                        error = this.ValidateSiblingId();
                        break;
                    case "EnvelopeNumber":
                        error = this.ValidateEnvelopeNumber() ?? error;
                        break;
                    default:
                        Debug.Fail("Unexpected property being validated on NewPatient: " + propertyName);
                        break;
                }
            }
            return error;
        }
        string ValidateEnvelopeNumber()
        {
            if (!EnvelopeNumber.HasValue) {return null;}
            if (IsNewRecord && _repository.Participants.Any(p=>p.Id == EnvelopeNumber.Value))
            {
                return Strings.NewPatientViewModel_Error_EnvelopeInDb;
            }
            if (MultipleSiblingId.HasValue && MultipleSibling.IsMale == _patient.IsMale)
            {
                return Strings.NewPatient_Error_TwinAndEnvelope;
            }
            return null;
        }
        string ValidateNewIdentifier()
        {
            if (IsNewRecord)
            {
                string currentHospId = _patient.HospitalIdentifier.Trim();
                if (_repository.Participants.Any(p=>p.HospitalIdentifier == currentHospId))
                {
                    return Strings.NewPatientVM_Error_DuplicateEnrol;
                }
                if(_repository.ScreenedPatients.Any(s=>s.HospitalIdentifier == currentHospId))
                {
                    return Strings.NewPatientVM_Error_DuplicateScreen;
                }
            }
            return null;
        }
        const double twinSeperationMaxHrs = 5;
        string ValidateSiblingId()
        {
            if (_hasSiblingEnrolled && !_patient.MultipleSiblingId.HasValue)
            {
                return Strings.NewPatientVM_Error_SiblingIdEmpty;
            }
            if (_patient.MultipleSiblingId.HasValue)
            {
                if (MultipleSibling == null)
                {
                    return Strings.NewPatientVM_Error_SiblingNotFound;
                }

                if (_patient.DateOfBirth.HasValue && _patient.GestAgeBirth.HasValue)
                {
                    var expectedNewPatientGest = ExpectedGestAgeAtBirth(_patient.DateOfBirth.Value, MultipleSibling.DateTimeBirth, MultipleSibling.GestAgeBirth);
                    const double oneDayGestation = 1.0 / 7;
                    if (Math.Abs(expectedNewPatientGest - _patient.GestAgeBirth.Value) > oneDayGestation)
                    {
                        return string.Format(Strings.NewPatient_Error_TwinGestAge, expectedNewPatientGest.ToCgaString(0));
                    }
                }
                /*
                 * www.dailymail.co.uk/health/article-2316634/Twins-born-87-days-apart-mothers-contractions-simply-STOPPED.html

                if (_newPatient.DateTimeBirth.HasValue &&
                    Math.Abs((MultipleSibling.DateTimeBirth - _newPatient.DateTimeBirth.Value).TotalHours) > twinSeperationMaxHrs)
                {
                    return string.Format(Strings.NewPatient_Error_TwinSeparation, twinSeperationMaxHrs);
                }

                if (_newPatient.GestAgeBirth != MultipleSibling.GestAgeBirth)
                {
                    return Strings.NewPatient_Error_TwinGestAge;
                }
                */
                SiblingNote = (MultipleSibling.IsMale == _patient.IsMale)?string.Empty:Strings.NewPatient_Error_TwinGenderDifferent;
                
            }
            return null;
        }
        #endregion // IDataErrorInfo Members

        static double ExpectedGestAgeAtBirth(DateTime dateOfBirthA, DateTime dateOfBirthB, double gestAgeB)
        {
            return gestAgeB + ((dateOfBirthA.Date - dateOfBirthB.Date).TotalDays/7);
        }
    }
}
