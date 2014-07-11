using BlowTrial.Domain.Providers;
using BlowTrial.Domain.Tables;
using BlowTrial.Helpers;
using BlowTrial.Infrastructure;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BlowTrial.Models
{
    public class VaccineAdministeredModel : ValidationBase
    {
        #region Constructor
        public VaccineAdministeredModel()
        {
            _validatedProperties = new string[] { "AdministeredAtDate", "AdministeredAtTime","VaccineGiven" };
        }
        #endregion

        #region fields
        Vaccine _vaccine;
        DateTimeSplitter _administeredAtDateTime = new DateTimeSplitter();
        private readonly int[] BcgVaccines = new int[] { DataContextInitialiser.DanishBcg.Id, DataContextInitialiser.RussianBcg.Id };
        #endregion

        #region Properties
        public int Id { get; set; }

        public DateTime? AdministeredAtDateTime
        {
            get
            {
                return _administeredAtDateTime.DateAndTime;
            }
            set
            {
                _administeredAtDateTime.DateAndTime = value;
            }
        }
        public DateTime? AdministeredAtDate
        {
            get
            {
                return _administeredAtDateTime.Date;
            }
            set
            {
                _administeredAtDateTime.Date = value;
                //DischargeDateTime = _dischargeDateTime.DateAndTime;
            }
        }
        public TimeSpan? AdministeredAtTime
        {
            get
            {
                return _administeredAtDateTime.Time;
            }
            set
            {
                _administeredAtDateTime.Time = value;
                //DischargeDateTime = _dischargeDateTime.DateAndTime;
            }
        }
        public int VaccineId { get; set; }
        public Vaccine VaccineGiven 
        { 
            get
            {
                return _vaccine;
            }
            set
            {
                _vaccine = value;
            }
        }
        public ParticipantProgressModel AdministeredTo { get; set; }

        #endregion

        #region ValidationBase overrides
        public override bool IsValid()
        {
            DateTime now = DateTime.Now;
            return _validatedProperties.All(p=>GetValidationError(p,now)==null);
        }
        public override string GetValidationError(string propertyName)
        {
            return GetValidationError(propertyName, null);
        }
        public string GetValidationError(string propertyName, DateTime? now)
        {
            if (!_validatedProperties.Contains(propertyName))
            { return null; }
            switch (propertyName)
            {
                case "AdministeredAtDate":
                    return ValidateAdministrationDateTime(now).DateError;
                case "AdministeredAtTime":
                    return ValidateAdministrationDateTime(now).TimeError;
                case "VaccineGiven":
                    return ValidateVaccine();
                default:
                    Debug.Fail("Unexpected property being validated on NewPatient: " + propertyName);
                    break;
            }
            return null;
        }
        string ValidateVaccine()
        {
            if (this.VaccineGiven==null)
            {
                return Strings.VaccineAdministeredModel_Error_NoVaccine;
            }
            if (AdministeredTo.VaccineModelsAdministered.Any(v=>v.Id != this.Id && v.VaccineGiven==this.VaccineGiven))
            {
                return Strings.VaccineAdministeredVM_DuplicateVaccine;
            }
            if (BcgVaccines.Contains(this.VaccineGiven.Id) && AdministeredTo.VaccineModelsAdministered.Any(v => v.Id != this.Id && BcgVaccines.Contains(v.VaccineGiven.Id)))
            {
                return Strings.VaccineAdministeredVM_DualBcg;
            }
            return null;
        }
        DateTimeErrorString ValidateAdministrationDateTime(DateTime? now=null)
        {
            var error = _administeredAtDateTime.ValidateNotEmpty();
            if (VaccineGiven!=null && VaccineGiven.IsBcg)
            {
                _administeredAtDateTime.ValidateIsAfter(Strings.ParticipantModel_Error_RegistrationDateTime, AdministeredTo.RegisteredAt, ref error);
            }
            else
            {
                _administeredAtDateTime.ValidateIsAfter(Strings.DateOfBirth, AdministeredTo.DateTimeBirth, ref error);
            }
            DateTime nowVal = now ?? DateTime.Now;
            if (AdministeredTo.DeathOrLastContactDateTime.HasValue)
            {
                _administeredAtDateTime.ValidateIsBefore(
                    string.Format((AdministeredTo.IsKnownDead==true)? Strings.ParticipantUpdateView_Label_DeathDateTime: Strings.ParticipantUpdateView_Label_LastContactDateTime),
                    AdministeredTo.DeathOrLastContactDateTime.Value,
                    ref error);
            }
            else if (nowVal >= AdministeredTo.Becomes28On)
            {
                _administeredAtDateTime.ValidateIsBefore(
                    string.Format(Strings.TimeInterval_28days, AdministeredTo.Becomes28On),
                    AdministeredTo.Becomes28On,
                    ref error);
            }
            else
            {
                _administeredAtDateTime.ValidateIsBefore(
                    Strings.DateTime_Now,
                    nowVal,
                    ref error);
            }
            
            return error;
        }
        #endregion
    }
}
