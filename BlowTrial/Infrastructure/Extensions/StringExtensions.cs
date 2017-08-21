using System;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;

namespace BlowTrial.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string GetSafeFilename(string filename, string delimiter = "_")
        {
            return string.Join(delimiter, filename.Split(System.IO.Path.GetInvalidFileNameChars()));
        }
        public static string ToSeparatedWords(this string value)
        {
            if (value != null)
            {
                return Regex.Replace(value, "([A-Z][a-z]?)", " $1").Trim();
            }
            return null;
        }
        public static string ToBriefString(this string value) // expression trees cannot handle default values - do it manually
        {
            return ToBriefString(value, 15);
        }
        public static string ToBriefString(this string value, int strLength)
        {
            return ToBriefString(value, strLength, "...");
        }
        public static string ToBriefString(this string value, int strLength, string truncateWith)
        {
            if (value == null) { return null; }
            if (value.Length < strLength) {return value;}
            strLength -= truncateWith.Length;
            return (value.Substring(0, strLength) + truncateWith);
        }
        public static string ToLabelFormat(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) { throw new ArgumentException("String cannot be null or whitespace"); }
            return char.ToUpper(value[0]) + value.Substring(1) + ':';
        }

        public static string InsertDateStampToFileName(this string fileName)
        {
            const string TimeStampFormat = "yyyyMMddhhmm";
            int dotpos = fileName.LastIndexOf('.');
            if (dotpos == -1) { return fileName + DateTime.Now.ToString(TimeStampFormat); }
            return fileName.Substring(0, dotpos) + DateTime.Now.ToString(TimeStampFormat) + fileName.Substring(dotpos);
        }
        /// <summary>
        ///convert text to Pascal Case
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertToPascalCase(string str)
        {
            //if nothing is proivided throw a null argument exception
            if (str == null) throw new ArgumentNullException("str", "Null text cannot be converted!");
            str = str.Trim();
            if (str.Length == 0) return str;

            //split the provided string into an array of words
            string[] words = str.Split(' ');

            //loop through each word in the array
            for (int i = 0; i < words.Length; i++)
            {
                //if the current word is greater than 1 character long
                if (words[i].Length > 0)
                {
                    //grab the current word
                    string word = words[i];

                    //convert the first letter in the word to uppercase
                    char firstLetter = char.ToUpper(word[0]);

                    //concantenate the uppercase letter to the rest of the word
                    words[i] = firstLetter + word.Substring(1);
                }
            }

            //return the converted text
            return string.Join(string.Empty, words);
        }
        /*
        public static string Pluralise(this string value, int count)
        {
            if (count == 1)
            {
                return value;
            }
            return PluralizationService
                .CreateService(new CultureInfo("en-AU"))
                .Pluralize(value);
        }
        public static string Singularise(this string value)
        {
            return PluralizationService
                .CreateService(new CultureInfo("en-AU"))
                .Singularize(value);
        }
        */
        public static string Replace(this string source, string oldString, string newString, StringComparison comp)
        {
            int index = source.IndexOf(oldString, comp);

            // Determine if we found a match
            bool MatchFound = index >= 0;

            if (MatchFound)
            {
                // Remove the old text
                source = source.Remove(index, oldString.Length);

                // Add the replacemenet text
                source = source.Insert(index, newString);
            }

            return source;
        }

        public static DateTime? ToDateTime(this string val)
        {
            if (DateTime.TryParse(val, out DateTime temp)) { return temp; }
            else { return null; }
        }

        public static int? ToInt(this string val)
        {
            if (int.TryParse(val, out int temp)) { return temp; }
            else { return null; }
        }
    }
}