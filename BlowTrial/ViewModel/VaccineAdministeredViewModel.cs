using BlowTrial.Domain.Tables;
using BlowTrial.Models;
using BlowTrial.Properties;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BlowTrial.ViewModel
{
    public sealed class VaccineAdministeredViewModel : NotifyChangeBase, IDataErrorInfo
    {
        #region Constructors
        public VaccineAdministeredViewModel(VaccineAdministeredModel vaccineModel, IEnumerable<VaccineViewModel> vaccineList)
        {
            VaccineList = vaccineList;
            this.VaccineAdministeredModel = vaccineModel;
            SelectedVaccine = (vaccineModel.VaccineGiven==null)?vaccineList.First(l => l.Vaccine == null)
                : VaccineList.First(l => l.Vaccine!=null && l.Vaccine.Id == vaccineModel.VaccineGiven.Id);
        }
        #endregion
        #region Fields
        VaccineViewModel _selectedVaccine;
        public VaccineAdministeredModel VaccineAdministeredModel { get; private set; }
        #endregion

        #region Properties
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
                NotifyPropertyChanged("SelectedVaccine", "AdministeredAtDate", "AdministeredAtTime","EarliestDate");
            }
        }

        public Vaccine VaccineGiven { get { return this.VaccineAdministeredModel.VaccineGiven; } }

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

        public DateTime EarliestDate
        {
            get
            {
                if (SelectedVaccine.Vaccine == null || !SelectedVaccine.Vaccine.IsBcg)
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
