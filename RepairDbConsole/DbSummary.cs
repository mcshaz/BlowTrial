using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairDbConsole
{
    class DbSummary
    {
        public string CentreName { get; set; }
        public int Participants { get; set; }
        public DateTime MostRecentEnrollment { get; set; }
        public DateTime MostRecentDemographicUpdate { get; set; }
        public DateTime MostRecentVaccineAdmin { get; set; }
        public DateTime MostRecentVaccineUpdate { get; set; }
        public int TotalVaccinesAdmin { get; set; }
        public int ParticipantsWithVaccinesAdmin { get; set; }
    }
}
