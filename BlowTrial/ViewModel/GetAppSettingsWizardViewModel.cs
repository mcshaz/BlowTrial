using BlowTrial.Models;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace BlowTrial.ViewModel
{
    public sealed class GetAppSettingsWizardViewModel : ViewModelBase, IDataErrorInfo
    {
        #region fields
        GetAppSettingsModel _appModel;
        ObservableCollection<StudySiteDataViewModel> _studySitesData;
        #endregion
        #region constructors
        public GetAppSettingsWizardViewModel()
        {
            CloudDirectoryVM = new CloudDirectoryViewModel(new CloudDirectoryModel());
            _appModel = new GetAppSettingsModel();
            _studySitesData = new ObservableCollection<StudySiteDataViewModel>(
                _appModel.StudySitesData.Select(s => new StudySiteDataViewModel(s)));
            _studySitesData.CollectionChanged += StudySitesData_CollectionChanged;
            StudySitesLV = new ListCollectionView(_studySitesData);
            AddSite();
        }
        void AddSite()
        {
            var newSite = new StudySiteDataModel
                {
                    Id = Guid.NewGuid(),
                    AppSetting = _appModel
                };
            var newVm = new StudySiteDataViewModel(newSite);
            newVm.PropertyChanged += NewSiteDataVm_PropertyChanged;
            _studySitesData.Add(newVm);
        }
        void StudySitesData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (StudySiteDataViewModel s in e.OldItems)
                {
                    _appModel.StudySitesData.Remove(s.SiteModel);
                    s.PropertyChanged -= NewSiteDataVm_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (StudySiteDataViewModel s in e.NewItems)
                {
                    _appModel.StudySitesData.Add(s.SiteModel);
                }
            }
        }

        void NewSiteDataVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
                var newStudySiteVm = (StudySiteDataViewModel)sender;
                if (newStudySiteVm.SiteName != null && newStudySiteVm.SiteModel.IsValid)
                {
                    StudySitesLV.CommitNew();
                    newStudySiteVm.PropertyChanged -= NewSiteDataVm_PropertyChanged;
                    AddSite();
                }
        }
        #endregion
        #region properties
        public ListCollectionView StudySitesLV { get; private set; }
        public CloudDirectoryViewModel CloudDirectoryVM {get; private set;}
        public bool PatientsPreviouslyRandomised
        {
            get
            {
                return _appModel.PatientsPreviouslyRandomised;
            }
            set
            {
                if (_appModel.PatientsPreviouslyRandomised == value) { return; }
                _appModel.PatientsPreviouslyRandomised = value;
                NotifyPropertyChanged("PatientsPreviouslyRandomised");
            }
        }
        public bool? BackupToCloud
        {
            get
            {
                return _appModel.BackupToCloud;
            }
            set
            {
                if (_appModel.BackupToCloud == value) { return; }
                _appModel.BackupToCloud = value;
                NotifyPropertyChanged("BackupToCloud", "IsBackupDirectionOk");
            }
        }
        public bool IsBackupDirectionOk
        {
            get
            {
                return GetValidationError("BackupToCloud") == null;
            }
        }
        public bool IsPreviouslyRandomisedOk
        {
            get
            {
                return GetValidationError("PatientsPreviouslyRandomised") == null;
            }
        }
        public bool IsValid
        {
            get
            {
                return _appModel.IsValid && CloudDirectoryVM.IsValid;
            }
        }
        #endregion

        #region IDataError implementation
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
        string GetValidationError(string propertyName)
        {
            return((IDataErrorInfo)_appModel)[propertyName];
        }
        #endregion

        #region ListBoxOptions
        KeyValuePair<bool?, string>[] _backupToCloudOptions;
        public KeyValuePair<bool?, string>[] BackupToCloudOptions
        {
            get
            {
                return _backupToCloudOptions ??
                    (_backupToCloudOptions = CreateBoolPairs(Strings.CloudDirectoryVm_DataCollectingSite, Strings.CloudDirectoryVm_DataReceivingSite));
            }
        }

        KeyValuePair<bool, string>[] _previouslyRandomisedOptions;
        public KeyValuePair<bool, string>[] PreviouslyRandomisedOptions
        {
            get
            {
                return _previouslyRandomisedOptions ??
                    (_previouslyRandomisedOptions = 
                        new KeyValuePair<bool,string>[] {
                            new KeyValuePair<bool,string>(true,Strings.PreviouslyRandomisedOptions_True),
                            new KeyValuePair<bool,string>(false,Strings.PreviouslyRandomisedOptions_False)
                        });
            }
        }
        #endregion
    }

    public sealed class StudySiteDataViewModel : NotifyChangeBase, IDataErrorInfo
    {
        public StudySiteDataViewModel(StudySiteDataModel siteModel)
        {
            SiteModel = siteModel;
        }
        public StudySiteDataModel SiteModel {get; private set;}
        public string SiteName
        {
            get
            {
                return SiteModel.SiteName;
            }
            set
            {
                if (SiteModel.SiteName == value) { return; }
                SiteModel.SiteName = value;
                NotifyPropertyChanged("SiteName");
            }
        }
        public Color SiteBackgroundColour 
        { 
            get 
            {
                return SiteModel.SiteBackgroundColour ?? Color.FromArgb(255, 255, 255, 255);
            }
            set
            {
                if (SiteModel.SiteBackgroundColour == value) { return; }
                SiteModel.SiteBackgroundColour = value;
                NotifyPropertyChanged("SiteBackgroundColour", "SiteBackgroundBrush");
            }
        }
        public Color SiteTextColour
        {
            get
            {
                return SiteModel.SiteTextColour ?? Color.FromArgb(255, 0, 0, 0);
            }
            set
            {
                if (SiteModel.SiteTextColour == value) { return; }
                SiteModel.SiteTextColour = value;
                NotifyPropertyChanged("SiteTextColour", "SiteTextBrush");
            }
        }
        public Brush SiteTextBrush
        {
            get
            {
                return new SolidColorBrush( SiteTextColour );
            }
        }
        public Brush SiteBackgroundBrush
        {
            get
            {
                return new SolidColorBrush( SiteBackgroundColour );
            }
        }

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
        string GetValidationError(string propertyName)
        {
            return ((IDataErrorInfo)SiteModel)[propertyName];
        }
    }
}
