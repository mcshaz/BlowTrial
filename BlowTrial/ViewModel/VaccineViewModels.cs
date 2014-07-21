using BlowTrial.Domain.Tables;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BlowTrial.ViewModel
{
    public class VaccineViewModel : ViewModelBase
    {
        public VaccineViewModel(Vaccine vaccine)
        {
            this.Vaccine = vaccine;
            if (vaccine != null) { VaccineId = vaccine.Id; }
        }
        public int VaccineId { get; private set; }
        public Vaccine Vaccine { get; private set;}
        bool _isGivenToThisPatient;
        public bool IsGivenToThisPatient
        {
            get { return _isGivenToThisPatient; }
            set
            {
                if (_isGivenToThisPatient == value || Vaccine==null) { return; }
                _isGivenToThisPatient = value;
                base.NotifyPropertyChanged("IsGivenToThisPatient");
            }
        }
        public override string DisplayName
        {
            get 
            {
                if (Vaccine == null) { return Strings.DropDownList_PleaseSelect; }
                return this.Vaccine.Name;
            }
        }
    }
}
