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
    public sealed class StudySitesViewModel : WizardPageViewModel, IDataErrorInfo
    {
        #region fields
        StudySitesModel _appModel;

        #endregion

        #region constructors
        public StudySitesViewModel(StudySitesModel model)
        {
            _appModel = model;
            StudySitesData = new ObservableCollection<StudySiteItemViewModel>(
                _appModel.StudySitesData.Select(s => new StudySiteItemViewModel(s)));
            StudySitesData.Add(NewSiteDataVM());
            StudySitesData.CollectionChanged += StudySitesData_CollectionChanged;
            
            DisplayName = Strings.StudySitesViewModel_DisplayName;
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
                    Id = Guid.NewGuid(),
                    AllLocalSites = _appModel
                };
            var newVm = new StudySiteItemViewModel(newSite);
            newVm.PropertyChanged += NewSiteDataVm_PropertyChanged;
            return newVm;
        }

        void NewSiteDataVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var newStudySiteVm = (StudySiteItemViewModel)sender;
            if (newStudySiteVm.SiteName != null && newStudySiteVm.SiteModel.IsValid())
            {
                newStudySiteVm.PropertyChanged -= NewSiteDataVm_PropertyChanged;
                _appModel.StudySitesData.Add(newStudySiteVm.SiteModel);
                var newVM = NewSiteDataVM();
                StudySitesData.Add(newVM);
                newVM.AllowBlanks = true;
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

    public sealed class StudySiteItemViewModel : NotifyChangeBase, IDataErrorInfo
    {
        public StudySiteItemViewModel(StudySiteItemModel siteModel)
        {
            SiteModel = siteModel;
        }
        public StudySiteItemModel SiteModel {get; private set;}
        public bool AllowBlanks { get; set; }
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
                return SiteModel.SiteTextColour ?? Color.FromArgb(255, 0, 0, 0);
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

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                if (AllowBlanks && string.IsNullOrEmpty(SiteName) && SiteModel.SiteTextColour==null && SiteModel.SiteBackgroundColour==null)
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
    }
}
