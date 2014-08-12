using BlowTrial.Domain.Tables;
using GenericToDataString;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.CSV
{
    public static class PatientDataToCSV
    {
        public static void ParticipantDataToCSV(IList<Participant> allParticipants, IEnumerable<Vaccine> allVaccines, char delimiter, string dateFormat, bool encloseStringInQuotes, bool encloseDateInQuotes,TextWriter writer)
        {
            var participants = ListConverters.ToStringValues(allParticipants, CSVOptions(dateFormat, encloseStringInQuotes, encloseDateInQuotes));
            for (int c=0;c<participants.PropertiesDetail.Count;c++)
            {
                if (c>0) {writer.Write(delimiter);}
                writer.Write(participants.PropertiesDetail[c].Name);
            }
            foreach (var v in allVaccines)
            {
                writer.Write(delimiter);
                writer.Write(ListConverters.StataSafeVarname(v.Name, ""));
            }
            writer.Write("\r\n");
            int[] vaccineIds = allVaccines.Select(v => v.Id).ToArray();
            for (int r=0;r<participants.StringValues.Length;r++)
            {
                writer.Write(participants.StringValues[r][0]);
                for (int c = 1; c < participants.PropertiesDetail.Count;c++ )
                {
                    writer.Write(delimiter);
                    writer.Write(participants.StringValues[r][c]);
                }
                foreach (int vid in vaccineIds)
                {
                    writer.Write(delimiter);
                    var admin = allParticipants[r].VaccinesAdministered.FirstOrDefault(va => va.VaccineId == vid);
                    if (admin != null)
                    {
                        writer.Write(admin.AdministeredAt.ToString(dateFormat));
                    }
                }
                writer.Write("\r\n");
            }
        }

        internal static DataTypeOption[] CSVOptions(string dateFormat, bool encloseStringInQuotes, bool encloseDateInQuotes)
        {
            var returnvar = new List<DataTypeOption>();
            if (encloseStringInQuotes)
            {
                returnvar.Add(new DataTypeOption(typeof(string),(s,attr) => '"' + (string)s + '"'));
            }
            if (encloseDateInQuotes)
            {
                returnvar.Add(new DataTypeOption(typeof(DateTime),(d,attr) => '"' + ((DateTime)d).ToString(dateFormat) + '"'));
            }
            else
            {
                returnvar.Add(new DataTypeOption(typeof(DateTime),(d,attr)=>((DateTime)d).ToString(dateFormat)));
            }
            return returnvar.ToArray();
        }

    }
}
