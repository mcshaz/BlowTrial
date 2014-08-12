using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using BlowTrial.Properties;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using AutoMapper;

namespace BlowTrial.ViewModel
{
    public sealed class ProtocolViolationViewModel:CrudWorkspaceViewModel, IDataErrorInfo
    {        
        #region Fields

        ProtocolViolationModel _violation;
        bool _isRecordAltered;

        #endregion // Fields

        #region Constructor

        public ProtocolViolationViewModel(IRepository repository, ProtocolViolationModel violation) : base(repository)
        {
            
            if (violation == null)
            {
                throw new ArgumentNullException("violation");
            }
            _violation = violation;
            base.DisplayName = (IsNewRecord)?Strings.ProtocolViolationVM_Register:Strings.ProtocolViolationVM_Edit;
            SaveCmd = new RelayCommand(Save, param=>WasValidOnLastNotify && _isRecordAltered);
        }
        #endregion

        #region Properties
        public ViolationTypeOption ViolationType
        {
            get
            {
                return _violation.ViolationType;
            }
            set
            {
                if (value == _violation.ViolationType) { return; }
                _isRecordAltered = true;
                _violation.ViolationType = value;
                NotifyPropertyChanged("ViolationType");
            }
        }

        public string SeverityDescription
        {
            get
            {
                switch (ViolationType)
                {
                    case ViolationTypeOption.NotSelected:
                        return null;
                    case ViolationTypeOption.Minor:
                        return Strings.ProtocolViolationVM_Minor;
                    default:
                        return Strings.ProtocolViolationVM_Major;
                }               
            }
        }

        const int abbrevMaxLength = 25;
        public string AbbrevDetails
        {
            get
            {
                if (_violation.Details.Length <= abbrevMaxLength)
                {
                    return _violation.Details;
                }
                return _violation.Details.Substring(0, abbrevMaxLength - 3) + "...";
            }
        }

        public string Details
        {
            get
            {
                return _violation.Details;
            }
            set
            {
                if (value == _violation.Details) { return; }
                _isRecordAltered = true;
                _violation.Details = value;
                NotifyPropertyChanged("Details", "AbbrevDetails");
            }
        }

        public string Name
        {
	        get
	        {
		        return _violation.Participant.Name;
	        }
        }

        public string HospitalIdentifier
        {
	        get
	        {
		        return _violation.Participant.HospitalIdentifier;
	        }
        }

        public int ParticipantId
        {
            get
            {
                return _violation.Participant.Id;
            }
        }

        public string StudyCentreName
        {
            get
            {
                return _violation.Participant.StudyCentre.Name;
            }
        }

        public string Gender
        {
            get
            {
                return _violation.Participant.Gender;
            }
        }

        public string TrialArm
        {
            get
            {
                return _violation.Participant.TrialArmDescription;
            }
        }

        public bool IsNewRecord
        {
            get
            {
                return _violation.Id == 0;
            }
        }

        public DateTime ReportingTimeLocal
        {
            get
            {
                return _violation.ReportingTimeLocal;
            }
        }
        public Brush BackgroundColour
        {
            get
            {
                return _violation.Participant.StudyCentre.BackgroundColour;
            }
        }
        public Brush TextColour
        {
            get
            {
                return _violation.Participant.StudyCentre.TextColour;
            }
        }
        #endregion

        #region Listbox Options
        /* Legacy:
        KeyValuePair<bool?, string>[] _majorViolationOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public KeyValuePair<bool?, string>[] MajorViolationOptions
        {
            get
            {
                return _majorViolationOptions ?? (_majorViolationOptions = CreateBoolPairs(Strings.ProtocolViolationVM_Major,Strings.ProtocolViolationVM_Minor));
            }
        }
        //End legacy */

        IEnumerable<KeyDisplayNamePair<ViolationTypeOption>> _violationTypeOptions;
        public IEnumerable<KeyDisplayNamePair<ViolationTypeOption>> ViolationTypeOptions
        {
            get
            {
                return _violationTypeOptions ?? (_violationTypeOptions = EnumToListOptions<ViolationTypeOption>());
            }
        }
        #endregion // Listbox options
        #region Commands
        public RelayCommand SaveCmd{get; private set;}

        void Save(object param)
        {
            ProtocolViolation violation = new ProtocolViolation
                {
                    ViolationType = _violation.ViolationType,
                    Details = _violation.Details,
                    ParticipantId = _violation.Participant.Id
                };
            if (!IsNewRecord)
            {
                violation.Id = _violation.Id;
                violation.ReportingInvestigator = _violation.ReportingInvestigator;
                violation.ReportingTimeLocal = _violation.ReportingTimeLocal;
            }
            _repository.AddOrUpdate(violation);
            _isRecordAltered = false;
            OnRequestClose();
        }

        #endregion

        #region Window Event Handlers

        internal void OnClosingWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !OkToProceed();
        }

        #endregion

        #region Private Methods
        public bool OkToProceed()
        {
            if (_isRecordAltered)
            {
                string title;
                string msg;
                MessageBoxButton buttonOptions;
                if (IsValid())
                {
                    title = Strings.ParticipantUpdateVM_Confirm_SaveChanges_Title;
                    msg = Strings.ParticipantUpdateVM_Confirm_SaveChanges;
                    buttonOptions = MessageBoxButton.YesNoCancel;
                }
                else
                {
                    title = Strings.ParticipantUpdateVM_Confirm_Close_Title;
                    msg = Strings.ParticipantUpdateVM_Confirm_Close;
                    buttonOptions = MessageBoxButton.OKCancel;
                }
                MessageBoxResult result = MessageBox.Show(
                    msg,
                    title,
                    buttonOptions,
                    MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Save(null);
                        break;
                    case MessageBoxResult.Cancel:
                        return false;
                    case MessageBoxResult.No:
                        if (!IsNewRecord)
                        {
                            _violation = Mapper.Map<ProtocolViolationModel>(_repository.FindViolation(_violation.Id));
                            NotifyPropertyChanged("Details", "AbbrevDetails", "MajorViolation", "SeverityDescription");
                        }
                        break;
                    default: //OK in Proceed 
                        break;
                }
            }
            // Yes, OK or No
            return true;
        }
        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get 
            { 
                string error = ((IDataErrorInfo)_violation)[propertyName];
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
            return _violation.IsValid();
        }

        #endregion // IDataErrorInfo Members
    }
}
