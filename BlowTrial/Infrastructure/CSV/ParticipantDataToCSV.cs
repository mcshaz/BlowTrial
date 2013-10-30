using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.CSV
{
    public static class PatientDataToCSV
    {
        public static string[] ParticipantDataToCSV(IList<ParticipantCsvModel> allParticipants, IEnumerable<Vaccine> allVaccines)
        {
            var participantsCsv = CSVconversion.IListToStrings<ParticipantCsvModel>(allParticipants);

            //append row 0 [headers]
            participantsCsv[0] += string.Join(string.Empty, allVaccines.Select(v => ',' + v.Name));
            for (int i=0; i< allParticipants.Count;i++)
            {
                foreach (var v in allVaccines)
                {
                    var admin = allParticipants[i].VaccinesAdministered.FirstOrDefault(va => va.VaccineGiven.Id == v.Id);
                    
                    if (admin==null)
                    {
                        participantsCsv[i+1] += ',';
                    }
                    else
                    {
                        participantsCsv[i+1] += ',' + admin.AdministeredAt.ToString("u");
                    }
                }
            }
            return participantsCsv;
        }
    }
}
