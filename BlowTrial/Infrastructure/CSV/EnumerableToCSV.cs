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
        public static void IListToCSVFile<T>(IList<T> collection, string fileName)
        {
            if (!fileName.EndsWith(".csv"))
            {
                fileName += ".csv";
            }
            System.IO.File.WriteAllLines(fileName, IListToStrings(collection));
        }
        public static string[] IListToStrings<T>(IList<T> collection)
        {
            Func<object, string> stringToString = new Func<object, string>(s => (s == null) ? string.Empty : ((string)s).Replace(",", @"\,"));
            Func<object, string> dateTimeToString = new Func<object, string>(d =>
                {
                    var nullableDT = (DateTime?)d;
                    return nullableDT.HasValue
                        ? nullableDT.Value.ToString("u")
                        : string.Empty;
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
                        headers.Append(',');
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
                returnVar[i] = propertyConverters.Select(pc => pc.Invoke(rowInstance)).ToSeparatedList(',');
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
