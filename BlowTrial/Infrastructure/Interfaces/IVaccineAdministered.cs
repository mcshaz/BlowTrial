using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Interfaces
{
    public interface IVaccineAdministered
    {
        Vaccine VaccineGiven { get; set; }
        Guid Id { get; set; }
        Guid ParticipantId { get; set; }
        Guid VaccineId { get; set; }
    }
}
