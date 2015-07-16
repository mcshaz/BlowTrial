using BlowTrial.Domain.Tables;
using BlowTrial.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace BlowTrial.ViewModel
{
    public sealed class VaccineAdministeredViewModel : NotifyChangeBase, IDataErrorInfo
    {
        #region Constructors
        public VaccineAdministeredViewModel(VaccineAdministeredModel vaccineModel, IEnumerable<VaccineViewModel> vaccineList)
        {
            VaccineList = vaccineList;
            VaccineAdministeredModel = vaccineModel;
            SelectedVaccine = VaccineList.First(l => l.VaccineId == vaccineModel.VaccineId);
        }
        #endregion
        #region Fields
        VaccineViewModel _selectedVaccine;
        
        #endregion

        #region Properties
        public VaccineAdministeredModel VaccineAdministeredModel { get; private set; }
        public IEnumerable<VaccineViewModel> VaccineList { get; private set; }

        public int Id { get { return VaccineAdministeredModel.Id; } }
        
        public VaccineViewModel SelectedVaccine
        {
            get { return _selectedVaccine ?? VaccineList.First(l=>l.Vaccine==null); }
            set 
            { 
                if (_selectedVaccine==value) {return;}
                if (_selectedVaccine !=null)
                {
                    _selectedVaccine.IsGivenToThisPatient = false;
                }
                if (value != null)
                {
                    value.IsGivenToThisPatient = true;
                }
                _selectedVaccine = value;
                this.VaccineAdministeredModel.VaccineGiven = _selectedVaccine.Vaccine;
                NotifyPropertyChanged("SelectedVaccine", "AdministeredAtDate", "AdministeredAtTime","EarliestOutcomeDate");
            }
        }

        public Vaccine VaccineGiven { get { return this.VaccineAdministeredModel.VaccineGiven; } }

        internal DateTime? AdministeredAtDateTime
        {
            get
            {
                return VaccineAdministeredModel.AdministeredAtDateTime;
            }
            set
            {
                if (value == this.VaccineAdministeredModel.AdministeredAtDateTime) { return; }
                this.VaccineAdministeredModel.AdministeredAtDateTime = value;
                NotifyPropertyChanged("AdministeredAtDate", "AdministeredAtTime");
            }
        }
        public DateTime? AdministeredAtDate 
        { 
            get
            {
                return this.VaccineAdministeredModel.AdministeredAtDate;
            }
            set
            {
                if (value == this.VaccineAdministeredModel.AdministeredAtDate) { return; }
                this.VaccineAdministeredModel.AdministeredAtDate = value;
                NotifyPropertyChanged("AdministeredAtDate", "AdministeredAtTime", "SelectedVaccine");
            }
        }
        public TimeSpan? AdministeredAtTime
        {
            get
            {
                return this.VaccineAdministeredModel.AdministeredAtTime;
            }
            set
            {
                if (value == this.VaccineAdministeredModel.AdministeredAtTime) { return; }
                this.VaccineAdministeredModel.AdministeredAtTime = value;
                NotifyPropertyChanged("AdministeredAtDate", "AdministeredAtTime", "SelectedVaccine");
            }
        }
        public bool IsBcg
        {
            get
            {
                return VaccineAdministeredModel.VaccineGiven.IsBcg;
            }
        }

        public DateTime EarliestOutcomeDate
        {
            get
            {
                if (SelectedVaccine.Vaccine == null || !IsBcg)
                {
                    return VaccineAdministeredModel.AdministeredTo.DateTimeBirth;
                }
                return VaccineAdministeredModel.AdministeredTo.RegisteredAt;
            }
        }

        public bool AllowEmptyRecord { get; set; }
        #endregion

        #region Methods
        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                if (AllowEmptyRecord && IsEmpty)
                {
                    return null;
                }
                if (propertyName == "SelectedVaccine") { propertyName = "VaccineGiven"; }
                string error = ((IDataErrorInfo)VaccineAdministeredModel)[propertyName];
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return SelectedVaccine.Vaccine == null && AdministeredAtDate == null && AdministeredAtTime == null;
            }
        }


        public bool IsValid()
        {
            if (AllowEmptyRecord && IsEmpty) { return true; }
            return this.VaccineAdministeredModel.IsValid();
        }

        #endregion // IDataErrorInfo Members

        #region Destructor

        #endregion
    }
}
