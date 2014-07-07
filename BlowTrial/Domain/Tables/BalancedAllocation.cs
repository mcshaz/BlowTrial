using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Infrastructure.Randomising;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.Domain.Tables
{
    public class BalancedAllocation : ISharedRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int StudyCentreId { get; set; }
        public RandomisationStrata RandomisationCategory { get; set; }
        public bool IsEqualised { get; set; }
        public DateTime RecordLastModified { get; set; }

        [ForeignKey("StudyCentreId")]
        public virtual StudyCentre Centre { get; set; }
    }
}
