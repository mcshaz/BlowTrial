using BlowTrial.Domain.Tables;
using GenericToDataString;
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
            string participantsCsv = ListConverters.ToCSV(allParticipants,delimiter, CSVOptions(dateFormat, encloseStringInQuotes, encloseDateInQuotes));

            string[] csvLines = participantsCsv.Split(new string[]{"\r\n"}, StringSplitOptions.None);

            csvLines[0] += delimiter + string.Join(delimiter.ToString(), allVaccines.Select(v => ListConverters.StataSafeVarname(v.Name, "")));

            int[] vaccineIds = allVaccines.Select(v => v.Id).ToArray();
            StringBuilder vaccineDates = new StringBuilder();
            for (int i=0; i< allParticipants.Count;i++)
            {
                vaccineDates.Clear();
                foreach (int vid in vaccineIds)
                {
                    vaccineDates.Append(delimiter);
                    var admin = allParticipants[i].VaccinesAdministered.FirstOrDefault(va => va.VaccineId == vid);
                    if (admin!=null)
                    {
                         vaccineDates.Append(admin.AdministeredAt.ToString(dateFormat));
                    }
                }
                csvLines[i+1] += vaccineDates.ToString();
            }

            return csvLines;
        }

        internal static DataTypeOption[] CSVOptions(string dateFormat, bool encloseStringInQuotes, bool encloseDateInQuotes)
        {
            var returnvar = new List<DataTypeOption>();
            if (encloseDateInQuotes)
            {
                returnvar.Add(new DataTypeOption<string>(s => '"' + s + '"'));
            }
            if (encloseDateInQuotes)
            {
                returnvar.Add(new DataTypeOption<DateTime>(d => '"' + d.ToString(dateFormat) + '"'));
            }
            else
            {
                returnvar.Add(new DataTypeOption<DateTime>(dateFormat));
            }
            return returnvar.ToArray();
        }

    }
}
