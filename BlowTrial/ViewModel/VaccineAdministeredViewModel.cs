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
        public VaccineAdministeredViewModel(VaccineAdministeredModel vaccineModel, ObservableCollection<VaccineViewModel> vaccineList)
        {
            VaccineList = vaccineList;
            this._vaccineAdministeredModel = vaccineModel;
            SelectedVaccine = vaccineList.FirstOrDefault(l => l.Vaccine == vaccineModel.VaccineGiven);
        }
        #endregion
        #region Fields
        VaccineViewModel _selectedVaccine;
        public VaccineAdministeredModel _vaccineAdministeredModel;
        #endregion

        #region Properties
        public ObservableCollection<VaccineViewModel> VaccineList { get; private set; }

        public int Id { get { return _vaccineAdministeredModel.Id; } }
        
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
                this._vaccineAdministeredModel.VaccineGiven = _selectedVaccine.Vaccine;
                NotifyPropertyChanged("SelectedVaccine", "AdministeredAt", "EarliestDate");
            }
        }

        public Vaccine VaccineGiven { get { return this._vaccineAdministeredModel.VaccineGiven; } }

        public DateTime? AdministeredAt 
        { 
            get
            {
                return this._vaccineAdministeredModel.AdministeredAt;
            }
            set
            {
                if (value == this._vaccineAdministeredModel.AdministeredAt) { return; }
                this._vaccineAdministeredModel.AdministeredAt = value;
                NotifyPropertyChanged("AdministeredAt", "SelectedVaccine");
            }
        }

        public DateTime EarliestDate
        {
            get
            {
                if (SelectedVaccine.Vaccine == null || SelectedVaccine.Vaccine.Name != Vaccine.BcgName())
                {
                    return _vaccineAdministeredModel.AdministeredTo.DateTimeBirth;
                }
                return _vaccineAdministeredModel.AdministeredTo.RegisteredAt;
            }
        }

        public bool IsValid
        {
            get
            {
                return this._vaccineAdministeredModel.IsValid;
            }
        }
        #endregion

        #region Methods
        #endregion

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error { get { return null; } }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                if (propertyName == "SelectedVaccine") { propertyName = "VaccineGiven"; }
                string error = ((IDataErrorInfo)_vaccineAdministeredModel)[propertyName];
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }

        #endregion // IDataErrorInfo Members

        #region Destructor

        #endregion
    }
}
