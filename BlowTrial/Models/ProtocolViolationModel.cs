using BlowTrial.Infrastructure;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Models
{
    public class ProtocolViolationModel : ValidationBase
    {
        public ProtocolViolationModel()
        {
            _validatedProperties = new string[] { "MajorViolation", "Details" };
        }

        public ParticipantModel Participant { get; set; }

        public int Id { get; set; }

        public bool? MajorViolation { get; set; }

        public string Details { get; set; }

        public string ReportingInvestigator { get; set; }

        public DateTime ReportingTimeLocal { get; set; }

        public override string GetValidationError(string propertyName)
        {
            if (!_validatedProperties.Contains(propertyName)) { return null; }
            string error = null;
            switch (propertyName)
            {
                case "MajorViolation":
                    error = ValidateDDLNotNull(MajorViolation);
                    break;
                case "Details":
                    error = ValidateFieldNotEmpty(Details);
                    break;
            }
            return error;
        }
    }
}
