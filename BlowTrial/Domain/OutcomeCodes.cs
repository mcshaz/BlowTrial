using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Outcomes
{
    public enum CauseOfDeathOption
    {
        Missing = 0,
        Infection = 1,
        CongenitalMalformation = 2,
        HyalineMembraneDisease = 3,
        IntraventricularHaemorrhage = 4,
        NecrotisingEnterocollitis = 5,
        Other = 6,
        Unknown = 7
    }
    public enum OutcomeAt28DaysOption
    {
        Missing = 0,
        InpatientAt28Days = 1,
        DiedInHospitalBefore28Days = 2,
        DischargedBefore28Days = 3, //Should not be final outcome
        DischargedAndKnownToHaveSurvived = 4,
        DischargedAndKnownToHaveDied = 5,
        DischargedAndLikelyToHaveSurvived = 6,
        DischargedAndLikelyToHaveDied = 7,
        DischargedAndOutcomeCompletelyUnknown = 8
    }
    public enum DataRequiredOption
    {
        DetailsMissing,
        OutcomeRequired,
        BcgDataRequired,
        AwaitingOutcomeOr28,
        Complete
    }
    public enum RandomisationArm
    {
        NotSet,
        Control,
        RussianBCG,
        DanishBcg
    }
}
