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
using System.Collections.ObjectModel;
using System.Windows.Data;

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
            SetupDirectoryItems();

        }
        void SetupDirectoryItems()
        {
            CloudDirectories = new ObservableCollection<DirectoryItemViewModel>();

            foreach (var c in _cloudDirModel.CloudDirectoryItems)
            {
                var vm = new DirectoryItemViewModel(c);
                CloudDirectories.Add(vm);
            }

            CloudDirectories.CollectionChanged += CloudDirectories_CollectionChanged;

            AddDirectoryItemVM();
        }

        void CloudDirectories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (DirectoryItemViewModel i in e.OldItems)
                {
                    i.PropertyChanged -= DirectoryItem_NewPropertyChanged;
                    i.PropertyChanged -= DirectoryItem_PropertyChanged;
                    _cloudDirModel.CloudDirectoryItems.Remove(i.DirectoryItem);
                }
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (DirectoryItemViewModel i in e.NewItems)
                {
                    i.PropertyChanged += DirectoryItem_PropertyChanged;
                }
            }
        }

        void AddDirectoryItemVM()
        {
            if (!CloudDirectories.Any() || _cloudDirModel.IsBackingUpToCloud == false)
            {
                var newDir = new DirectoryItemViewModel(new DirectoryItemModel());
                if (CloudDirectories.Any()) { newDir.AllowBlank = true; }
                newDir.PropertyChanged += DirectoryItem_NewPropertyChanged;
                CloudDirectories.Add(newDir);
            }

        }
        void DirectoryItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DirectoryPath")
            {
                NotifyPropertyChanged("CloudDirectories");
            }
        }
        void DirectoryItem_NewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DirectoryPath")
            {
                var item = (DirectoryItemViewModel)sender;
                if (item.IsValid())
                {
                    item.PropertyChanged -= DirectoryItem_NewPropertyChanged;
                    item.AllowBlank = false;
                    _cloudDirModel.CloudDirectoryItems.Add(item.DirectoryItem);
                    AddDirectoryItemVM();
                }
                
            }
        }

        #endregion

        #region Properties
        public ObservableCollection<DirectoryItemViewModel> CloudDirectories
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
            BlowTrialDataService.SetBackupDetails(
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
        
        public DirectoryItemViewModel(DirectoryItemModel directoryItem)
        {
            DirectoryItem = directoryItem;
            SelectDirectoryCmd = new RelayCommand(SelectDirectory);
            AllowBlank = false;
        }

        public bool AllowBlank { get; set; }

        public string DirectoryPath
        {
            get
            {
                return DirectoryItem.DirectoryPath;
            }
            set
            {
                if (value == DirectoryItem.DirectoryPath) { return; }
                DirectoryItem.DirectoryPath = value;
                NotifyPropertyChanged("DirectoryPath");
            }
        }
        public DirectoryItemModel DirectoryItem { get; private set; }
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
                if (AllowBlank && string.IsNullOrEmpty(DirectoryPath)) { return null; }
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
            bool returnVal = DirectoryItem.IsValid();
            CommandManager.InvalidateRequerySuggested();
            return returnVal;
        }


        private string GetValidationError(string propertyName)
        {
            string error = ((IDataErrorInfo)DirectoryItem)[propertyName];

            // Dirty the commands registered with CommandManager,
            // such as our Save command, so that they are queried
            // to see if they can execute now.
            return error;
        }

        #endregion // IDataErrorInfo Members
    }
}
