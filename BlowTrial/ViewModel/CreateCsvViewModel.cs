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
using BlowTrial.Helpers;
using BlowTrial.TextTemplates;
using GenericToDataString;

namespace BlowTrial.ViewModel
{

    public sealed class CreateCsvViewModel: WorkspaceViewModel, IDataErrorInfo
    {
        #region Fields
        CreateCsvModel _model;
        bool _canAlterDateInQuotes;
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
            DateFormat = "u";
            IsDateInQuotes = true;
            IsStringInQuotes = true;
            //SetDateInQuotes();
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
                SetDateInQuotes();
                NotifyPropertyChanged("DateFormat", "DateExample");
            }
        }

        public string DateExample
        {
            get
            {
                return _model.ExampleDate ?? Strings.NotApplicable;
            }
        }

        public bool CanAlterDateInQuotes
        {
            get
            {
                return _canAlterDateInQuotes;
            }
            private set 
            {
                if (_canAlterDateInQuotes == value) { return; }
                _canAlterDateInQuotes = value;
                NotifyPropertyChanged("CanAlterDateInQuotes");
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
                SetDateInQuotes();
                NotifyPropertyChanged("SelectedFileType");
            }
        }

        public bool IsStringInQuotes
        {
            get
            {
                return _model.IsStringInQuotes;
            }
            set
            {
                if (_model.IsStringInQuotes == value) { return; }
                _model.IsStringInQuotes = value;
                NotifyPropertyChanged("IsStringInQuotes");
            }
        }

        public bool IsDateInQuotes
        {
            get
            {
                return _model.IsDateInQuotes;
            }
            set
            {
                if (_model.IsDateInQuotes == value) { return; }
                _model.IsDateInQuotes = value;
                NotifyPropertyChanged("IsDateInQuotes");
            }
        }

        public bool SimultaneousStata
        {
            get
            {
                return _model.SimultaneousStata;
            }
            set
            {
                if (_model.SimultaneousStata == value) { return; }
                _model.SimultaneousStata = value;
                NotifyPropertyChanged("SimultaneousStata");
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

        #region methods
        void SetDateInQuotes()
        {
            if (_model.ExampleDate.Contains(',') && SelectedFileType.Delimiter == ',')
            {
                IsDateInQuotes = true;
                CanAlterDateInQuotes = false;
            }
            else
            {
                CanAlterDateInQuotes = true;
            }
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
            string dofile = null;
            switch (TableType)
            {
                case TableOptions.Participant:
                    var vaccines = _repository.Vaccines.ToList();
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(_model.Filename))
                        {
                            PatientDataToCSV.ParticipantDataToCSV(_repository.Participants.Include("VaccinesAdministered").ToList(), vaccines, SelectedFileType.Delimiter, DateFormat, IsStringInQuotes, IsDateInQuotes, sw);
                        }
                    }
                    catch(System.IO.IOException e)
                    {
                        System.Windows.MessageBox.Show(e.Message);
                        return;
                    }
                    if (SimultaneousStata)
                    {
                        dofile = new ParticipantDataStataTemplate(
                            new ParticipantStataData( _model.FileNameWithExtension,
                                SelectedFileType.Delimiter,
                                _repository.LocalStudyCentres.Select(s => new KeyValuePair<int, string>(s.Id, s.Name)),
                                vaccines.Select(v=>v.Name)
                            )).TransformText();
                    }
                    break;
                case TableOptions.ScreenedPatients:
                    var screened = Mapper.Map<ScreenedPatientCsvModel[]>(_repository.ScreenedPatients.ToArray());
                    var csvEncodedScreened = ListConverters.ToCSV(screened, SelectedFileType.Delimiter, PatientDataToCSV.CSVOptions(DateFormat, IsStringInQuotes, IsDateInQuotes));
                    try
                    {
                        File.WriteAllText(_model.FileNameWithExtension, csvEncodedScreened);
                    }
                    catch (System.IO.IOException e)
                    {
                        System.Windows.MessageBox.Show(e.Message);
                        return;
                    }
                    if (SimultaneousStata)
                    {
                        dofile = new ScreenedPatientStataTemplate(new CentreStataData(
                            _model.FileNameWithExtension,
                            SelectedFileType.Delimiter,
                            _repository.LocalStudyCentres.Select(s => new KeyValuePair<int, string>(s.Id, s.Name))
                            )).TransformText();
                    }
                    break;
                case TableOptions.ProtocolViolations:
                    var viol = _repository.ProtocolViolations.ToArray();
                    var csvEncodedViols = ListConverters.ToCSV(viol, SelectedFileType.Delimiter, PatientDataToCSV.CSVOptions(DateFormat, IsStringInQuotes, IsDateInQuotes));
                    try
                    {
                        File.WriteAllText(_model.FileNameWithExtension, csvEncodedViols);
                    }
                    catch(System.IO.IOException e)
                    {
                        System.Windows.MessageBox.Show(e.Message);
                        return;
                    }
                    if (SimultaneousStata)
                    {
                        dofile = new ScreenedPatientStataTemplate(new CentreStataData(
                            _model.FileNameWithExtension,
                            SelectedFileType.Delimiter,
                            _repository.LocalStudyCentres.Select(s => new KeyValuePair<int, string>(s.Id, s.Name))
                            )).TransformText();
                    }
                    break;
                default:
                    throw new InvalidOperationException("save called no table selected");
            }
            if (dofile != null)
            {
                try
                {
                    File.WriteAllText(Path.ChangeExtension(_model.Filename, "do"), dofile);
                }
                catch (System.IO.IOException e)
                {
                    System.Windows.MessageBox.Show(e.Message);
                    return;
                }
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
        IEnumerable<KeyDisplayNamePair<TableOptions>> _tableTypeOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public IEnumerable<KeyDisplayNamePair<TableOptions>> TableTypeOptions
        {
            get
            {
                if (_tableTypeOptions == null)
                {
                    _tableTypeOptions = EnumToListOptions<TableOptions>(null);
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
