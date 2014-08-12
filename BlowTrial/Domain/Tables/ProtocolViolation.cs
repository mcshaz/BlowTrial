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
        public ViolationTypeOption ViolationType { get; set; }
        public string ReportingInvestigator { get; set; }
        public DateTime ReportingTimeLocal { get; set; }

        public DateTime RecordLastModified { get; set; }

        public virtual Participant Participant { get; set; }
        /*
        [ForeignKey("ReportingInvestigator")]
        public virtual Investigator ReportingInvestigator { get; set; }
        */
    }
    public enum ViolationTypeOption
    {
        NotSelected = 0,
        Minor = 1,
        MajorWrongTreatment = 2,
        MajorWrongAllocation = 3,
        MajorOther = 4
    }
}
