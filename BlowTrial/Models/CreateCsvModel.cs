using BlowTrial.Infrastructure;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;

namespace BlowTrial.Models
{
    public enum TableOptions { Participant = 0, ScreenedPatients }
    public class CreateCsvModel : ValidationBase
    {
        public CreateCsvModel()
        {
           _validatedProperties = new string[] { "Filename" /*, "TableType" */ };
        }
        string _filename;
        public string Filename { get { return _filename; }
            set
            {
                if (value.EndsWith(".csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    _filename = value;
                }
                else
                {
                    _filename = value + ".csv";
                }
            }
        }
        public TableOptions TableType { get; set; }

        public override string GetValidationError(string propertyName)
        {
            if (_validatedProperties.Contains(propertyName))
            {
                switch (propertyName)
                {
                    case "Filename":
                        return ValidateFilename();
                    //                case "TableType":
                    //                    return ValidateTableType();
                    default:
                        Debug.Fail("Unexpected property being validated on ParticipantUpdateModel: " + propertyName);
                        break;
                }
            }
            return null;
        }
        string ValidateFilename()
        {
            if (string.IsNullOrEmpty(Filename))
            {
                return Strings.Field_Error_Empty;
            }

            if (!System.IO.Directory.Exists(Path.GetDirectoryName(Filename)))
            {
                return Strings.CreateCsvModel_Error_DirectoryExists;
            }
            return null;
        }
        string ValidateTableType()
        {
            /*
            if (TableType == TableOptions.Missing)
            {
                return Strings.Field_Error_Empty;
            }
            */
            return null;
        }
    }
}
