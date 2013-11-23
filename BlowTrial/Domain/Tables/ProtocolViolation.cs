using BlowTrial.Infrastructure.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlowTrial.Domain.Tables
{
    public class ProtocolViolation : ISharedRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [ForeignKey("Participant")]
        public int ParticipantId { get; set; }
        public string Details { get; set; }
        public bool MajorViolation { get; set; }
        public string ReportingInvestigator { get; set; }
        public DateTime ReportingTimeLocal { get; set; }

        public DateTime RecordLastModified { get; set; }

        public virtual Participant Participant { get; set; }
        /*
        [ForeignKey("ReportingInvestigator")]
        public virtual Investigator ReportingInvestigator { get; set; }
        */
    }
}
