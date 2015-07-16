using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure;
using BlowTrial.Properties;
using System;
using System.Diagnostics;
using System.Linq;

namespace BlowTrial.Models
{
    public class UnsuccessfulFollowUpModel : ValidationBase
    {
        #region Constructor
        public UnsuccessfulFollowUpModel()
        {
            _validatedProperties = new string[] { "AttemptedContact" };
        }
        #endregion

        #region fields
        #endregion

        #region Properties
        public int Id { get; set; }
        public DateTime? AttemptedContact{get;set;}
        public int ParticipantId { get; set; }
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
                case "AttemptedContact":
                    return ValidateAttemptedContact(now);
                default:
                    Debug.Fail("Unexpected property being validated on NewPatient: " + propertyName);
                    break;
            }
            return null;
        }

        private string ValidateAttemptedContact(DateTime? now = null)
        {
            if (!AttemptedContact.HasValue)
            {
                return Strings.UnsuccessfulFollowUpModel_Error_AttemptedDateRequired;
            }
            /*
            if (AttemptedContact.Value > (now ?? DateTime.Now))
            {
                return string.Format(Strings.DateTime_Error_Date_MustComeBefore, Strings.DateTime_Today);
            };
            */
            return null;
        }

        #endregion
    }
}
