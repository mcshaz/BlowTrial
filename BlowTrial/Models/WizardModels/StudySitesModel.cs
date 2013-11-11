using BlowTrial.Infrastructure;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            if (StudySitesData.Any(s=>!s.IsValid()))
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
            _validatedProperties = new string[] { "Id", "SiteBackgroundColour", "SiteTextColour", "SiteName", "PhoneMask", "HospitalIdentifierMask" };
            PhoneMask = "(000) 0000-0000";
            HospitalIdentifierMask = "L000000";
        }
        public virtual StudySitesModel AllLocalSites { get; set; }
        public int? Id { get; set; }
        public string SiteName { get; set; }
        public Color? SiteBackgroundColour { get; set; }
        public Color? SiteTextColour { get; set; }

        public string PhoneMask { get; set; }

        public string HospitalIdentifierMask { get; set; }

        int _studyCentreIdIncrement;
        int StudyCentreIdIncrement
        {
            get
            {
                if (_studyCentreIdIncrement == 0)
                {
                    _studyCentreIdIncrement = int.Parse(ConfigurationManager.AppSettings["StudyCentreIdIncrement"]);
                }
                return _studyCentreIdIncrement;
            }
        }

        public override string GetValidationError(string propertyName)
        {
            if (!_validatedProperties.Contains(propertyName))
            { return null; }

            string error = null;

            switch (propertyName)
            {
                case "Id":
                    error = ValidateId();
                    break;
                case "SiteName":
                    error = ValidateSiteName();
                    break;
                case "SiteBackgroundColour":
                    error = ValidateSiteBackgroundColour();
                    break;
                case "SiteTextColour":
                    error = ValidateSiteTextColour();
                    break;
                case "PhoneMask":
                    error = ValidateMask(PhoneMask, 16);
                    break;
                case "HospitalIdentifierMask":
                    error = ValidateMask(HospitalIdentifierMask, 16);
                    break;
                default:
                    Debug.Fail("Unexpected property being validated on StudySiteDataModel: " + propertyName);
                    break;
            }
            return error;
        }
        string ValidateId()
        {
            if (Id==null)
            {
                return Strings.Field_Error_Empty;
            }
            if (Id <= 0)
            {
                return Strings.Int_Error_LessThan1;
            }
            if (Id % StudyCentreIdIncrement != 0)
            {
                return string.Format(Strings.StudySitesModel_Error_InvalidMultiple, StudyCentreIdIncrement);
            }
            if (AllLocalSites.StudySitesData.Count(s=>s.Id == Id)>1)
            {
                return Strings.StudySitesModel_Error_DuplicateId;
            }
            return null;
        }
        string ValidateSiteName()
        {
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
            if (SiteTextColour == null)
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
            if (SiteBackgroundColour == null)
            {
                error = Strings.StudySiteDataModel_Error_NoColour;
            }
            if (error == null && AllLocalSites.StudySitesData.Any(s => s.Id != Id && s.SiteBackgroundColour == SiteBackgroundColour))
            {
                error = Strings.GetAppSettingsModel_Error_DuplicateColours;
            }
            return error;
        }
        //static readonly char[] InvalidMaskChars = "09#L?Aa&C.><!\\\"".ToCharArray();
        static string ValidateMask(string mask, int stringLength)
        {
            if (string.IsNullOrEmpty(mask))
            {
                return Strings.Field_Error_Empty;
            }
            if (mask.Length > stringLength)
            {
                return string.Format(Strings.Mask_Error_TooLong, stringLength);
            }
            /*
            if (mask.Any(c=>!InvalidMaskChars.Any(i=>i==c)))
            {
                return Strings.Mask_Error_InvalidCharacters;
            }
            */
            return null;
        }
    }

}
