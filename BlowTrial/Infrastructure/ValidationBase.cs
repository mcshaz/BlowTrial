using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure
{
    public abstract class ValidationBase : IDataErrorInfo
    {
        #region IDataErrorInfo Members

        public virtual string Error { get { return null; } }

        public virtual string this[string propertyName]
        {
            get { return this.GetValidationError(propertyName); }
        }

        #endregion // IDataErrorInfo Members

        #region Helper Methods
        public static string ValidateDDLNotNull<T>(Nullable<T> toValidate) where T : struct
        {
            if (toValidate==null)
            {
                return Strings.DropDownList_Error_BoolNull;
            }
            return null;
        }
        public static string ValidateFieldNotEmpty(string toValidate)
        {
            if (string.IsNullOrWhiteSpace(toValidate))
            {
                return Strings.Field_Error_Empty;
            }
            return null;
        }
        public static string ValidateFieldLength(string toValidate, int minLength, int maxLength)
        {
            Debug.Assert(minLength < maxLength);
            string error = ValidateFieldNotEmpty(toValidate);
            if (error==null)
            {
                if (toValidate.Length < minLength)
                {
                    error = string.Format(Strings.Field_Error_TooShort, minLength);
                }
                else if (toValidate.Length > maxLength)
                {
                    error = string.Format(Strings.Field_Error_TooLong, maxLength);
                }
            }
            return error;
        }
        public static string ValidateFieldNotEmpty<T>(Nullable<T> toValidate) where T : struct
        {
            if(toValidate==null)
            {
                return Strings.Field_Error_Empty;
            }
            return null;
        }
        #endregion

        #region Validation

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public bool IsValid()
        {
            foreach (string property in _validatedProperties)
            {
                if (GetValidationError(property) != null)
                { 
                    return false; 
                }
            }
            return true;
        }

        protected IEnumerable<string> _validatedProperties;

        public virtual string GetValidationError(string propertyName)
        {
            throw new NotImplementedException("this method must be overriden");
        }
        #endregion
    }
}
