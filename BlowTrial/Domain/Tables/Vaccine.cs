using BlowTrial.Properties;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlowTrial.Domain.Tables
{
    public class Vaccine
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public static string BcgName()
        {
            return Strings.Vaccine_Bcg;
        }
        public static string OpvName()
        {
            return Strings.Vaccine_Opv;
        }
        public static string HepBName()
        {
            return Strings.Vaccine_HepB;
        }
        public bool IsBcg
        {
            get
            {
                return IsBcgName(Name);
            }
        }
        public static bool IsBcgName(string vaccineName)
        {
            return vaccineName == BcgName();
        }
    }
    [Table(VaccineAdminTableName)]
    public class VaccineAdministered
    {
        [Key]
        public int Id { get; set; }
        public int VaccineId { get; set; }
        public int ParticipantId { get; set; }
        public DateTime AdministeredAt { get; set; }
        [ForeignKey("VaccineId")]
        public virtual Vaccine VaccineGiven { get; set; }
        [ForeignKey("ParticipantId")]
        public virtual Participant AdministeredTo { get; set; }
        internal const string VaccineAdminTableName = "VaccinesAdministered";
    }
}
