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
    public class GetAppSettingsModel :ValidationBase
    {
        public GetAppSettingsModel()
        {
            StudySitesData = new List<StudySiteDataModel>();
            _validatedProperties = new string[] { "PatientsPreviouslyRandomised", "StudySitesData", "BackupToCloud" };

        }
        public bool? BackupToCloud { get; set; }
        public List<StudySiteDataModel> StudySitesData { get; private set; }
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
                case "StudySitesData":
                    error = ValidateStudySitesData();
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
        string ValidateStudySitesData()
        {
            if (StudySitesData.Any(s=>!s.IsValid))
            {
                return Strings.GetAppSettingsModel_Error_StudySite;
            }
            return null;
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

    public class StudySiteDataModel : ValidationBase
    {
        public StudySiteDataModel()
        {
            _validatedProperties = new string[] { "SiteBackgroundColour", "SiteTextColour", "SiteName" };
        }
        public virtual GetAppSettingsModel AppSetting { get; set; }
        public Guid Id { get; set; }
        public string SiteName { get; set; }
        public Color? SiteBackgroundColour { get; set; }
        public Color? SiteTextColour { get; set; }

        public override string GetValidationError(string propertyName)
        {
            if (!_validatedProperties.Contains(propertyName))
            { return null; }

            string error = null;

            switch (propertyName)
            {
                case "SiteName":
                    error = ValidateSiteName();
                    break;
                case "SiteBackgroundColour":
                    error = ValidateSiteBackgroundColour();
                    break;
                case "SiteTextColour":
                    error = ValidateSiteTextColour();
                    break;
                default:
                    Debug.Fail("Unexpected property being validated on StudySiteDataModel: " + propertyName);
                    break;
            }
            return error;
        }
        string ValidateSiteName()
        {
            if (SiteBackgroundColour==null && SiteTextColour==null)
            {
                return null;
            }
            string error = ValidateFieldNotEmpty(SiteName);
            if (error == null && AppSetting.StudySitesData.Any(s => s.Id != Id && s.SiteName == SiteName))
            {
                error = Strings.GetAppSettingsModel_Error_DuplicateSiteName;
            }
            return error;
        }
        string ValidateSiteTextColour()
        {
            string error = null;
            if ((!string.IsNullOrEmpty(SiteName) || SiteBackgroundColour != null) 
                && SiteTextColour == null)
            {
                error = Strings.StudySiteDataModel_Error_NoColour;
            }
            if (error == null && AppSetting.StudySitesData.Any(s => s.Id != Id && s.SiteTextColour == SiteTextColour))
            {
                error = Strings.GetAppSettingsModel_Error_DuplicateColours;
            }
            return error;
        }
        string ValidateSiteBackgroundColour()
        {
            string error = null;
            if ((!string.IsNullOrEmpty(SiteName) || SiteTextColour != null)
                && SiteBackgroundColour == null)
            {
                error = Strings.StudySiteDataModel_Error_NoColour;
            }
            if (error == null && AppSetting.StudySitesData.Any(s => s.Id != Id && s.SiteBackgroundColour == SiteBackgroundColour))
            {
                error = Strings.GetAppSettingsModel_Error_DuplicateColours;
            }
            return error;
        }

    }

}
