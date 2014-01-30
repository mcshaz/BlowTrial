using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BlowTrial.Infrastructure.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlowTrial.Infrastructure
{
    static class CSVconversion
    {
        private static readonly string[] extensions = new string[] { ".txt", ".csv", ".tab", ".tsv" };
        public static void IListToCSVFile<T>(IList<T> collection, string fileName, char delimiter, string dateFormat = "u", bool encloseStringInQuotes = false, bool encloseDateInQuotes=false)
        {
            if (!extensions.Any(e=>fileName.EndsWith(e)))
            {
                fileName += extensions.First();
            }
            System.IO.File.WriteAllLines(fileName, IListToStrings(collection, delimiter));
        }
        public static string[] IListToStrings<T>(IList<T> collection, char delimiter = ',', string dateFormat = "u", bool encloseStringInQuotes = false, bool encloseDateInQuotes = false)
        {
            Func<string, string> stringInQuotes = new Func<string, string>(s => '"' + s + '"');
            string lookfor = delimiter.ToString();
            string replacewith = '\\' + lookfor;
            Func<string, string> escapedString = new Func<string,string>(s => s.Replace(lookfor, replacewith));

            Func<object, string> stringToString = (encloseStringInQuotes)
                ? new Func<object, string>(s=>stringInQuotes((string)s))
                : new Func<object, string>(s=>escapedString((string)s ?? string.Empty));
            Func<object, string> dateTimeToString = new Func<object, string>(d =>
                    {
                        var nullableDT = (DateTime?)d;
                        string dateTxt = nullableDT.HasValue
                            ? nullableDT.Value.ToString(dateFormat)
                            : string.Empty;
                        return encloseDateInQuotes
                            ? stringInQuotes(dateTxt)
                            : escapedString(dateTxt);
                    });

            Func<object, string> boolToString = new Func<object, string>(b =>
            {
                if (b == null) { return string.Empty; }
                return b.Equals(true) ? "1" : "0";
            });
            Func<object, string> objectToString = new Func<object, string>(o => (o == null) ? string.Empty : o.ToString());

            StringBuilder headers = new StringBuilder();

            List<Func<object, string>> propertyConverters = new List<Func<object, string>>();
            bool isSubsequentProp = false;
            Type type = typeof(T);
            Type[] myDbTypes = MyDatabaseTypes();
            foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pi.CanRead
                    && !Attribute.IsDefined(pi, typeof(NotMappedAttribute))
                    && myDbTypes.Contains(pi.PropertyType))
                {
                    if (isSubsequentProp)
                    {
                        headers.Append(delimiter);
                    }
                    else
                    {
                        isSubsequentProp = true;
                    }

                    Func<object, string> converter;
                    if (pi.PropertyType == typeof(DateTime?) || pi.PropertyType == typeof(DateTime))
                    {
                        converter = dateTimeToString;
                    }
                    else if (pi.PropertyType == typeof(string))
                    {
                        converter = stringToString;
                    }
                    else if (pi.PropertyType == typeof(bool) || pi.PropertyType==typeof(bool?))
                    {
                        converter = boolToString;
                    }
                    else
                    {
                        converter = objectToString;
                    }
                    propertyConverters.Add(new Func<object, string>(o => converter.Invoke(pi.GetValue(o, null))));
                    headers.Append(pi.Name);

                }
            }
            string[] returnVar = new String[collection.Count + 1];
            returnVar[0] = headers.ToString();

            for (int i = 1; i < returnVar.Length; i++)
            {
                var rowInstance = collection[i - 1];
                returnVar[i] = propertyConverters.Select(pc => pc.Invoke(rowInstance)).ToSeparatedList(delimiter);
            }
            return returnVar;
        }
        public static Type[] MyDatabaseTypes()
        {
            return new Type[]
            {
                typeof(string),
                typeof(int),
                typeof(double),
                typeof(DateTime),
                typeof(bool),
                typeof(int?),
                typeof(double?),
                typeof(DateTime?),
                typeof(bool?)
            };
        }
    }
}
