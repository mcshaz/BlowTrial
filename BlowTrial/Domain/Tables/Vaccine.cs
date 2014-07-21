using BlowTrial.Domain.Providers;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Properties;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlowTrial.Domain.Tables
{
    public class Vaccine : ISharedRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [StringLength(16)]
        public string Name { get; set; }
        public DateTime RecordLastModified { get; set; }
    }
    [Table(VaccineAdminTableName)]
    public class VaccineAdministered : ISharedRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int VaccineId { get; set; }
        public int ParticipantId { get; set; }
        public DateTime RecordLastModified { get; set; }
        public DateTime AdministeredAt { get; set; }
        [ForeignKey("VaccineId")]
        public virtual Vaccine VaccineGiven { get; set; }
        [ForeignKey("ParticipantId")]
        public virtual Participant AdministeredTo { get; set; }
        internal const string VaccineAdminTableName = "VaccinesAdministered";
    }
}
