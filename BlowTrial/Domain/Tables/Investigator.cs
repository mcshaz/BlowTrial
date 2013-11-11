using BlowTrial.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Tables
{
    public class Investigator : IUser
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [StringLength(64)]
        public String Username { get; set; }
        [StringLength(128)]
        public string Password { get; set; }
        public DateTime LastLoginAt { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
        /*
        public virtual ICollection<Participant> ParticipantsEnroled { get; set; }
        public virtual ICollection<ScreenedPatient> ScreenedPatientsRegistered { get; set; }
        public virtual ICollection<ProtocolViolation> ReportedViolations { get; set; }
        */
    }

    public class Role
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Investigator> Investigators { get; set; }
    }
}
