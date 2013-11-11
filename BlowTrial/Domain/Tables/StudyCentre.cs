using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace BlowTrial.Domain.Tables
{
    public class StudyCentre : ISharedRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public Guid DuplicateIdCheck { get; set; }
        [StringLength(128)]
        public String Name { get; set; }
        public int ArgbTextColour { get; set; }
        public int ArgbBackgroundColour { get; set; }
        [StringLength(16)]
        public string PhoneMask { get; set; }
        [StringLength(16)]
        public string HospitalIdentifierMask { get; set; }

        public DateTime RecordLastModified { get; set; }

        public virtual ICollection<Participant> Participants { get; set; }
    }
}
