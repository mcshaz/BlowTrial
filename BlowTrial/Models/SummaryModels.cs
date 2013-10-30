using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Models
{
    public class ParticipantsSummary
    {
        public int TotalCount { get; set; }
        public int InterventionArmCount { get; set; }
        public double InterventionArmFrac { get { return (double)InterventionArmCount / (double)TotalCount; } }
        public int ControlArmCount { get { return TotalCount - InterventionArmCount; } }
        public double ControlArmFrac { get { return 1 - InterventionArmFrac; } }
        public int CompletedRecordCount { get; set; }
        public int PendingRecordCount { get { return TotalCount - CompletedRecordCount; } }
    }
    public class ScreenedPatientsSummary
    {
        public int TotalCount { get; set; }
        public int LikelyDie24HrCount { get; set; }
        public int BadMalformCount { get; set; }
        public int BadInfectnImmuneCount { get; set; }
        public int WasGivenBcgPriorCount { get; set; }
        public int RefusedConsentCount { get; set; }
        public int MissedCount { get; set; }
    }
}
