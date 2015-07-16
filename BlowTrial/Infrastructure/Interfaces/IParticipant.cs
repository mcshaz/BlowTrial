using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Interfaces
{
    public interface IParticipant
    {
        OutcomeAt28DaysOption OutcomeAt28Days {get;}
        DateTime DateTimeBirth {get;}
        DateTime? DischargeDateTime {get;}
        DateTime? DeathOrLastContactDateTime {get;}
        CauseOfDeathOption CauseOfDeath {get;}
        RandomisationArm TrialArm {get;}
        MaternalBCGScarStatus MaternalBCGScar { get; }
        FollowUpBabyBCGReactionStatus FollowUpBabyBCGReaction { get; }
        bool PermanentlyUncontactable { get; }
        DateTime RegisteredAt { get; }

        ICollection<VaccineAdministered> VaccinesAdministered {get;}
        ICollection<ProtocolViolation> ProtocolViolations { get; }
        ICollection<UnsuccessfulFollowUp> UnsuccessfulFollowUps { get; }
    }
}
