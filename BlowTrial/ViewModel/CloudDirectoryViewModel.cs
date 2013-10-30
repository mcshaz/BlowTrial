using BlowTrial.Helpers;
using BlowTrial.Models;
using BlowTrial.Properties;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

namespace BlowTrial.ViewModel
{
    public sealed class CloudDirectoryViewModel: WorkspaceViewModel, IDataErrorInfo
    {
        #region Fields
        CloudDirectoryModel _cloudDirModel;
        #endregion

        #region Constructor
        public CloudDirectoryViewModel(CloudDirectoryModel cloudDirModel)
        {
            _cloudDirModel = cloudDirModel;
            HasBackupDirectionBeenSet = cloudDirModel.BackupToCloud.HasValue;
            SaveCmd = new RelayCommand(Save, param=>IsValid);
            SelectDirectoryCmd = new RelayCommand(SelectDirectory);
            CancelCmd = new RelayCommand(param => CloseCmd.Execute(param), param => HasBackupDirectionBeenSet);
            IntervalTimeScale = 1;
        }
        #endregion

        #region Properties
        public string CloudDirectory 
        { 
            get
            {
                return _cloudDirModel.CloudDirectory;
            }
            set
            {
                if (_cloudDirModel.CloudDirectory == value) { return; }
                _cloudDirModel.CloudDirectory = value;
                NotifyPropertyChanged("CloudDirectory");
            }
        }
        public bool HasBackupDirectionBeenSet { get; private set; }
        private int _intervalTimeScale;
        public int IntervalTimeScale
        {
            get
            {
                return _intervalTimeScale;
            }
            set
            {
                if (_intervalTimeScale == value) { return; }
                _intervalTimeScale = value;
                NotifyPropertyChanged("IntervalTimeScale", "BackupInterval");
            }
        }
        public int? BackupInterval
        {
            get
            {
                return _cloudDirModel.BackupIntervalMinutes/IntervalTimeScale;
            }
            set
            {
                if (value == BackupInterval) { return; }
                _cloudDirModel.BackupIntervalMinutes = value * IntervalTimeScale;
                NotifyPropertyChanged("BackupInterval");
            }
        }
        public bool? BackupToCloud
        {
            get
            {
                return _cloudDirModel.BackupToCloud;
            }
            set
            {
                if (_cloudDirModel.BackupToCloud == value) { return; }
                _cloudDirModel.BackupToCloud = value;
                NotifyPropertyChanged("BackupToCloud");
            }
        }

        #endregion

        #region Commands
        public RelayCommand SaveCmd { get; private set; }
        public void Save(object param)
        {
            if (!IsValid)
            {
                throw new InvalidOperationException("CloudDirectoryViewModel not valid - cannot call save");
            }
            AppDataService.SetBackupDetails(_cloudDirModel.CloudDirectory, 
                _cloudDirModel.BackupIntervalMinutes.Value,
                _cloudDirModel.BackupToCloud.Value);
            CloseCmd.Execute(null);
        }
        public RelayCommand SelectDirectoryCmd { get; private set; }
        public void SelectDirectory(object param)
        {
            var dialog = new FolderBrowserDialog
                { 
                    Description = Strings.CloudDirectoryVm_SelectDir,
                };
            if (!string.IsNullOrEmpty(CloudDirectory)) { dialog.SelectedPath = CloudDirectory; }
            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.Cancel)
            {
                CloudDirectory = dialog.SelectedPath;
            }
        }
        public RelayCommand CancelCmd { get; private set; }
        #endregion

        #region Listbox options
        KeyValuePair<int, string>[] _intervalTimeScaleOptions;
        /// <summary>
        /// Returns a list of strings used to populate a drop down list for a bool? property.
        /// </summary>
        public KeyValuePair<int, string>[] IntervalTimeScaleOptions
        {
            get
            {
                if (_intervalTimeScaleOptions==null)
                {
                    _intervalTimeScaleOptions = new KeyValuePair<int, string>[]
                    {
                        new KeyValuePair<int,string>(1,"Minutes"),
                        new KeyValuePair<int, string>(60,"Hours")
                    };
                }
                return _intervalTimeScaleOptions;
            }
        }
        KeyValuePair<bool?, string>[] _backupToCloudOptions;
        public KeyValuePair<bool?, string>[] BackupToCloudOptions
        {
            get
            {
                return _backupToCloudOptions ?? 
                    (_backupToCloudOptions = CreateBoolPairs(Strings.CloudDirectoryVm_DataCollectingSite, Strings.CloudDirectoryVm_DataReceivingSite));
            }
        }
        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                if (propertyName=="BackupInterval" || propertyName=="IntervalTimeScale")
                {
                    propertyName = "BackupIntervalMinutes";
                }
                string returnVal = this.GetValidationError(propertyName);
                CommandManager.InvalidateRequerySuggested();
                return returnVal;
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
                bool returnVal = _cloudDirModel.IsValid;
                CommandManager.InvalidateRequerySuggested();
                return returnVal;
            }
        }


        private string GetValidationError(string propertyName)
        {
            string error = (_cloudDirModel as IDataErrorInfo)[propertyName];

            // Dirty the commands registered with CommandManager,
            // such as our Save command, so that they are queried
            // to see if they can execute now.
            return error;
        }

        #endregion // IDataErrorInfo Members
    }
}
