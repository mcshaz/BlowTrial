using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace BlowTrial.Domain.Tables
{
    public class StudyCentre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        [StringLength(128)]
        public String Name { get; set; }
        public int ArgbTextColour { get; set; }
        public int ArgbBackgroundColour { get; set; }
        public DateTime RecordLastModified { get; set; }
    }
}
