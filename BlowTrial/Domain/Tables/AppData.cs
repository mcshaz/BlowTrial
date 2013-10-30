using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Tables
{
    public class AppData
    {
        [Key]
        public int Id { get; set; }
        [StringLength(1024)]
        public string CloudDirectory { get; set; }
        public int BackupIntervalMinutes { get; set; }
        public bool BackupToCloud { get; set; }
    }
}
