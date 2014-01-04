using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Models
{
    public class VaccineAdministeredModel : ValidationBase
    {
        #region Constructor
        public VaccineAdministeredModel()
        {
            _validatedProperties = new string[] { "AdministeredAt", "Vaccine" };
        }
        #endregion

        #region fields
        DateTime? _administeredAt;
        Vaccine _vaccine;
        #endregion

        #region Properties
        public int Id { get; set; }
        public DateTime? AdministeredAt 
        { 
            get
            {
                return _administeredAt;
            }
            set
            {
                _administeredAt = value;
            }
        }
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
        public ParticipantModel AdministeredTo { get; set; }
        #endregion

        #region ValidationBase overrides
        public override string GetValidationError(string propertyName)
        {
            switch (propertyName)
            {
                case "AdministeredAt":
                    return ValidateAdministrationTime();
                case "VaccineGiven":
                    return ValidateVaccine();
            }
            return null;
        }
        string ValidateVaccine()
        {
            if (this.VaccineGiven==null)
            {
                return Strings.VaccineAdministeredModel_Error_NoVaccine;
            }
            if (AdministeredTo.VaccinesAdministered.Any(v=>v.Id != this.Id && v.VaccineGiven==this.VaccineGiven))
            {
                return Strings.VaccineAdministeredVM_DuplicateVaccine;
            }
            return null;
        }
        string ValidateAdministrationTime()
        {
            if (this.AdministeredAt == null)
            {
                return Strings.VaccineAdministeredModel_Error_NoAdminDate;
            }
            if (AdministeredAt > DateTime.Today)
            {
                return string.Format(Strings.DateTime_Error_Date_MustComeBefore, 
                    Strings.DateTime_Now);
            }
            if (AdministeredTo.DeathOrLastContactDate < AdministeredAt)
            {
                return string.Format(Strings.DateTime_Error_Date_MustComeBefore,
                    (AdministeredTo.IsKnownDead==true)
                    ?Strings.ParticipantUpdateView_Label_DeathDate
                    :Strings.ParticipantUpdateView_Label_LastContactDate);
            }
            if (AdministeredTo.DateTimeBirth.Date > AdministeredAt)
            {
                return string.Format(Strings.DateTime_Error_Date_MustComeAfter,
                    Strings.DateOfBirth);
            }
            else if (VaccineGiven!=null && VaccineGiven.IsBcg && AdministeredTo.RegisteredAt.Date > AdministeredAt)
            {
                return string.Format(Strings.DateTime_Error_Date_MustComeAfter,
                    Strings.ParticipantModel_Error_RegistrationDate);
            }
            return null;
        }
        #endregion
    }
}
