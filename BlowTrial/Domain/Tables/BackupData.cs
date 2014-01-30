using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Tables
{
    public class BackupData
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BackupIntervalMinutes { get; set; }
        public bool IsBackingUpToCloud { get; set; }
        public bool IsEnvelopeRandomising { get; set; }
    }

    public class CloudDirectory
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Path { get; set; }
    }

    /// <summary>
    /// This exists because some sights have subtly different local ethics aproval
    /// </summary>
    public class RandomisingMessage
    {
        [Key]
        public int Id { get; set; }
        [StringLength(200)]
        public string InterventionInstructions { get; set; }
        [StringLength(200)]
        public string ControlInstructions { get; set; }
    }
}
