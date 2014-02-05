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
            string participantsCsv = ListConverters.ToCSV<ParticipantCsvModel>(allParticipants,delimiter, CSVOptions(dateFormat, encloseStringInQuotes, encloseDateInQuotes));

            string[] csvLines = participantsCsv.Split(new string[]{"\r\n"}, StringSplitOptions.None);

            csvLines[0] += delimiter + string.Join(delimiter.ToString(), allVaccines.Select(v=>v.Name));
            for (int i=0; i< allParticipants.Count;i++)
            {
                foreach (var v in allVaccines)
                {
                    var admin = allParticipants[i].VaccinesAdministered.FirstOrDefault(va => va.VaccineGiven.Id == v.Id);
                    
                    if (admin==null)
                    {
                        csvLines[i] += delimiter;
                    }
                    else
                    {
                        csvLines[i] += delimiter + admin.AdministeredAt.ToString(dateFormat);
                    }
                }
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
                returnvar.Add(new DataTypeOption<DateTime>(d => d.ToString(dateFormat)));
            }
            return returnvar.ToArray();
        }

    }
}
