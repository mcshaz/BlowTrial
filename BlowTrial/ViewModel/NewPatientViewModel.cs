using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure;
using BlowTrial.Infrastructure.Centiles;
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

namespace BlowTrial.ViewModel
{
    class NewPatientViewModel:WorkspaceViewModel, IDataErrorInfo
    {        
        #region Fields

        private NewPatientModel _newPatient;

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

            Mediator.Register("NewDayElapsed", UpdateDateNotified);

        }
        #endregion

        #region Properties
        public string Name
        {
	        get
	        {
		        return _newPatient.Name;
	        }
	        set
	        {
		        if (value == _newPatient.Name) { return; }
		        _newPatient.Name=value;
                NotifyPropertyChanged("Name", "DisplayName", "OkToRandomise");
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
		        _newPatient.HospitalIdentifier=value;
                NotifyPropertyChanged("HospitalIdentifier", "DisplayName", "OkToRandomise");
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
		        _newPatient.AdmissionWeight=value;
                NotifyPropertyChanged("AdmissionWeight", "OkToRandomise");
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
		        _newPatient.GestAgeWeeks=value;
                NotifyPropertyChanged("GestAgeWeeks", "GestAgeDays", "OkToRandomise");
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
		        _newPatient.GestAgeDays=value;
                NotifyPropertyChanged("GestAgeWeeks", "GestAgeDays");
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
                                         where !string.IsNullOrWhiteSpace(a.DisplayName)
                                         select a.DisplayName).ToSeparatedList(';');
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
		        _newPatient.IsMale=value;
                NotifyPropertyChanged("IsMale", "OkToRandomise");
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
                NotifyPropertyChanged("DateOfBirth", "OkToRandomise", "TimeOfBirth");
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
                NotifyPropertyChanged("TimeOfBirth", "IsYoungerThanMinEnrollTime");
            }
        }
        public bool IsYoungerThanMinEnrollTime
        {
            get
            {
                if (_newPatient.DateTimeBirth==null) {return false;}
                var ageMins = (DateTime.Now - _newPatient.DateTimeBirth.Value).TotalMinutes;
                return ageMins >= 0 && ageMins < NewPatientModel.MinEnrollAgeMins;
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
		        _newPatient.LikelyDie24Hr=value;
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
		        _newPatient.BadMalform=value;
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
		        _newPatient.BadInfectnImmune=value;
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
		        _newPatient.WasGivenBcgPrior=value;
		        NotifyPropertyChanged("WasGivenBcgPrior", "IsConsentRequired");
	        }
        }
        public bool IsConsentRequired
        {
            get
            {
                NotifyPropertyChanged("OkToRandomise");
                return (LikelyDie24Hr==false &&
                    BadMalform==false && 
                    BadInfectnImmune==false &&
                    WasGivenBcgPrior==false);
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
		        _newPatient.RefusedConsent=value;
                NotifyPropertyChanged("RefusedConsent", "OkToRandomise");
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
                NotifyPropertyChanged("MultipleSiblingId");
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
                return CanRandomise(null);
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
                return _multipleSibling ?? (_multipleSibling = _repository.Participants.Find(MultipleSiblingId));
            }
        }
        private int CentreId
        {
            get { return int.Parse(System.Configuration.ConfigurationManager.AppSettings["CentreId"]); }
        }
        public DateTime Today
        {
            get { return DateTime.Today; }
        }
        public DateTime MinScreeningDate
        {
            get { return DateTime.Today.AddDays(-NewPatientModel.MaxAgeDaysScreen); }
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
                WtForAgeCentile = string.Format(Strings.NewPatientVM_Centile ,
                    WeightData.CumSnormBirthWtForGestAge(AdmissionWeight.Value, GestAgeWeeks.Value, GestAgeDays ?? 0, IsMale.Value));
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
            return IsNewRecord && IsValid && _newPatient.OkToRandomise();
        }
        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        /// 
        public void Randomise(object parameter)
        {
            if (!IsValid || !_newPatient.OkToRandomise())
            {
                throw new InvalidOperationException("Underlying NewPatientModel does not validate");
            }
            Participant newParticipant = new Participant
            {
                Name = _newPatient.Name,
                MothersName = _newPatient.MothersName,
                HospitalIdentifier = _newPatient.HospitalIdentifier,
                AdmissionWeight = _newPatient.AdmissionWeight.Value,
                GestAgeBirth = _newPatient.GestAgeBirth,
                DateTimeBirth = _newPatient.DateTimeBirth.Value,
                Abnormalities =_newPatient.Abnormalities,
                PhoneNumber = _newPatient.PhoneNumber,
                IsMale = _newPatient.IsMale.Value,
                RegisteredAt = DateTime.Now,
                RegisteringInvestigator = GetCurrentPrincipal().Identity.Name,
                CentreId = CentreId
            };
            if (MultipleSibling != null && MultipleSibling.IsMale == newParticipant.IsMale)
            {
                RandomisingEngine.ForcePairedAllocation(newParticipant, MultipleSiblingId.Value,_repository);
            }
            else
            {
                RandomisingEngine.CreateAllocation(newParticipant,_repository);
            }
            _repository.Add(newParticipant);
            if (!false.Equals(parameter)) // for testing purposes, supress dialog
            {
                string userMsg = (newParticipant.IsInterventionArm) ? Strings.NewPatient_ToIntervention : Strings.NewPatient_ToControl;
                userMsg = string.Format(userMsg, _newPatient.Name + '(' + _newPatient.HospitalIdentifier + ')');
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
            return okToScreen && IsValid;
        }
        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        /// 
        public void AddScreen(object parameter)
        {
            if (!IsValid || !_newPatient.OkToScreen())
            {
                throw new InvalidOperationException("Underlying NewPatientModel does not validate");
            }
            var screenedPt = new ScreenedPatient
            {
                Name = _newPatient.Name,
                MothersName = _newPatient.MothersName,
                HospitalIdentifier = _newPatient.HospitalIdentifier,
                AdmissionWeight = _newPatient.AdmissionWeight.Value,
                GestAgeBirth = _newPatient.GestAgeBirth,
                DateTimeBirth = _newPatient.DateTimeBirth.Value,
                Abnormalities = _newPatient.Abnormalities,
                PhoneNumber = _newPatient.PhoneNumber,
                IsMale = _newPatient.IsMale.Value,
                RegisteredAt = DateTime.Now,
                RegisteringInvestigator = GetCurrentPrincipal().Identity.Name,
                CentreId = CentreId,
                BadInfectnImmune = _newPatient.BadInfectnImmune.Value,
                BadMalform = _newPatient.BadMalform.Value,
                LikelyDie24Hr = _newPatient.LikelyDie24Hr.Value,
                RefusedConsent = _newPatient.RefusedConsent.Value,
                Missed = _newPatient.Missed.Value,
                WasGivenBcgPrior = _newPatient.WasGivenBcgPrior.Value
            };
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
            _wtForAgeCentile = null;
            NotifyPropertyChanged("Name", "HospitalIdentifier", "AdmissionWeight", "GestAgeDays", "GestAgeWeeks", "IsMale", "DateOfBirth", "TimeOfBirth", "LikelyDie24Hr", "BadMalform", "BadInfectnImmune", "WasGivenBcgPrior", "RefusedConsent", "MothersName", "WtForAgeCentile", "PhoneNumber", "IsYoungerThanMinEnrollTime");
        }
        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get 
            { 
                string error = this.GetValidationError(propertyName);
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }

        #endregion // IDataErrorInfo Members

        #region Validation

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool returnVal = _newPatient.IsValid && !ValidatedProperties.Any(v => this.GetValidationError(v) != null);
                CommandManager.InvalidateRequerySuggested();
                return returnVal;
            }
        }

        readonly string[] ValidatedProperties = 
        { 
            "HospitalIdentifier",
            "MultipleSiblingId"
        };
        protected string GetValidationError(string propertyName)
        {
            string error = ((IDataErrorInfo)_newPatient)[propertyName];

            if (error==null && ValidatedProperties.Contains(propertyName))
            {
                switch (propertyName)
                {
                    case "HospitalIdentifier":
                        error = this.ValidateNewIdentifier();
                        break;
                    case "MultipleSiblingId":
                        error = this.ValidateSiblingId();
                        break;
                    default:
                        Debug.Fail("Unexpected property being validated on NewPatient: " + propertyName);
                        break;
                }
            }
            return error;
        }

        string ValidateNewIdentifier()
        {
            if (IsNewRecord)
            {
                if (_repository.Participants.Any(p=>p.HospitalIdentifier == _newPatient.HospitalIdentifier))
                {
                    return Strings.NewPatientVM_Error_DuplicateEnrol;
                }
                if(_repository.ScreenedPatients.Any(s=>s.HospitalIdentifier==_newPatient.HospitalIdentifier))
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
                if (MultipleSibling.IsMale != _newPatient.IsMale)
                {
                    SiblingNote = Strings.NewPatient_Error_TwinGenderDifferent;
                }
            }
            return null;
        }
        #endregion // IDataErrorInfo Members

        #region Destructor
        ~NewPatientViewModel()
        {
            Mediator.Unregister("NewDayElapsed", UpdateDateNotified);
        }
        #endregion
    }
    public class Abnormality : ViewModelBase 
    {
        public string Description
        {
            get { return DisplayName; }
            set { DisplayName = value; }
        }
    }
}
