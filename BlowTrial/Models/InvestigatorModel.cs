using BlowTrial.Domain.Interfaces;
using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace BlowTrial.Models
{
    public class InvestigatorModel : IUser
    {
        public Guid Id { get; set; }
        [StringLength(64)]
        public String Username { get; set; }
        [StringLength(128)]
        public string Password { get; set; }
        public DateTime LastLoginAt { get; set; }
        public int FailedPwdAttempts { get; set; }
        public bool IsLockedOut { get; set; }

        public virtual ICollection<Participant> ParticipantsEnrolled { get; set; }
        public virtual ICollection<ScreenedPatient> ScreenedPatientsRegistered { get; set; }
        public virtual ICollection<BlowTrial.Domain.Tables.Role> Roles { get; set; }
    }
}
