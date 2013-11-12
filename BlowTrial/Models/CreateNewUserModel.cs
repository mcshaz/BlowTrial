using BlowTrial.Infrastructure;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Models
{
    public class CreateNewUserModel : ValidationBase
    {
        public CreateNewUserModel()
        {
            _validatedProperties = new string[] { "UserName", "EncryptedPassword", "ConfirmEncryptedPassword" };
        }
        public string UserName { get; set; }
        public string EncryptedPassword { get; set; }
        public int UnencryptedPasswordLength { get; set; }
        public string ConfirmEncryptedPassword { get; set; }

        public override string GetValidationError(string propertyName)
        {
            if (!_validatedProperties.Contains(propertyName)) { return null; }
            string error = null;
            switch (propertyName)
            {
                case "UserName":
                    error = ValidateFieldLength(UserName, 4, 50);
                    break;
                case "EncryptedPassword":
                    error = ValidateUnencryptedPasswordLength();
                    break;
                case "ConfirmEncryptedPassword":
                    error = ValidateConfirmPassword();
                    break;
            }
            return error;
        }
        const int MinPasswordLength = 4;
        const int MaxPasswordLength = 40;
        string ValidateUnencryptedPasswordLength()
        {
            if (UnencryptedPasswordLength==0)
            {
                return Strings.Field_Error_Empty;
            }
            if (UnencryptedPasswordLength < MinPasswordLength)
            {
                return string.Format(Strings.Field_Error_TooShort, MinPasswordLength);
            }
            if (UnencryptedPasswordLength > MaxPasswordLength)
            {
                return string.Format(Strings.Field_Error_TooShort, MaxPasswordLength);
            }
            return null;
        }
        string ValidateConfirmPassword()
        {
            string error = ValidateFieldNotEmpty(ConfirmEncryptedPassword);
            if (error==null && ConfirmEncryptedPassword != EncryptedPassword)
            {
                error = Strings.CreateNewUserModel_Error_PasswordsDontMatch;
            }
            return error;
        }
    }
}
