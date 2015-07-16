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
    public enum MaternalBCGScarStatus
    {
        Missing=0,
        NoScar=1,
        ScarPresent=2,
        Unascertainable =3
    }

    public enum FollowUpBabyBCGReactionStatus{
        Missing=0,
        NoReaction=1,
        PapuleOnly=2,
        PustuleOrScar=3,
    }

    public enum DataRequiredOption
    {
        NotSet = 0,
        DetailsMissing = 1,
        OutcomeRequired = 2,
        BcgDataRequired = 3,
        AwaitingOutcomeOr28 =4,

        MaternalBCGScarDetails = 6,
        Awaiting6WeeksToElapse = 7,
        AwaitingInfantScarDetails=8,
        FailedInitialContact=9,

        Complete = 20
    }
    public enum RandomisationArm
    {
        NotSet = 0,
        Control,
        RussianBCG,
        DanishBcg,
        MoreauBcg,
        JapanBcg,
        GreenSignalBcg
    }
}
