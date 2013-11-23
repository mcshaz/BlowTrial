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
using System.Windows.Media;
using BlowTrial.Helpers;

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
        public bool? MajorViolation
        {
            get
            {
                return _violation.MajorViolation;
            }
            set
            {
                if (value == _violation.MajorViolation) { return; }
                _isRecordAltered = true;
                _violation.MajorViolation = value;
                NotifyPropertyChanged("MajorViolation");
            }
        }

        public string SeverityDescription
        {
            get
            {
                switch(_violation.MajorViolation)
                {
                    case true:
                        return Strings.ProtocolViolationVM_Major;
                    case false:
                        return Strings.ProtocolViolationVM_Minor;
                    default:
                        return null;
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
                return _violation.Participant.TrialArm;
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
        #endregion

        #region Listbox Options
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
        #endregion // Listbox options

        #region Commands
        public RelayCommand SaveCmd{get; private set;}

        void Save(object param)
        {
            ProtocolViolation violation = new ProtocolViolation
                {
                    MajorViolation = _violation.MajorViolation.Value,
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
                    default: //OK in Proceed ? OK/Cancel and No In Close without saving? yes/no/cancel
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
