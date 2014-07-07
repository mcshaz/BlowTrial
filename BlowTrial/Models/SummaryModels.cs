using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Models
{
    public class ParticipantsSummary
    {
        public int TotalCount() { return TrialArmCounts.Values.Sum(); }
        public Dictionary<RandomisationArm, int> TrialArmCounts { get; set; }
        public IEnumerable<KeyValuePair<RandomisationArm, Double>> FractionsInArm() 
        {
            double totalCount = TotalCount();
            return TrialArmCounts.Select(t => new KeyValuePair<RandomisationArm, double>(t.Key, t.Value/totalCount)); 
        }
        public Dictionary<DataRequiredOption, int> DataRequiredCount { get; set; }
        public IEnumerable<KeyValuePair<DataRequiredOption, Double>> DataRequiredFractions()
        {
            double totalCount = TotalCount();
            return DataRequiredCount.Select(d => new KeyValuePair<DataRequiredOption, double>(d.Key, d.Value / totalCount));
        }
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
