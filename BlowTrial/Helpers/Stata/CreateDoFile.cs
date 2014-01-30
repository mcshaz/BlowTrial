using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace BlowTrial.Helpers.Stata
{
    public static class CreateDoFile
    {
        public static string Participant(string csvFileName, string lastVaxName, IEnumerable<KeyValuePair<int,string>> centres, char delimiter)
        {
            var replaceDictionary = new Dictionary<string, string>();
            replaceDictionary.Add("delimiter", GetDelimitOption(delimiter));
            replaceDictionary.Add("csvFile", csvFileName);
            replaceDictionary.Add("lastvax", Regex.Replace(lastVaxName,@"\W","").ToLower());
            replaceDictionary.Add("centreNames", CentresToLabels(centres));
            replaceDictionary.Add("stataFile", Path.ChangeExtension(csvFileName,"dta"));

            return ReadTemplateFile("ParticipantDataStataTemplate.txt", replaceDictionary);
        }

        public static string ScreenedPatients(string csvFileName, IEnumerable<KeyValuePair<int, string>> centres, char delimiter)
        {
            var replaceDictionary = new Dictionary<string, string>();
            replaceDictionary.Add("delimiter", GetDelimitOption(delimiter));
            replaceDictionary.Add("csvFile", csvFileName);
            replaceDictionary.Add("centreNames", CentresToLabels(centres));
            replaceDictionary.Add("stataFile", Path.ChangeExtension(csvFileName, "dta"));

            return ReadTemplateFile("ScreenedPatientStataTemplate.txt", replaceDictionary);
        }

        public static string ProtocolViolations(string csvFileName, IEnumerable<KeyValuePair<IntegerRange, string>> centres, char delimiter)
        {
            var replaceDictionary = new Dictionary<string, string>();
            replaceDictionary.Add("delimiter", GetDelimitOption(delimiter));
            replaceDictionary.Add("csvFile", csvFileName);
            replaceDictionary.Add("stataFile", Path.ChangeExtension(csvFileName, "dta"));
            replaceDictionary.Add("centreNames", CentresToLabels(centres.Select(c=>new KeyValuePair<int, string>(c.Key.Min, c.Value))));
            replaceDictionary.Add("centreDefs", string.Join(Environment.NewLine, centres.Select(c=>string.Format("replace centreid={0} if id>={0} & id<={1}",c.Key.Min, c.Key.Max))));

            return ReadTemplateFile("ProtocolViolationStataTemplate.txt", replaceDictionary);
        }

        static string GetDelimitOption(char delimiter)
        {
            switch (delimiter)
            {
                case '\t':
                    return "tab";
                case ',':
                    return "comma";
                default:
                    return "delimiter(\"" + delimiter + "\")";
            }
        }

        static string CentresToLabels(IEnumerable<KeyValuePair<int,string>> centres)
        {
            return string.Join(" ", centres.Select(c => c.Key.ToString() + " \"" + c.Value.ToString() + '"'));
        }

        static string ReadTemplateFile(string fileName, IDictionary<string, string> replacements)
        {
            var uri = new Uri("TextTemplates/" + fileName, UriKind.Relative);
            var resource = Application.GetContentStream(uri);
            StreamReader sr = new StreamReader(resource.Stream);
            StringBuilder sb = new StringBuilder((int)(resource.Stream.Length*1.3));
            string line;
            while ( ( line = sr.ReadLine() ) != null ) {
                int replaceStart = 0;
                while ((replaceStart = line.IndexOf("<%", replaceStart)) > -1)
                {
                    int replaceEnd = line.IndexOf("%>", replaceStart) + 2;
                    int replaceLength = replaceEnd - replaceStart -4;
                    string replaceKey = line.Substring(replaceStart + 2, replaceLength).Trim();
                    line = line.Substring(0, replaceStart) + replacements[replaceKey] + line.Substring(replaceEnd);
                }
                sb.AppendLine(line);
            }
            sr.Close();
            return sb.ToString();
        }

    }
}
