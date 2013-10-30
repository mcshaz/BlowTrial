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
        OutcomeAt28DaysOption OutcomeAt28Days {get; set;}
        DateTime DateTimeBirth {get;set;}
        DateTime? DischargeDateTime {get; set;}
        DateTime? DeathOrLastContactDateTime {get;set;}
        CauseOfDeathOption CauseOfDeath {get; set;}
        bool IsInterventionArm {get; set;}
        ICollection<VaccineAdministered> VaccinesAdministered {get; set;}
    }
}
