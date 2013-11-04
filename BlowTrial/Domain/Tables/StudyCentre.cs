using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media;

namespace BlowTrial.Domain.Tables
{
    public class StudyCentre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [StringLength(128)]
        public String Name { get; set; }
        public int ArgbTextColour { get; set; }
        public int ArgbBackgroundColour { get; set; }
    }
}
