using BlowTrial.Helpers;
using BlowTrial.Models;
using BlowTrial.Properties;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using System.Linq;

namespace BlowTrial.ViewModel
{
    public sealed class CloudDirectoryViewModel: WizardPageViewModel, IDataErrorInfo
    {
        #region Fields
        CloudDirectoryModel _cloudDirModel;
        #endregion

        #region Constructor
        public CloudDirectoryViewModel(CloudDirectoryModel cloudDirModel)
        {
            _cloudDirModel = cloudDirModel;
            SaveCmd = new RelayCommand(Save, param=>IsValid());
            CancelCmd = new RelayCommand(param => CloseCmd.Execute(param));
            IntervalTimeScale = 1;
            DisplayName = Strings.CloudDirectoryViewModel_DisplayName;
        }
        #endregion

        #region Properties
        public IList<DirectoryItemViewModel> CloudDirectories
        {
            get;
            private set;
        }
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

        #endregion

        #region Commands
        public RelayCommand SaveCmd { get; private set; }
        public void Save(object param)
        {
            BackupDataService.SetBackupDetails(
                _cloudDirModel.CloudDirectoryItems.Select(c=>c.DirectoryPath), 
                _cloudDirModel.BackupIntervalMinutes.Value);
            CloseCmd.Execute(null);
        }

        public ICommand CancelCmd { get; private set; }
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
        public override bool IsValid()
            {
                bool returnVal = _cloudDirModel.IsValid();
                CommandManager.InvalidateRequerySuggested();
                return returnVal;
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

    public sealed class DirectoryItemViewModel :NotifyChangeBase, IDataErrorInfo
    {
        DirectoryItemModel _directoryItem;
        public DirectoryItemViewModel(DirectoryItemModel directoryItem)
        {
            _directoryItem = directoryItem;
            SelectDirectoryCmd = new RelayCommand(SelectDirectory);
        }

        public string DirectoryPath
        {
            get
            {
                return _directoryItem.DirectoryPath;
            }
            set
            {
                if (value == _directoryItem.DirectoryPath) { return; }
                _directoryItem.DirectoryPath = value;
                NotifyPropertyChanged("DirectoryPath");
            }
        }

        public ICommand SelectDirectoryCmd { get; private set; }
        public void SelectDirectory(object param)
        {
            var dialog = new FolderBrowserDialog
            {
                Description = Strings.CloudDirectoryVm_SelectDir,
            };
            if (!string.IsNullOrEmpty(DirectoryPath)) { dialog.SelectedPath = DirectoryPath; }
            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.Cancel)
            {
                DirectoryPath = dialog.SelectedPath;
            }
        }
        #region IDataErrorInfo 
        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
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
        public bool IsValid()
        {
            bool returnVal = _directoryItem.IsValid();
            CommandManager.InvalidateRequerySuggested();
            return returnVal;
        }


        private string GetValidationError(string propertyName)
        {
            string error = ((IDataErrorInfo)_directoryItem)[propertyName];

            // Dirty the commands registered with CommandManager,
            // such as our Save command, so that they are queried
            // to see if they can execute now.
            return error;
        }

        #endregion // IDataErrorInfo Members
    }
}
