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
    public class StudySitesModel :ValidationBase
    {
        public StudySitesModel()
        {
            StudySitesData = new List<StudySiteItemModel>();
            _validatedProperties = new string[] { "StudySitesData" };

        }

        public List<StudySiteItemModel> StudySitesData { get; private set; }

        public override string GetValidationError(string propertyName)
        {
            if (!_validatedProperties.Contains(propertyName))
            { return null; }

            string error = null;

            switch (propertyName)
            {
                case "StudySitesData":
                    error = ValidateStudySitesData();
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
            var firstSite = StudySitesData.FirstOrDefault();
            if (firstSite==null || firstSite.SiteName==null)
            {
                return Strings.GetAppSettingsModel_Error_NoStudySites;
            }
            return null;
        }
    }

    public class StudySiteItemModel : ValidationBase
    {
        public StudySiteItemModel()
        {
            _validatedProperties = new string[] { "SiteBackgroundColour", "SiteTextColour", "SiteName" };
        }
        public virtual StudySitesModel AllLocalSites { get; set; }
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
            if (error == null && AllLocalSites.StudySitesData.Any(s => s.Id != Id && s.SiteName == SiteName))
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
            if (error == null && AllLocalSites.StudySitesData.Any(s => s.Id != Id && s.SiteTextColour == SiteTextColour))
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
            if (error == null && AllLocalSites.StudySitesData.Any(s => s.Id != Id && s.SiteBackgroundColour == SiteBackgroundColour))
            {
                error = Strings.GetAppSettingsModel_Error_DuplicateColours;
            }
            return error;
        }

    }

}
