using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Extensions
{
    static class StringEnumerationExtensions
    {
        //http://stackoverflow.com/questions/5813605/c-sharp-splitting-on-a-pipe-with-a-an-escaped-pipe-in-the-data
        /// <summary>
        /// split strings on comma
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static IEnumerable<string> FromSeperatedList(this string s, char seperationCharacter)
        {
            int pos = 0;
            while (pos < s.Length)
            {
                // Get word start
                int start = pos;

                // Get word end
                pos = s.IndexOf(seperationCharacter, pos);
                while (pos > 0 && s[pos - 1] == '\\')
                {
                    pos++;
                    pos = s.IndexOf(seperationCharacter, pos);
                }

                // Adjust for comma not found
                if (pos < 0)
                    pos = s.Length;

                // Extract this word
                yield return s.Substring(start, pos - start);

                // Skip over comma
                if (pos < s.Length)
                    pos++;
            }
        }
        internal static string ToSeparatedList(this IEnumerable<string> st, char seperationCharacter)
        {
            string seperationSt = seperationCharacter.ToString();
            return string.Join(seperationSt, st.Select(s => s.Replace(seperationSt, '\\' + seperationSt)));
        }
    }
}
