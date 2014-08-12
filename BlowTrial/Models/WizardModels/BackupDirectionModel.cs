using BlowTrial.Domain.Tables;
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
    public class BackupDirectionModel :CloudDirectoryModel
    {
        public BackupDirectionModel()
            : base(new string[] { "PatientsPreviouslyRandomised", "AllocationType" })
        { }

        public bool PatientsPreviouslyRandomised { get; set; }

        public AllocationGroups AllocationType { get; set; }

        public override string GetValidationError(string propertyName)
        {
           switch (propertyName)
            { 
                case "PatientsPreviouslyRandomised":
                    return ValidatePatientsPreviouslyRandomised();
                case "AllocationType":
                    return ValidateAllocationType();
            }
            return base.GetValidationError(propertyName);
        }
        string ValidateAllocationType()
        {
            return ValidateEnumNotDefault(AllocationType);
        }
        string ValidatePatientsPreviouslyRandomised()
        {
            if (PatientsPreviouslyRandomised && IsBackingUpToCloud != true)
            {
                return Strings.StudySiteDataModel_Error_PatientsPreviouslyRandomisedNotRelevant;
            }
            return null;
        }

    }
}
