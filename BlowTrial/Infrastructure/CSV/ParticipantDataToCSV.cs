using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.CSV
{
    public static class PatientDataToCSV
    {
        public static string[] ParticipantDataToCSV(IList<ParticipantCsvModel> allParticipants, IEnumerable<Vaccine> allVaccines, char delimiter, string dateFormat, bool encloseStringInQuotes, bool encloseDateInQuotes)
        {
            var participantsCsv = CSVconversion.IListToStrings<ParticipantCsvModel>(allParticipants, delimiter, dateFormat, encloseStringInQuotes, encloseDateInQuotes);

            //append row 0 [headers]
            participantsCsv[0] += string.Join(string.Empty, allVaccines.Select(v => delimiter + v.Name));
            for (int i=0; i< allParticipants.Count;i++)
            {
                foreach (var v in allVaccines)
                {
                    var admin = allParticipants[i].VaccinesAdministered.FirstOrDefault(va => va.VaccineGiven.Id == v.Id);
                    
                    if (admin==null)
                    {
                        participantsCsv[i+1] += delimiter;
                    }
                    else
                    {
                        participantsCsv[i+1] += delimiter + admin.AdministeredAt.ToString(dateFormat);
                    }
                }
            }
            return participantsCsv;
        }
    }
}
