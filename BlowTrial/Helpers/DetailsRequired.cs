using BlowTrial.Domain.Outcomes;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Helpers
{
    public static class DataRequiredStrings
    {
        const string optionPrefix = "DataRequiredOption_";
        static IDictionary<DataRequiredOption, string> _outcomeDict;
        public static string GetDetails(DataRequiredOption option)
        {
            if (_outcomeDict==null)
            {
                var rm = Strings.ResourceManager;
                _outcomeDict = Enum.GetValues(typeof(DataRequiredOption))
                    .Cast<DataRequiredOption>()
                    .ToDictionary(d => d,
                        d => rm.GetString(optionPrefix + d.ToString()));
            }
            return _outcomeDict[option];
        }
    }
}
