using BlowTrial.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BlowTrial.Models
{
    public class RandomisedMessagesModel : ValidationBase
    {
        #region Constructors
        public RandomisedMessagesModel()
        {
            _validatedProperties = new string[] { "InterventionInstructions", "ControlInstructions" };
        }
        #endregion

        #region fields
        #endregion

        #region Properties
        public string InterventionInstructions { get; set; }
        public string ControlInstructions { get; set; }
        #endregion

        #region Validation Overrides/implementation
        public override string GetValidationError(string propertyName)
        {
            if (!_validatedProperties.Contains(propertyName))
            { return null; }

            string error = null;

            switch (propertyName)
            {
                case "InterventionInstructions":
                    error = ValidateFieldLength(InterventionInstructions, 18, 200);
                    break;
                case "ControlInstructions":
                    error = ValidateFieldLength(InterventionInstructions, 18, 200);
                    break;
                default:
                    Debug.Fail("Unexpected property being validated on RandomisedMessagesModel: " + propertyName);
                    break;
            }

            return error;
        }
        #endregion

    }
}
