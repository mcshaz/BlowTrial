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
    public enum AllocationGroups {NotApplicable = 0, IndiaTwoArm = 1, IndiaThreeArmUnbalanced=2, IndiaThreeArmBalanced=3}
    public class AllocationBlock : ISharedRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public byte GroupRepeats { get; set; }
        public AllocationGroups AllocationGroup { get; set; }
        public RandomisationStrata RandomisationCategory { get; set; }
        public DateTime RecordLastModified { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
    }
}
