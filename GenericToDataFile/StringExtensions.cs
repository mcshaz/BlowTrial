using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericToDataString
{
    public static class StringExtensions
    {
        public static string ToSeparatedList(this IEnumerable<string> st, char seperationCharacter)
        {
            string seperationSt = seperationCharacter.ToString();
            return string.Join(seperationSt, st.Select(s => s.Replace(seperationSt, '\\' + seperationSt)));
        }
    }
}
