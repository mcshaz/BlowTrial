using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlowTrial.Domain.Tables
{
    public class UnsuccessfulFollowUp
    {
        [Key]
        public int Id { get; set; }
        public DateTime AttemptedContact { get; set; }
        [ForeignKey("TrialParticipant")]
        public int ParticipantId { get; set; }

        public Participant TrialParticipant { get; set; }
    }
}
