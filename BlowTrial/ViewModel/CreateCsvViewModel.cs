using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using AutoMapper;
using BlowTrial.Domain.Tables;
using System.Linq;
using BlowTrial.Infrastructure.CSV;
using System.IO;
using BlowTrial.Infrastructure;
using Microsoft.Win32;
using BlowTrial.Properties;

namespace BlowTrial.ViewModel
{

    public sealed class CreateCsvViewModel: WorkspaceViewModel, IDataErrorInfo
    {
        #region Fields
        CreateCsvModel _model;
        #endregion

        #region Constructor
        public CreateCsvViewModel(IRepository repository, CreateCsvModel csvModel)
            : base(repository)
        {
            _model = csvModel;
            SaveCmd = new RelayCommand(Save, param=>IsValid());
            SelectFileCmd = new RelayCommand(SelectFile);
            CancelCmd = new RelayCommand(param => CloseCmd.Execute(param), param => true);
            DisplayName = Strings.CreateCsvVM_Title;
            SelectedFileType = FileTypes.First();
        }
        #endregion

        #region Properties
        public string Filename 
        { 
            get
            {
                return _model.Filename;
            }
            set
            {
                if (_model.Filename == value) { return; }
                _model.Filename = value;
                NotifyPropertyChanged("Filename");
            }
        }

        public string DateFormat
        {
            get
            {
                return _model.DateFormat;
            }
            set
            {
                if (_model.DateFormat == value) { return; }
                _model.DateFormat = value;
                NotifyPropertyChanged("DateFormat", "DateExample");
            }
        }

        readonly DateTime ExampleDate = new DateTime(2014,1,30,14,12,56);
        public string DateExample
        {
            get
            {
                try
                {
                    return ExampleDate.ToString(DateFormat);
                }
                catch(FormatException)
                {
                    return Strings.NotApplicable;
                }
            }
        }

        public DelimitedFileType SelectedFileType
        {
            get
            {
                return _model.SelectedFileType;
            }
            set
            {
                if (_model.SelectedFileType == value) { return; }
                _model.SelectedFileType = value;
                NotifyPropertyChanged("SelectedFileType");
            }
        }

        public TableOptions TableType
        { 
            get
            {
                return _model.TableType;
            }
            set
            {
                if (_model.TableType == value) { return; }
                _model.TableType = value;
                NotifyPropertyChanged("TableType");
            }
        }

        public IEnumerable<DelimitedFileType> FileTypes
        {
            get { return CreateCsvModel.FileTypes; }
        }

        #endregion

        #region Commands
        public RelayCommand SaveCmd { get; private set; }
        public void Save(object param)
        {
            if (!IsValid())
            {
                throw new InvalidOperationException("CreateCsvViewModel not valid - cannot call save");
            }
            switch (TableType)
            {
                case TableOptions.Participant:
                    var participants = Mapper.Map<ParticipantCsvModel[]>(_repository.Participants.Include("VaccinesAdministered").ToArray());
                    var csvEncodedParticipants = PatientDataToCSV.ParticipantDataToCSV(participants, _repository.Vaccines.ToArray(), SelectedFileType.Delimiter, DateFormat);
                    try
                    {
                        File.WriteAllLines(_model.FileNameWithExtension, csvEncodedParticipants);
                    }
                    catch(System.IO.IOException e)
                    {
                        System.Windows.MessageBox.Show(e.Message);
                        return;
                    }
                    break;
                case TableOptions.ScreenedPatients:
                    var screened = Mapper.Map<ScreenedPatientCsvModel[]>(_repository.ScreenedPatients.ToArray());
                    var csvEncodedScreened = CSVconversion.IListToStrings<ScreenedPatientCsvModel>(screened, SelectedFileType.Delimiter, DateFormat);
                    File.WriteAllLines(_model.FileNameWithExtension, csvEncodedScreened);
                    break;
                case TableOptions.ProtocolViolations:
                    var viol = _repository.ProtocolViolations.ToArray();
                    var csvEncodedViols = CSVconversion.IListToStrings<ProtocolViolation>(viol, SelectedFileType.Delimiter, DateFormat);
                    File.WriteAllLines(_model.FileNameWithExtension, csvEncodedViols);
                    break;
                default:
                    throw new InvalidOperationException("save called no table selected");
            }
            CloseCmd.Execute(null);
        }
        public RelayCommand SelectFileCmd { get; private set; }
        public void SelectFile(object param)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "(*.csv)|*.csv",
                CheckFileExists = false,
                AddExtension = false,
                ValidateNames = true,
                DefaultExt = ".csv", 
            };
            if (!string.IsNullOrEmpty(Filename)) { dialog.FileName = Filename; }
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                Filename = dialog.FileName;
            }
        }
        public RelayCommand CancelCmd { get; private set; }
        #endregion

        #region Listbox options
        IEnumerable<KeyValuePair<TableOptions, string>> _tableTypeOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public IEnumerable<KeyValuePair<TableOptions, string>> TableTypeOptions
        {
            get
            {
                if (_tableTypeOptions == null)
                {
                    _tableTypeOptions = EnumToListOptions<TableOptions>();
                }
                return _tableTypeOptions;
            }
        }

        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                string returnVal = ((IDataErrorInfo)_model)[propertyName];
                CommandManager.InvalidateRequerySuggested();
                return returnVal;
            }
        }

        #endregion // IDataErrorInfo Members

        #region Validation
        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public bool IsValid()

            {
                return _model.IsValid();
            
        }



        #endregion // IDataErrorInfo Members
    }
}
