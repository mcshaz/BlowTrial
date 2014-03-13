using BlowTrial.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BlowTrial.TextTemplates
{
    public abstract class StataData
    {
        public StataData(string csvFileName, char delimiter)
        {
            CsvFilename = csvFileName;
            Delimiter = delimiter;
        }
        public char Delimiter { get; private set; }
        public string CsvFilename { get; private set; }
        public string StataFilename { get { return Path.ChangeExtension(CsvFilename, "dta"); } }
        public static string GetStataVarname(string name)
        {
            return Regex.Replace(name,@"\W","").ToLower();
        }
        public static string GetStataLabel(IEnumerable<KeyValuePair<int,string>> centres)
        {
            return string.Join(" ", centres.Select(c => c.Key.ToString() + " \"" + c.Value.ToString() + '"'));
        }
        public string DelimitOption
        {
            get
            {
                switch (Delimiter)
                {
                    case '\t':
                        return "tab";
                    case ',':
                        return "comma";
                    default:
                        return "delimiter(\"" + Delimiter + "\")";
                }
            }
        }
    }
    public class CentreStataData : StataData
    {
        public CentreStataData(string csvFileName, char delimiter, IEnumerable<KeyValuePair<int, string>> centres)
            : base(csvFileName, delimiter)
        {
            Centres = centres;
        }
        public IEnumerable<KeyValuePair<int,string>> Centres {get; private set;}
        public string CentresLabel
        {
            get
            {
                return GetStataLabel(Centres);
            }
        }
    }
    public class CentreRangeStataData : CentreStataData
    {
        public CentreRangeStataData(string csvFileName, char delimiter, IEnumerable<KeyValuePair<IntegerRange, string>> centres)
            : base(csvFileName, delimiter, centres.Select(c => new KeyValuePair<int, string>(c.Key.Min, c.Value)))
        {
            CentreRanges = centres.Select(c => c.Key);
        }

        public IEnumerable<IntegerRange> CentreRanges { get; private set; }
    }
    public class ParticipantStataData : CentreStataData
    {
        public ParticipantStataData(string csvFileName, char delimiter, IEnumerable<KeyValuePair<int, string>> centres, IEnumerable<string> vaccines)
            : base(csvFileName, delimiter, centres)
        {
            Vaccines = vaccines.Select(v=>GetStataVarname(v));
        }
        public IEnumerable<string> Vaccines { get; private set; }
    }
    public partial class ParticipantDataStataTemplate
    {
        ParticipantStataData _data;
        public ParticipantDataStataTemplate(ParticipantStataData data)
        {
            _data = data;
        }
    }
    public partial class ProtocolViolationStataTemplate
    {
        CentreRangeStataData _data;
        public ProtocolViolationStataTemplate(CentreRangeStataData data)
        {
            _data = data;
        }
    }
    public partial class ScreenedPatientStataTemplate
    {
        CentreStataData _data;
        public ScreenedPatientStataTemplate(CentreStataData data)
        {
            _data = data;
        }
    }
}
