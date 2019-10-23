using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using BlowTrial.Properties;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using BlowTrial.Infrastructure.Extensions;
using BlowTrial.Domain.Tables;

namespace BlowTrial.ViewModel
{
    public sealed class StudySitesViewModel : WizardPageViewModel, IDataErrorInfo
    {
        #region fields
        readonly StudySitesModel _appModel;
        readonly IRepository _repo;
        #endregion

        #region constructors
        public StudySitesViewModel(StudySitesModel model, IRepository repo) : this(model)
        {
            SaveCmd = new RelayCommand(ExecuteSave, p => IsValid());
            _repo = repo;
        }
        public StudySitesViewModel(StudySitesModel model)
        {
            _appModel = model;
            StudySitesData = new ObservableCollection<StudySiteItemViewModel>();
            foreach (StudySiteItemModel s in _appModel.StudySitesData)
            {
                s.AllLocalSites = _appModel;
                StudySitesData.Add(new StudySiteItemViewModel(s) { CanAlterId = false });
            }
            var newItem = NewSiteDataVM();
            newItem.AllowEmptyRecord = _appModel.StudySitesData.Any();
            StudySitesData.Add(newItem);
            StudySitesData.CollectionChanged += StudySitesData_CollectionChanged;

            DisplayName = Strings.StudySitesViewModel_DisplayName;

                
        }
#endregion
        #region Methods
        public static BlowTrial.Domain.Tables.StudyCentre MapToStudySite(StudySiteItemModel s)
        {
            return new BlowTrial.Domain.Tables.StudyCentre
            {
                ArgbBackgroundColour = s.SiteBackgroundColour.Value.ToInt(),
                ArgbTextColour = s.SiteTextColour.ToInt(),
                IsCurrentlyEnrolling = s.IsCurrentlyEnrolling,
                IsOpvInIntervention = s.IsOpvInIntervention,
                IsToHospitalDischarge = s.IsToHospitalDischarge,
                Id = s.Id.Value,
                DefaultAllocation = s.DefaultAllocation,
                HospitalIdentifierMask = s.HospitalIdentifierMask,
                MaxIdForSite = s.Id.Value + s.MaxParticipantAllocations.Value - (s.Id == 1 ? 2 : 1),
                Name = s.SiteName,
                PhoneMask = s.PhoneMask,
                DuplicateIdCheck = (s.DuplicateIdCheck == Guid.Empty) ? Guid.NewGuid() : s.DuplicateIdCheck
            };
        }
        void ExecuteSave(object param)
        {
            _repo.AddOrUpdate(_appModel.StudySitesData.Select(s=> MapToStudySite(s)));
            OnRequestClose();
        }
        void StudySitesData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (StudySiteItemViewModel i in e.OldItems)
                {
                    i.PropertyChanged -= NewSiteDataVm_PropertyChanged;
                    _appModel.StudySitesData.Remove(i.SiteModel);
                }
            }
        }
        StudySiteItemViewModel NewSiteDataVM()
        {
            var newSite = new StudySiteItemModel
                {
                    AllLocalSites = _appModel
                };
            var newVm = new StudySiteItemViewModel(newSite);
            newVm.PropertyChanged += NewSiteDataVm_PropertyChanged;
            return newVm;
        }

        void NewSiteDataVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var newStudySiteVm = (StudySiteItemViewModel)sender;
            if (!newStudySiteVm.AllPropertiesNull())
            {
                if (newStudySiteVm.IsValid())
                {
                    newStudySiteVm.PropertyChanged -= NewSiteDataVm_PropertyChanged;
                    var newVM = NewSiteDataVM();
                    newVM.AllowEmptyRecord = true;
                    StudySitesData.Add(newVM);
                }
                else if (!_appModel.StudySitesData.Contains(newStudySiteVm.SiteModel))
                {
                    _appModel.StudySitesData.Add(newStudySiteVm.SiteModel);
                }
                NotifyPropertyChanged("StudySitesData");
            }
        }
        #endregion

        #region properties
        public ObservableCollection<StudySiteItemViewModel> StudySitesData { get; private set; }

        public override bool IsValid()
        {
            return _appModel.IsValid();
        }

        public RelayCommand SaveCmd { get; private set; }
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

    }

    public sealed class StudySiteItemViewModel : ViewModelBase, IDataErrorInfo
    {
        public StudySiteItemViewModel(StudySiteItemModel siteModel)
        {
            SiteModel = siteModel;
            CanAlterId = true;
        }
        public bool CanAlterId { get; set; }
        public StudySiteItemModel SiteModel {get; private set;}
        public bool AllowEmptyRecord { get; set; }
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
                NotifyPropertyChanged("SiteName", "SiteBackgroundColour", "SiteTextColour");
            }
        }
        public AllocationGroups DefaultAllocation
        {
            get
            {
                return SiteModel.DefaultAllocation;
            }
            set
            {
                if (SiteModel.DefaultAllocation == value) { return; }
                SiteModel.DefaultAllocation = value;
                NotifyPropertyChanged("DefaultAllocation");
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
                NotifyPropertyChanged("SiteBackgroundColour", "SiteBackgroundBrush", "SiteName", "SiteTextColour");
            }
        }
        public Color SiteTextColour
        {
            get
            {
                return SiteModel.SiteTextColour;
            }
            set
            {
                if (SiteModel.SiteTextColour == value) { return; }
                SiteModel.SiteTextColour = value;
                NotifyPropertyChanged("SiteTextColour", "SiteTextBrush", "SiteName", "SiteBackgroundColour");
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
        public int? Id
        {
            get { return SiteModel.Id; }
            set
            {
                if (SiteModel.Id == value) { return; }
                SiteModel.Id = value;
                NotifyPropertyChanged("Id");
            }
        }
        public bool IsCurrentlyEnrolling
        {
            get
            {
                return SiteModel.IsCurrentlyEnrolling;
            }
            set
            {
                if (SiteModel.IsCurrentlyEnrolling == value) { return; }
                SiteModel.IsCurrentlyEnrolling = value;
                NotifyPropertyChanged("IsCurrentlyEnrolling");
            }
        }
        public bool IsOpvInIntervention
        {
            get
            {
                return SiteModel.IsOpvInIntervention;
            }
            set
            {
                if (SiteModel.IsOpvInIntervention == value) { return; }
                SiteModel.IsOpvInIntervention = value;
                NotifyPropertyChanged("IsOpvInIntervention");
            }
        }
        public bool IsToHospitalDischarge
        {
            get
            {
                return SiteModel.IsToHospitalDischarge;
            }
            set
            {
                if (SiteModel.IsToHospitalDischarge == value) { return; }
                SiteModel.IsToHospitalDischarge = value;
                NotifyPropertyChanged("IsToHospitalDischarge");
            }
        }
        public string HospitalIdentifierMask
        {
            get { return SiteModel.HospitalIdentifierMask; }
            set
            {
                if (SiteModel.HospitalIdentifierMask == value) { return; }
                SiteModel.HospitalIdentifierMask = value;
                NotifyPropertyChanged("HospitalIdentifierMask");
            }
        }
        public string PhoneMask
        {
            get { return SiteModel.PhoneMask; }
            set
            {
                if (SiteModel.PhoneMask == value) { return; }
                SiteModel.PhoneMask = value;
                NotifyPropertyChanged("PhoneMask");
            }
        }
        public int? MaxParticipantAllocations
        {
            get { return SiteModel.MaxParticipantAllocations; }
            set
            {
                if (SiteModel.MaxParticipantAllocations == value) { return; }
                SiteModel.MaxParticipantAllocations = value;
                NotifyPropertyChanged("MaxParticipantAllocations");
            }
        }
        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                if (AllowEmptyRecord && AllPropertiesNull())
                {
                    return null;
                }
                string error = this.GetValidationError(propertyName);
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }
        string GetValidationError(string propertyName)
        {
            return ((IDataErrorInfo)SiteModel)[propertyName];
        }
        public bool IsValid()
        {
            return SiteModel.IsValid();
        }

        #region ListBoxOptions
        static IEnumerable<KeyDisplayNamePair<AllocationGroups>> _allocationTypeOptions;
        public IEnumerable<KeyDisplayNamePair<AllocationGroups>> AllocationTypeOptions
        {
            get
            {
                return _allocationTypeOptions ?? (_allocationTypeOptions = EnumToListOptions<AllocationGroups>(default(AllocationGroups)));
            }
        }
        #endregion // Listbox options

        internal bool AllPropertiesNull()
        {
            return string.IsNullOrEmpty(SiteName) 
                && SiteModel.SiteTextColour==StudySiteItemModel.DefaultTextColor 
                && SiteModel.SiteBackgroundColour==null
                && SiteModel.Id==null
                && SiteModel.MaxParticipantAllocations == null
                && SiteModel.PhoneMask == StudySiteItemModel.DefaultPhoneMask
                && SiteModel.HospitalIdentifierMask == StudySiteItemModel.DefaultHospitalIdentifierMask;
        }
    }
}
