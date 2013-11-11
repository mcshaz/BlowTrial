using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Interfaces
{
    interface IParticipant
    {
        OutcomeAt28DaysOption OutcomeAt28Days {get;}
        DateTime DateTimeBirth {get;}
        DateTime? DischargeDateTime {get;}
        DateTime? DeathOrLastContactDateTime {get;}
        CauseOfDeathOption CauseOfDeath {get;}
        bool IsInterventionArm {get;}
        ICollection<VaccineAdministered> VaccinesAdministered {get;}
    }
}
