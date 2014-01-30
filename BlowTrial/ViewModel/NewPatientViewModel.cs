using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using BlowTrial.Properties;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using BlowTrial.Infrastructure.Extensions;
using System.Windows.Media;
using BlowTrial.Helpers;
using StatsForAge.DataSets;

namespace BlowTrial.ViewModel
{
    class NewPatientViewModel:CrudWorkspaceViewModel, IDataErrorInfo
    {        
        #region Fields

        private NewPatientModel _newPatient;
        private bool _isEnrollmentDateTimeAssigned;

        #endregion // Fields

        #region Constructor

        public NewPatientViewModel(IRepository repository, NewPatientModel patient) : base(repository)
        {
            base.DisplayName = Strings.NewPatientVM_DisplayName;
            if (patient == null)
            {
                throw new ArgumentNullException("patient");
            }

            _newPatient = patient;
            RandomiseCmd = new RelayCommand(Randomise, CanRandomise);
            ClearAllCmd = new RelayCommand(ClearAllFields, CanClear);
            AddScreenCmd = new RelayCommand(AddScreen, CanScreen);

        }
        #endregion

        #region Properties
        public bool RecordAltered
        {
            get;
            private set;
        }
        public bool IsEnvelopeRandomising 
        { 
            get
            {
                return _newPatient.IsEnvelopeRandomising;
            }
        }
        public int? EnvelopeNumber
        {
            get
            {
                return _newPatient.EnvelopeNumber;
            }
            set
            {
                if (value == _newPatient.EnvelopeNumber) { return; }
                _newPatient.EnvelopeNumber = value;
                RecordAltered = true;
                NotifyPropertyChanged("EnvelopeNumber");
            }
        }
        
        public StudyCentreModel StudyCentre
        {
            get
            {
                return _newPatient.StudyCentre;
            }
            set
            {
                if (value == _newPatient.StudyCentre) { return; }
                _newPatient.StudyCentre = value;
                RecordAltered = true;
                NotifyPropertyChanged("StudyCentre", "BackgroundBrush", "TextBrush", "HospitalIdentifierMask", "PhoneMask");
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
		        return _newPatient.Name;
	        }
	        set
	        {
		        if (value == _newPatient.Name) { return; }
                _newPatient.Name = value;
                RecordAltered = true;
                NotifyPropertyChanged("Name", "DisplayName", "OkToRandomise", "EnvelopeNumber");
	        }
        }
        public string HospitalIdentifier
        {
	        get
	        {
		        return _newPatient.HospitalIdentifier;
	        }
	        set
	        {
		        if (value == _newPatient.HospitalIdentifier) { return; }
                _newPatient.HospitalIdentifier = value;
                RecordAltered = true;
                NotifyPropertyChanged("HospitalIdentifier", "DisplayName", "OkToRandomise", "EnvelopeNumber");
	        }
        }
        public string PhoneNumber
        {
            get
            {
                return _newPatient.PhoneNumber;
            }
            set
            {
                if (value == _newPatient.PhoneNumber) { return; }
                _newPatient.PhoneNumber = value;
                RecordAltered = true;
                NotifyPropertyChanged("PhoneNumber");
            }
        }
        public string MothersName
        {
            get
            {
                return _newPatient.MothersName;
            }
            set
            {
                if (value == _newPatient.MothersName) { return; }
                _newPatient.MothersName = value;
                RecordAltered = true;
                NotifyPropertyChanged("MothersName");
            }
        }
        public int? AdmissionWeight
        {
	        get
	        {
		        return _newPatient.AdmissionWeight;
	        }
	        set
	        {
		        if (value == _newPatient.AdmissionWeight) { return; }
                _newPatient.AdmissionWeight = value;
                RecordAltered = true;
                NotifyPropertyChanged("AdmissionWeight", "OkToRandomise", "EnvelopeNumber", "IsConsentRequired");
                CalculateWtCentile();
	        }
        }
        public int? GestAgeWeeks
        {
	        get
	        {
		        return _newPatient.GestAgeWeeks;
	        }
	        set
	        {
		        if (value == _newPatient.GestAgeWeeks) { return; }
                _newPatient.GestAgeWeeks = value;
                RecordAltered = true;
                NotifyPropertyChanged("GestAgeWeeks", "GestAgeDays", "OkToRandomise", "MultipleSiblingId");
                CalculateWtCentile();
	        }
        }
        public int? GestAgeDays
        {
	        get
	        {
		        return _newPatient.GestAgeDays;
	        }
	        set
	        {
		        if (value == _newPatient.GestAgeDays) { return; }
                _newPatient.GestAgeDays = value;
                RecordAltered = true;
                NotifyPropertyChanged("GestAgeWeeks", "GestAgeDays", "MultipleSiblingId");
                CalculateWtCentile();
	        }
        }
        ObservableCollection<Abnormality> _abnormalities;
        public ObservableCollection<Abnormality> Abnormalities
        {
	        get
	        {
                if (_abnormalities == null)
                {
                    if (_newPatient.Abnormalities == null)
                    {
                        _abnormalities = new ObservableCollection<Abnormality>();
                    }
                    else
                    {
                        _abnormalities = new ObservableCollection<Abnormality>(_newPatient.Abnormalities.FromSeperatedList(';').Select(s => new Abnormality { Description = s }));
                    }
                    _abnormalities.CollectionChanged += _abnormalities_CollectionChanged;

                }
                return _abnormalities;
	        }
        }

        void _abnormalities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Abnormality i in e.OldItems)
                {
                    i.PropertyChanged -= abnormality_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (Abnormality i in e.NewItems)
                {
                    i.PropertyChanged += abnormality_PropertyChanged;
                }
            }
        }

        void abnormality_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _newPatient.Abnormalities = (from a in _abnormalities
                                         where !string.IsNullOrWhiteSpace(a.Description)
                                         select a.Description).ToSeparatedList(';');
            RecordAltered = true;
            NotifyPropertyChanged("Abnormalities");
        }

        public bool? IsMale
        {
	        get
	        {
		        return _newPatient.IsMale;
	        }
	        set
	        {
		        if (value == _newPatient.IsMale) { return; }
                _newPatient.IsMale = value;
                RecordAltered = true;
                NotifyPropertyChanged("IsMale", "OkToRandomise", "EnvelopeNumber");
                CalculateWtCentile();
	        }
        }
        public DateTime? DateOfBirth
        {
	        get
	        {
                return _newPatient.DateOfBirth;
	        }
	        set
	        {
                if (value == _newPatient.DateOfBirth) { return; }
                _newPatient.DateOfBirth = value;
                RecordAltered = true;
                NotifyPropertyChanged("DateOfBirth", "EnvelopeNumber", "TimeOfBirth", "MultipleSiblingId");
                if (IsEnvelopeRandomising) 
                {
                    if (!_isEnrollmentDateTimeAssigned)
                    {
                        _newPatient.DateOfEnrollment = value.Value;
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
                return _newPatient.TimeOfBirth;
            }
            set
            {
                if (value == _newPatient.TimeOfBirth) { return; }
                _newPatient.TimeOfBirth = value;
                RecordAltered = true;
                NotifyPropertyChanged("TimeOfBirth", "IsYoungerThanMinEnrolTime", "MultipleSiblingId");
                if (IsEnvelopeRandomising)
                {
                    var dob = _newPatient.DateTimeBirth;
                    var enrol = _newPatient.DateTimeOfEnrollment;
                    if (dob.HasValue && !_isEnrollmentDateTimeAssigned)
                    {
                        DateTime defaultEnrol = new DateTime[] {dob.Value.AddHours(2), DateTime.Now}.Min(d=>d);
                        _newPatient.DateTimeOfEnrollment = defaultEnrol;
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
                return _newPatient.DateOfEnrollment;
            }
            set
            {
                if (value == _newPatient.DateOfEnrollment) { return; }
                _newPatient.DateOfEnrollment = value;
                RecordAltered = true;
                _isEnrollmentDateTimeAssigned = true;
                NotifyPropertyChanged("DateOfEnrollment", "TimeOfEnrollment", "OkToRandomise");
            }
        }
        public TimeSpan? TimeOfEnrollment
        {
            get
            {
                return _newPatient.TimeOfEnrollment;
            }
            set
            {
                if (value == _newPatient.TimeOfEnrollment) { return; }
                _newPatient.TimeOfEnrollment = value;
                RecordAltered = true;
                _isEnrollmentDateTimeAssigned = true;
                NotifyPropertyChanged("DateOfEnrollment", "TimeOfEnrollment", "DateOfBirth", "TimeOfBirth");
            }
        }
        public bool IsYoungerThanMinEnrolTime
        {
            get
            {
                if (_newPatient.DateTimeBirth==null) {return false;}
                var ageMins = (DateTime.Now - _newPatient.DateTimeBirth.Value).TotalMinutes;
                return ageMins >= 0 && ageMins < NewPatientModel.MinEnrolAgeMins;
            }
        }
        public bool? LikelyDie24Hr
        {
	        get
	        {
		        return _newPatient.LikelyDie24Hr;
	        }
	        set
	        {
		        if (value == _newPatient.LikelyDie24Hr) { return; }
                _newPatient.LikelyDie24Hr = value;
                RecordAltered = true;
                NotifyPropertyChanged("LikelyDie24Hr", "IsConsentRequired");
	        }
        }
        public bool? BadMalform
        {
	        get
	        {
		        return _newPatient.BadMalform;
	        }
	        set
	        {
		        if (value == _newPatient.BadMalform) { return; }
                _newPatient.BadMalform = value;
                RecordAltered = true;
                NotifyPropertyChanged("BadMalform", "IsConsentRequired");
	        }
        }
        public bool? BadInfectnImmune
        {
	        get
	        {
		        return _newPatient.BadInfectnImmune;
	        }
	        set
	        {
		        if (value == _newPatient.BadInfectnImmune) { return; }
                _newPatient.BadInfectnImmune = value;
                RecordAltered = true;
                NotifyPropertyChanged("BadInfectnImmune", "IsConsentRequired");
	        }
        }
        public bool? WasGivenBcgPrior
        {
	        get
	        {
		        return _newPatient.WasGivenBcgPrior;
	        }
	        set
	        {
		        if (value == _newPatient.WasGivenBcgPrior) { return; }
                _newPatient.WasGivenBcgPrior = value;
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
		        return _newPatient.RefusedConsent;
	        }
	        set
	        {
		        if (value == _newPatient.RefusedConsent) { return; }
                _newPatient.RefusedConsent = value;
                RecordAltered = true;
                NotifyPropertyChanged("RefusedConsent", "OkToRandomise", "EnvelopeNumber", "Name", "MothersName", "PhoneNumber", "DateOfBirth");
	        }
        }
        public int? MultipleSiblingId
        {
            get
            {
                return _newPatient.MultipleSiblingId;
            }
            set
            {
                if (value == _newPatient.MultipleSiblingId) { return; }
                _newPatient.MultipleSiblingId = value;
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
                NotifyPropertyChanged("HasSiblingEnrolled");
            }
        }
        public bool OkToRandomise
        {
            get
            {
                return _newPatient.OkToRandomise();
            }
        }
        public override string DisplayName
        {
            get
            {
                if (!(string.IsNullOrWhiteSpace(_newPatient.Name) && string.IsNullOrWhiteSpace(_newPatient.HospitalIdentifier)))
                {
                    return string.Format("{0}:{1}({2})", Strings.MainWindowViewModel_Command_RegisterNewPatient, _newPatient.Name, _newPatient.HospitalIdentifier);
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
            get { return DateTime.Today.AddDays(-NewPatientModel.MaxAgeDaysScreen); }
        }

        RandomisingMessage _randomisingMessage;
        string ControlArmMessage
        {
            get
            {
                return (_randomisingMessage ?? (_randomisingMessage = BlowTrialDataService.GetRandomisingMessage())).ControlInstructions;
            }
        }
        string InterventionArmMessage
        {
            get
            {
                return (_randomisingMessage ?? (_randomisingMessage = BlowTrialDataService.GetRandomisingMessage())).InterventionInstructions;
            }
        }
        #endregion // Properties

        #region Private Methods
        void CalculateWtCentile()
        {
            if (AdmissionWeight == null || AdmissionWeight < NewPatientModel.MinBirthWeightGrams || IsMale==null || GestAgeWeeks==null || GestAgeWeeks < NewPatientModel.MinGestAgeBirth)
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
        KeyValuePair<bool?, string>[] _requiredBoolOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public KeyValuePair<bool?,string>[] RequiredBoolOptions
        {
            get
            {
                return _requiredBoolOptions ?? (_requiredBoolOptions = CreateBoolPairs());
            }
        }
        KeyValuePair<bool?, string>[] _isMaleOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public KeyValuePair<bool?, string>[] IsMaleOptions
        {
            get
            {
                return _isMaleOptions ?? (_isMaleOptions = CreateBoolPairs("Male","Female"));
            }
        }
        KeyValuePair<bool?, string>[] _likelyDie24HrOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public KeyValuePair<bool?, string>[] LikelyDie24HrOptions
        {
            get
            {
                return _likelyDie24HrOptions ?? (_likelyDie24HrOptions = CreateBoolPairs(Strings.NewPatient_IsLikelyDie24, Strings.NewPatient_NotLikelyDie24));
            }
        }
        KeyValuePair<bool?, string>[] _refusedConsentOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public KeyValuePair<bool?, string>[] RefusedConsentOptions
        {
            get
            {
                return _refusedConsentOptions ?? (_refusedConsentOptions = CreateBoolPairs(Strings.NewPatient_ConsentRefused, Strings.NewPatient_ConsentObtained));
            }
        }
        KeyValuePair<bool?, string>[] _wasGivenBcgPriorOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public KeyValuePair<bool?, string>[] WasGivenBcgPriorOptions
        {
            get
            {
                return _wasGivenBcgPriorOptions ?? (_wasGivenBcgPriorOptions = CreateBoolPairs(Strings.NewPatient_BCGgiven, Strings.NewPatient_BCGnotGiven));
            }
        }

        IEnumerable<KeyValuePair<StudyCentreModel, string>> _studyCentreOptions;
        public IEnumerable<KeyValuePair<StudyCentreModel, string>> StudyCentreOptions
        {
            get
            {
                if (_studyCentreOptions==null)
                {
                    var studyCentres = _repository.LocalStudyCentres;
                    var returnVar = new List<KeyValuePair<StudyCentreModel, string>>(
                        _repository.LocalStudyCentres
                        .Select(s => new KeyValuePair<StudyCentreModel, string>(s, s.Name)));
                    if (studyCentres.Skip(1).Any())
                    {
                        returnVar.Insert(0,
                            new KeyValuePair<StudyCentreModel, string>(null, Strings.DropDownList_PleaseSelect));
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
		         return _newPatient.Id == 0;
            }
        }
        public bool CanRandomise(object parameter)
        {
            return IsNewRecord && WasValidOnLastNotify && _newPatient.OkToRandomise();
        }
        const string CancelString = "userCancelled";
        string ValidateEnvelopeSoftErrors()
        {
            if (!IsValid())
            {
                throw new InvalidOperationException("Can only validate soft errors after hard errors have been removed");
            }
            if (!IsEnvelopeRandomising)
            {
                return null;
            }
            string envelopeSoftError = ((IDataErrorInfo)_newPatient)["EnvelopeNumber"];
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
            if (!IsValid() || !_newPatient.OkToRandomise())
            {
                throw new InvalidOperationException("Underlying NewPatientModel does not validate");
            }
#endif
            string violationString = null;
            if (IsEnvelopeRandomising) 
            {
                violationString = ValidateEnvelopeSoftErrors();
                if (violationString == CancelString)
                {
                    return;
                }
            }
            Participant newParticipant = new Participant
            {
                Name = _newPatient.Name,
                MothersName = _newPatient.MothersName,
                HospitalIdentifier = _newPatient.HospitalIdentifier.Trim(),
                AdmissionWeight = _newPatient.AdmissionWeight.Value,
                GestAgeBirth = _newPatient.GestAgeBirth,
                DateTimeBirth = _newPatient.DateTimeBirth.Value,
                Abnormalities =_newPatient.Abnormalities,
                PhoneNumber = _newPatient.PhoneNumber,
                IsMale = _newPatient.IsMale.Value,
                RegisteredAt = _newPatient.DateTimeOfEnrollment ?? DateTime.Now,
                RegisteringInvestigator = GetCurrentPrincipal().Identity.Name,
                CentreId = StudyCentre.Id
            };
            if (MultipleSibling != null && MultipleSibling.IsMale == newParticipant.IsMale)
            {
                newParticipant.MultipleSiblingId = _newPatient.MultipleSiblingId.Value;
                if (IsEnvelopeRandomising)
                {
                    newParticipant.Id = (from p in _repository.Participants
                                         where p.Id > EnvelopeDetails.MaxEnvelopeNumber
                                         orderby p.Id descending
                                         select p.Id).FirstOrDefault();
                    if (newParticipant.Id == 0)
                    {
                        newParticipant.Id = EnvelopeDetails.MaxEnvelopeNumber;
                    }
                    newParticipant.Id += 1;
                    newParticipant.BlockNumber = EnvelopeDetails.FirstAvailableBlockNumber;
                    newParticipant.IsInterventionArm = MultipleSibling.IsInterventionArm;
                }
                else
                {
                    RandomisingEngine.ForcePairedAllocation(newParticipant, MultipleSiblingId.Value, _repository);
                }
            }
            else if (IsEnvelopeRandomising)
            {
                Envelope envelope = EnvelopeDetails.GetEnvelope(EnvelopeNumber.Value);
                newParticipant.BlockNumber = envelope.BlockNumber;
                newParticipant.BlockSize = envelope.BlockSize;
                newParticipant.IsInterventionArm = envelope.IsInterventionArm;
                newParticipant.Id = EnvelopeNumber.Value;
            }
            else
            {
                RandomisingEngine.CreateAllocation(newParticipant,_repository);
            }
            _repository.Add(newParticipant);
            if (violationString !=null)
            {
                var violation = new ProtocolViolation
                {
                    MajorViolation = true,
                    Details = violationString,
                    ParticipantId = newParticipant.Id,
                };
                _repository.AddOrUpdate(violation);
            }
            if (!false.Equals(parameter)) // for testing purposes, supress dialog
            {
                string userMsg = (newParticipant.IsInterventionArm) 
                    ? Strings.NewPatient_ToIntervention + " " + InterventionArmMessage
                    : Strings.NewPatient_ToControl + " " + ControlArmMessage;
                userMsg = string.Format(userMsg, newParticipant.Name + '(' + newParticipant.HospitalIdentifier + ')', newParticipant.Id);
                MessageBox.Show(userMsg, Strings.NewPatient_SuccesfullyRandomised, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            ClearAllFields();
        }
        public RelayCommand AddScreenCmd { get; private set; }
        public bool CanScreen(object parameter)
        {
            var okToScreen = _newPatient.OkToScreen();
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
                HospitalIdentifier = _newPatient.HospitalIdentifier.Trim(),
                AdmissionWeight = _newPatient.AdmissionWeight.Value,
                GestAgeBirth = _newPatient.GestAgeBirth,
                DateTimeBirth = _newPatient.DateTimeBirth.Value,
                Abnormalities = _newPatient.Abnormalities,
                IsMale = _newPatient.IsMale.Value,
                RegisteredAt = DateTime.Now,
                RegisteringInvestigator = GetCurrentPrincipal().Identity.Name,
                CentreId = StudyCentre.Id,
                BadInfectnImmune = _newPatient.BadInfectnImmune.Value,
                BadMalform = _newPatient.BadMalform.Value,
                LikelyDie24Hr = _newPatient.LikelyDie24Hr.Value,
                RefusedConsent = _newPatient.RefusedConsent,
                Missed = _newPatient.Missed,
                WasGivenBcgPrior = _newPatient.WasGivenBcgPrior.Value
            };
            if (GetValidationError("HospitalIdentifier",true)!=null)
            {
                log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType).Error(
                    string.Format("duplicate value got through on Id:{0}, okToScreen:{1} , wasValid:{2}, isValid:{3}",
                    screenedPt.HospitalIdentifier,
                    _newPatient.OkToScreen(), WasValidOnLastNotify, IsValid()));
            }
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
            if (_abnormalities!= null) { _abnormalities.Clear(); }
            _newPatient = new NewPatientModel();
            StudyCentre = StudyCentreOptions.First().Key;
            _multipleSibling = null;
            _hasSiblingEnrolled = false;
            _wtForAgeCentile = null;
            NotifyPropertyChanged("Name", "HospitalIdentifier", "AdmissionWeight", "GestAgeDays", "GestAgeWeeks", "IsMale", "DateOfBirth", "TimeOfBirth", "DateOfEnrollment", "TimeOfEnrollment", "LikelyDie24Hr", "BadMalform", "BadInfectnImmune", "WasGivenBcgPrior", "RefusedConsent", "MothersName", "WtForAgeCentile", "PhoneNumber", "IsYoungerThanMinEnrolTime", "EnvelopeNumber", "OkToRandomise", "IsConsentRequired", "HasSiblingEnrolled", "MultipleSiblingId");
            RecordAltered = false;
            _isEnrollmentDateTimeAssigned = false;
        }
        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
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
            bool returnVal = _newPatient.IsValid() && !ValidatedProperties.Any(v => this.GetValidationError(v, true) != null);
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
            string error = _newPatient.GetValidationError(propertyName, hardErrorsOnly);

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
            if (IsNewRecord && EnvelopeNumber.HasValue)
            {
                if(_repository.Participants.Any(p=>p.Id == EnvelopeNumber.Value))
                {
                    return Strings.NewPatientViewModel_Error_EnvelopeInDb;
                }
            }
            return null;
        }
        string ValidateNewIdentifier()
        {
            if (IsNewRecord)
            {
                string currentHospId = _newPatient.HospitalIdentifier.Trim();
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
            if (_hasSiblingEnrolled && !_newPatient.MultipleSiblingId.HasValue)
            {
                return Strings.NewPatientVM_Error_SiblingIdEmpty;
            }
            if (_newPatient.MultipleSiblingId.HasValue)
            {
                if (MultipleSibling == null)
                {
                    return Strings.NewPatientVM_Error_SiblingNotFound;
                }
                if (_newPatient.DateTimeBirth.HasValue &&
                    Math.Abs((MultipleSibling.DateTimeBirth - _newPatient.DateTimeBirth.Value).TotalHours) > twinSeperationMaxHrs)
                {
                    return string.Format(Strings.NewPatient_Error_TwinSeparation, twinSeperationMaxHrs);
                }
                if (_newPatient.GestAgeBirth != MultipleSibling.GestAgeBirth)
                {
                    return Strings.NewPatient_Error_TwinGestAge;
                }
                if (MultipleSibling.IsMale != _newPatient.IsMale)
                {
                    SiblingNote = Strings.NewPatient_Error_TwinGenderDifferent;
                }
            }
            return null;
        }
        #endregion // IDataErrorInfo Members
    }
    public class Abnormality : NotifyChangeBase 
    {
        #region Description
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value) { return; }
                _description = value;
                NotifyPropertyChanged("Description");
            }
        }

        #endregion // DisplayName
    }
}
