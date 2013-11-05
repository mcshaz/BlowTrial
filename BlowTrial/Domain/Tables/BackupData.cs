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
}
