using BlowTrial.Infrastructure;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlowTrial.Models
{
    public class CloudDirectoryModel : ValidationBase
    {

        #region Constructors

        public CloudDirectoryModel()
        {
            _validatedProperties = new string[]
            { 
                "CloudDirectory",
                "BackupIntervalMinutes",
                "BackupToCloud"
            };
        }
        #endregion //Constructors

        #region Fields
        #endregion // Fields

        #region Properties
        public string CloudDirectory { get; set; }
        public int? BackupIntervalMinutes { get; set; }
        public bool? BackupToCloud { get; set; }
        #endregion

        #region Methods
        #endregion

        #region Validation

        public override string GetValidationError(string propertyName)
        {
            if (!_validatedProperties.Contains(propertyName))
            { return null; }

            string error = null;

            switch (propertyName)
            {
                case "CloudDirectory":
                    error = ValidateCloudDirectory();
                    break;
                case "BackupIntervalMinutes":
                    error = ValidateBackupInterval();
                    break;
                case "BackupToCloud":
                    error = ValidateDDLNotNull(BackupToCloud);
                    break;
                default:
                    Debug.Fail("Unexpected property being validated on NewPatient: " + propertyName);
                    break;
            }

            return error;
        }
        string ValidateCloudDirectory()
        {
            if (string.IsNullOrEmpty(CloudDirectory))
            {
                return Strings.Field_Error_Empty;
            }
            if (CloudDirectory.Length > 1204)
            {
                return Strings.CloudDirectory_Error_StringLengthTooLong;
            }
            if (!Directory.Exists(CloudDirectory))
            {
                return Strings.CloudDirectory_Error_DirectoryNotFound;
            }
            return null;
        }
        private const int MinsInDay = 60 * 24;
        string ValidateBackupInterval()
        {
            if (BackupIntervalMinutes==null)
            {
                return Strings.Field_Error_Empty;
            }
            if (BackupIntervalMinutes <1 || BackupIntervalMinutes > MinsInDay)
            {
                return Strings.CloudDirectory_Error_IntervalOutOfRange;
            }
            return null;
        }

        #endregion // Validation
    }
}
