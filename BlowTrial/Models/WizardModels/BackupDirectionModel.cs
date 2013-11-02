using BlowTrial.Infrastructure;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace BlowTrial.Models
{
    public class BackupDirectionModel :ValidationBase
    {
        public BackupDirectionModel()
        {
            _validatedProperties = new string[] { "PatientsPreviouslyRandomised", "BackupToCloud" };

        }
        public bool? BackupToCloud { get; set; }
        public bool PatientsPreviouslyRandomised { get; set; }


        public override string GetValidationError(string propertyName)
        {
            if (!_validatedProperties.Contains(propertyName))
            { return null; }

            string error = null;

            switch (propertyName)
            {
                case "PatientsPreviouslyRandomised":
                    error = ValidatePatientsPreviouslyRandomised();
                    break;
                case "BackupToCloud":
                    error = ValidateDDLNotNull(BackupToCloud);
                    break;
                default:
                    Debug.Fail("Unexpected property being validated on GetAppSettingsModel: " + propertyName);
                    break;
            }

            return error;
        }
        string ValidatePatientsPreviouslyRandomised()
        {
            if (PatientsPreviouslyRandomised && BackupToCloud!=true)
            {
                return Strings.StudySiteDataModel_Error_PatientsPreviouslyRandomisedNotRelevant;
            }
            return null;
        }
    }
}
