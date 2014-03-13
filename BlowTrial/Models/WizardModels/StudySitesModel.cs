using BlowTrial.Infrastructure;
using BlowTrial.Properties;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;
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
        public StudySitesModel() : this(new List<StudySiteItemModel>())
        {
        }

        public StudySitesModel(IEnumerable<StudySiteItemModel> siteItems)
        {
            _validatedProperties = new string[] { "StudySitesData" };
            StudySitesData = new List<StudySiteItemModel>(siteItems);
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
                    Debug.Fail("Unexpected property being validated on StudySitesModel: " + propertyName);
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
        #region constructor
        public const string DefaultPhoneMask = "(000) 0000-0000";
        public const string DefaultHospitalIdentifierMask = ">L000000";
        public StudySiteItemModel()
        {
            _validatedProperties = new string[] { "Id", "SiteBackgroundColour", "SiteTextColour","SiteName", "PhoneMask", "HospitalIdentifierMask", "MaxParticipantAllocations" };
            PhoneMask = DefaultPhoneMask;
            HospitalIdentifierMask = DefaultHospitalIdentifierMask;
            SiteTextColour = DefaultTextColor;
        }
        #endregion
        #region fields
        static readonly Rgb ErrorColour = new Rgb() { R = 255, G = 0, B = 0 };
        static IColorSpaceComparison _colourSpacecomparison;
        Color? _siteBackgroundColour;
        Color _siteTextColour;
        const double minColourDif = 10;
        const double minContrastDif = 25;
        public static readonly Color DefaultTextColor = new Color() { R = 0, G = 0, B = 0, A = 255 };
        #endregion
        IColorSpaceComparison ColourSpacecomparison
        {
            get
            {
                return _colourSpacecomparison ?? (_colourSpacecomparison = new CieDe2000Comparison());
            }
        }
        public virtual StudySitesModel AllLocalSites { get; set; }
        public int? Id { get; set; }
        public string SiteName { get; set; }
        public Guid DuplicateIdCheck { get; set; }
        public Color? SiteBackgroundColour { 
            get { return _siteBackgroundColour; }
            set
            {
                if (value == _siteBackgroundColour) { return; }
                _siteBackgroundColour = value;
                if (_siteBackgroundColour.HasValue)
                {
                    SiteBackgroundRgb = new Rgb() { R = _siteBackgroundColour.Value.R, G = _siteBackgroundColour.Value.G, B = _siteBackgroundColour.Value.B };
                }
                else
                {
                    SiteBackgroundRgb = null;
                }
            }
        }
        public Color SiteTextColour 
        {
            get { return _siteTextColour;  }
            set
            {
                if (value == _siteTextColour) { return; }
                _siteTextColour = value;
                SiteTextRgb = new Rgb() { R = _siteTextColour.R, G = _siteTextColour.G, B = _siteTextColour.B };
            }
        }
        internal Rgb SiteBackgroundRgb { get; set; }
        internal Rgb SiteTextRgb { get; set; }
        public int? MaxParticipantAllocations { get; set; }
        public int? MaxIdForSite()
        {
            return Id + MaxParticipantAllocations - (Id==1?2:1);
        }
        public string PhoneMask { get; set; }
        public string HospitalIdentifierMask { get; set; }
        int StudyCentreIdIncrement
        {
            get
            {
                return 100;
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
                case "MaxParticipantAllocations":
                    error = ValidateMaxParticipantAllocations();
                    break;
                default:
                    Debug.Fail("Unexpected property being validated on StudySiteDataModel: " + propertyName);
                    break;
            }
            return error;
        }
        string ValidateMaxParticipantAllocations()
        {
            if (MaxParticipantAllocations == null)
            {
                return Strings.Field_Error_Empty;
            }
            if (MaxParticipantAllocations <= 0 )
            {
                return Strings.Int_Error_LessThan1;
            }
            if (MaxParticipantAllocations % StudyCentreIdIncrement != 0)
            {
                return string.Format(Strings.StudySitesModel_Error_InvalidMultiple, StudyCentreIdIncrement);
            }
            return null;
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
            if (Id != 1 && Id % StudyCentreIdIncrement != 0)
            {
                return string.Format(Strings.StudySitesModel_Error_InvalidMultiple, StudyCentreIdIncrement);
            }
            if (AllLocalSites.StudySitesData.Count(s=>s.Id == Id)>1)
            {
                return Strings.StudySitesModel_Error_DuplicateId;
            }
            if (AllLocalSites.StudySitesData.Any(a => a.Id != Id && a.Id <= MaxIdForSite() && a.MaxIdForSite() >= Id))
            {
                return Strings.StudySitesModel_Error_OverlappingIdRanges;
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
            if (SiteBackgroundRgb != null && SiteTextRgb.Compare(SiteBackgroundRgb, this.ColourSpacecomparison) < minContrastDif)
            {
                error = Strings.StudySiteDataModel_Error_TextAndBackgroundMatch;
            }
            return error;
        }
        string ValidateSiteBackgroundColour()
        {
            if (SiteBackgroundColour == null)
            {
                return Strings.StudySiteDataModel_Error_NoColour;
            }
            if (AllLocalSites.StudySitesData.Any(s => s.Id != Id && s.SiteBackgroundRgb != null && s.SiteBackgroundRgb.Compare(SiteBackgroundRgb, this.ColourSpacecomparison) < minColourDif))
            {
                return Strings.GetAppSettingsModel_Error_DuplicateColours;
            }
            if (ErrorColour.Compare(SiteBackgroundRgb, this.ColourSpacecomparison) < minContrastDif)
            {
                return Strings.StudySiteDataModel_Error_BackgroundAndErrorMatch;
            }
            return null;
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
