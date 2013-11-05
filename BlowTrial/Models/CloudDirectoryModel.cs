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
                "CloudDirectories",
                "BackupIntervalMinutes",
                "IsBackingUpToCloud"
            };
            CloudDirectoryItems = new List<DirectoryItemModel>();
        }
        #endregion //Constructors

        #region Fields
        #endregion // Fields

        #region Properties
        public IList<DirectoryItemModel> CloudDirectoryItems { get; private set; }
        public IEnumerable<string> CloudDirectories
        {
            set
            {
                foreach (string s in value)
                {
                    CloudDirectoryItems.Add(new DirectoryItemModel{ DirectoryPath = s});
                }
            }
        }
        public int? BackupIntervalMinutes { get; set; }
        public bool? IsBackingUpToCloud { get; set; }
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
                case "CloudDirectories":
                    error = ValidateCloudDirectories();
                    break;
                case "BackupIntervalMinutes":
                    error = ValidateBackupInterval();
                    break;
                case "IsBackingUpToCloud":
                    error = ValidateDDLNotNull(IsBackingUpToCloud);
                    break;
                default:
                    Debug.Fail("Unexpected property being validated on NewPatient: " + propertyName);
                    break;
            }

            return error;
        }
        string ValidateCloudDirectories()
        {
            if (!CloudDirectoryItems.Any())
            {
                return Strings.CloudDirectoryModel_Error_NoDirectories;
            }
            if (CloudDirectoryItems.Any(c => !c.IsValid()))
            {
                return Strings.CloudDirectoryModel_Error_InValid;
            }
            if (CloudDirectoryItems.Count > 1 && IsBackingUpToCloud!=false)
            {
                return Strings.CloudDirectoryModel_Error_ExcessDirectories;
            }
            if (CloudDirectoryItems.GroupBy(i => i.DirectoryPath).Any(g => g.Count() > 1))
            {
                return Strings.CloudDirectoryModel_Error_DuplicateDirectory;
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
    public class DirectoryItemModel : ValidationBase
    {
        public DirectoryItemModel()
        {
            _validatedProperties = new string[] { "DirectoryPath" };
        }
        public string DirectoryPath { get; set; }
        string ValidateCloudDirectory()
        {
            if (string.IsNullOrEmpty(DirectoryPath))
            {
                return Strings.Field_Error_Empty;
            }
            if (DirectoryPath.Length > 1204)
            {
                return Strings.CloudDirectory_Error_StringLengthTooLong;
            }
            if (!Directory.Exists(DirectoryPath))
            {
                return Strings.CloudDirectory_Error_DirectoryNotFound;
            }
            return null;
        }
        public override string GetValidationError(string propertyName)
        {
            if (!_validatedProperties.Contains(propertyName))
            { return null; }

            string error = null;

            switch (propertyName)
            {
                case "DirectoryPath":
                    error = ValidateCloudDirectory();
                    break;
            }
            return error;

        }

    }
}
