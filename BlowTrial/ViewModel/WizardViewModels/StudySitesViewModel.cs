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
    public sealed class StudySitesViewModel : ViewModelBase, IDataErrorInfo
    {
        #region fields
        StudySitesModel _appModel;
        ObservableCollection<StudySiteItemViewModel> _studySitesData;
        #endregion

        #region constructors
        public StudySitesViewModel()
        {
            _appModel = new StudySitesModel();
            _studySitesData = new ObservableCollection<StudySiteItemViewModel>(
                _appModel.StudySitesData.Select(s => new StudySiteItemViewModel(s)));
            _studySitesData.CollectionChanged += StudySitesData_CollectionChanged;
            _studySitesData.Add(NewSiteDataVM());
            StudySitesData = new ListCollectionView(_studySitesData);
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
        void StudySitesData_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (StudySiteItemViewModel s in e.OldItems)
                {
                    _appModel.StudySitesData.Remove(s.SiteModel);
                    s.PropertyChanged -= NewSiteDataVm_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (StudySiteItemViewModel s in e.NewItems)
                {
                    _appModel.StudySitesData.Add(s.SiteModel);
                }
                NotifyPropertyChanged("StudySitesData");
            }
        }

        void NewSiteDataVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var newStudySiteVm = (StudySiteItemViewModel)sender;
            if (newStudySiteVm.SiteName != null && newStudySiteVm.SiteModel.IsValid)
            {
                StudySitesData.CommitNew();
                newStudySiteVm.PropertyChanged -= NewSiteDataVm_PropertyChanged;
                StudySitesData.AddNewItem(NewSiteDataVM());
            }
        }
        #endregion

        #region properties
        public ListCollectionView StudySitesData { get; private set; }

        public bool IsValid
        {
            get
            {
                return _appModel.IsValid;
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

    }

    public sealed class StudySiteItemViewModel : NotifyChangeBase, IDataErrorInfo
    {
        public StudySiteItemViewModel(StudySiteItemModel siteModel)
        {
            SiteModel = siteModel;
        }
        public StudySiteItemModel SiteModel {get; private set;}
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
