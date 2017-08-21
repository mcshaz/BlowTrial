using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace GenericToDataString
{
    public static class ListConverters
    {
        public static string ToCSV<T>(IEnumerable<T> collection, char delimiter = ',', params DataTypeOption[] typeOptions)
        {
            return ToCSV(typeof(T), collection, typeOptions, delimiter);
        }
        public static string ToCSV(IEnumerable collection, char delimiter = ',', params DataTypeOption[] typeOptions)
        {
            return ToCSV(GetGenericIEnumerable(collection), collection, typeOptions, delimiter);
        }
        static string ToCSV(Type collectionType, IEnumerable collection, DataTypeOption[] typeOptions, char delimiter = ',')
        {
            var convertedList = ToStringValues(collectionType, collection, typeOptions);
            string delimString = delimiter.ToString();
            return string.Join(Environment.NewLine,
                (new string[] { string.Join(delimString, convertedList.PropertiesDetail.Select(prop => prop.Name)) })
                .Concat(convertedList.StringValues.Select(s=>string.Join(delimString,s))));
        }
        public static string StataSafeVarname(string varname, string unsafeReplacement = "_")
        {
            return Regex.Replace(varname, @"\W", unsafeReplacement);
        }
        const long StataStartDateTime = 618199776000000000;// new DateTime(1960,1,1).Ticks
        static string TicksToStataDateTime(long ticks)
        {
            return ((ticks - StataStartDateTime) / TimeSpan.TicksPerMillisecond).ToString();
        }
        static string TicksToStataDate(long ticks)
        {
            return ((ticks - StataStartDateTime) / TimeSpan.TicksPerDay).ToString();
        }
        const int StataStringChars = 4;
        static string ToStataString(string s)
        {
            return "`\"" + s + "\"'";
        }
        public static string ToStataDo<T>(IEnumerable<T> collection)
        {
            return ToStataDo(typeof(T), collection);
        }
        public static string ToStataDo(IEnumerable collection)
        {
            return ToStataDo(GetGenericIEnumerable(collection), collection);
        }
        static string ToStataDo(Type collectionType, IEnumerable collection)
        {
            var convertedList = ToStringValues(collectionType, collection, 
                new DataTypeOption(typeof(string),(s,attr) => ToStataString((string)s)),
                new DataTypeOption(typeof(char),(c,attr) => ToStataString(c.ToString())),
                new DataTypeOption(typeof(char[]),(c,attr) => ToStataString(new string((char[])c))),
                new DataTypeOption(typeof(DateTime),(d, atts)=>
                {
                    var dataType = (DataTypeAttribute)atts.FirstOrDefault(a => a.GetType() == typeof(DataTypeAttribute));
                    if (dataType != null)
                    {
                        if (dataType.DataType == DataType.Date)
                        {
                            return TicksToStataDate(((DateTime)d).Ticks);
                        }
                    }
                    return TicksToStataDateTime(((DateTime)d).Ticks);
                }),
                new DataTypeOption(typeof(TimeSpan),(ts,attr)=>((TimeSpan)ts).Milliseconds.ToString()), // note these 2 are not tested yet as they 
                new DataTypeOption(typeof(DateTimeOffset), (d, attr) => TicksToStataDateTime(((DateTimeOffset)d).UtcTicks))); // are never used by myself within data repositories
            int rows = convertedList.StringValues.Length;
            StringBuilder sb = new StringBuilder(string.Format("set obs {0}\r\n", rows));
            for (int c=0;c<convertedList.PropertiesDetail.Count;c++)
            {
                string propName = convertedList.PropertiesDetail[c].Name;
                Type baseType = convertedList.PropertiesDetail[c].BaseType;
                TypeCode propType = Type.GetTypeCode(baseType);
                if (propType == TypeCode.Object)
                {
                    if (baseType == typeof(TimeSpan))
                    {
                        propType = TypeCode.Double;
                    }
                    else if (baseType == typeof(DateTimeOffset))
                    {
                        propType = TypeCode.DateTime;
                    }
                    else if (baseType == typeof(char[]))
                    {
                        propType = TypeCode.String;
                    }
                }
                switch (propType)
                {
                    case TypeCode.Byte:
                    case TypeCode.Boolean:
                        sb.AppendFormat("generate byte {0} = .\r\n", propName);
                        break;
                    case TypeCode.Char:
                        sb.AppendFormat("generate str1 {0} = \"\"\r\n",propName);
                        break;
                    case TypeCode.String:
                        sb.AppendFormat("generate str{0} {1} = \"\"\r\n", 
                            (new int[]{ 1, convertedList.StringValues.Max(r => r[c].Length) - StataStringChars}).Max(), propName);
                        break;
                    case TypeCode.Int16:
                        sb.AppendFormat("generate int {0} = .\r\n",propName);
                        break;
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                        sb.AppendFormat("generate long {0} = .\r\n",propName);
                        break;
                    case TypeCode.Single:
                        sb.AppendFormat("generate float {0} = .\r\n",propName);
                        break;
                    case TypeCode.Double:
                        sb.AppendFormat("generate double {0} = .\r\n",propName);
                        break;
                    case TypeCode.DateTime:
                        DataTypeAttribute dataType = (DataTypeAttribute)convertedList.PropertiesDetail[c].Attributes.FirstOrDefault(a => a.GetType() == typeof(DataTypeAttribute));
                        if (dataType != null && dataType.DataType == DataType.Date)
                        {
                            sb.AppendFormat("generate long {0} = .\r\nformat {0} %td\r\n", propName);
                        }
                        else
                        {
                            sb.AppendFormat("generate double {0} = .\r\nformat {0} %tc\r\n",propName);
                        }
                        break;
                }
                DisplayAttribute display = (DisplayAttribute)convertedList.PropertiesDetail[c].Attributes.FirstOrDefault(a => a.GetType() == typeof(DisplayAttribute));

                sb.AppendFormat("label variable {0} \"{1}\"\r\n",propName,
                    (display==null || string.IsNullOrEmpty(display.Name)) ? propName.ToSeparatedWords() : display.Name);
                for (int r=0;r<rows;r++)
                {
                    string nextVal = convertedList.StringValues[r][c];
                    if (nextVal != string.Empty)
                    {
                        sb.AppendFormat("replace {0} = {1} in {2}\r\n", propName, nextVal, r+1);
                    }
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="delimiter"></param>
        /// <param name="typeOptions"></param>
        /// <returns></returns>
        public static PropertyValues ToStringValues<T>(IEnumerable<T> collection, params DataTypeOption[] typeOptions)
        {
            return ToStringValues(typeof(T), collection, typeOptions);
        }
        static PropertyValues ToStringValues(Type collectionType, IEnumerable collection, params DataTypeOption[] typeOptions)
        {
            var dataTypeSet = GetDatabaseTypes();
            var customTypeDictionary = typeOptions.ToDictionary(o => o.PropertyType);
            foreach (var dt in BaseTransforms())
            {
                if (!customTypeDictionary.ContainsKey(dt.PropertyType))
                {
                    customTypeDictionary.Add(dt.PropertyType, dt);
                }
            }
            List<DataTypeOption> nullables = new List<DataTypeOption>(customTypeDictionary.Count);
            foreach (var kv in customTypeDictionary)
            {
                if (kv.Key.IsValueType && !kv.Key.IsGenericType)
                {
                    Type nullable = typeof(Nullable<>).MakeGenericType(kv.Key);

                    if (!customTypeDictionary.ContainsKey(nullable))
                    {
                        nullables.Add(new DataTypeOption(nullable, (o, attr) => o == null ? "" : kv.Value.GetString(o, attr)));
                    }
                }
            }
            foreach (var n in nullables)
            {
                customTypeDictionary.Add(n.PropertyType, n);
            }

            PropertyInfo[] allProps = collectionType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<PropertyDetail> headers = new List<PropertyDetail>(allProps.Length);
            List<Func<object,string>> propertyConverters = new List<Func<object,string>>(allProps.Length);
            foreach (PropertyInfo pi in allProps)
            {
                if (pi.GetIndexParameters().Length > 0) { continue; }
                if (pi.CanRead
                    && !Attribute.IsDefined(pi, typeof(NotMappedAttribute)))
                {
                    if (customTypeDictionary.TryGetValue(pi.PropertyType, out DataTypeOption dto) || dataTypeSet.Contains(pi.PropertyType) || pi.PropertyType.IsEnum)
                    {
                        object[] atts = pi.GetCustomAttributes(false);
                        if (dto == null)
                        {
                            if (pi.PropertyType.IsEnum)
                            {
                                propertyConverters.Add(new Func<object, string>(o =>
                                {
                                    var convert = pi.GetValue(o, null);
                                    if (convert == null) { return ""; }
                                    return Convert.ToInt32(convert).ToString();
                                }));
                            }
                            else if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                propertyConverters.Add(new Func<object, string>(o =>
                                {
                                    object val = pi.GetValue(o, null);
                                    if (val == null) { return ""; }
                                    return val.ToString();
                                }));
                            }
                            else
                            {
                                propertyConverters.Add(new Func<object, string>(o => pi.GetValue(o, null).ToString()));
                            }
                        }
                        else
                        {
                            propertyConverters.Add(new Func<object, string>(o => dto.GetString(pi.GetValue(o, null), atts)));
                        }
                        DisplayAttribute attr = (DisplayAttribute)atts.FirstOrDefault(a => a.GetType() == typeof(DisplayAttribute));
                        headers.Add(new PropertyDetail { Name = pi.Name, BaseType = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType, Attributes = atts });
                    }
                }
            }
            string[][] stringVals = new string[GetCount(collection)][];
            int row = 0;
            foreach(object o in collection)
            {
                string[] currentRow = stringVals[row++] = new string[headers.Count];
                for (int col =0;col<headers.Count;col++)
                {
                    currentRow[col] = propertyConverters[col](o);
                }
            }
            return new PropertyValues
            {
                PropertiesDetail = headers,
                StringValues = stringVals // collection.Select(c=>propertyConverters.Select(pc=>pc.Invoke(c)).ToArray()).ToArray()
            };
        }

        public sealed class PropertyDetail
        {
            public string Name { get; internal set; }
            public Type BaseType { get; internal set; }
            public object[] Attributes { get; internal set; }
        }

        public sealed class PropertyValues
        {
            public IList<PropertyDetail> PropertiesDetail { get; internal set; }
            /// <summary>
            /// Values of converted string matrix in [row][column] format
            /// </summary>
            public string[][] StringValues { get; internal set; }
        }

        static HashSet<Type> GetDatabaseTypes()
        {
            return new HashSet<Type>(
                new Type[]{
                    typeof(char),
                    typeof(string),
                    typeof(char[]),
                    typeof(bool),
                    typeof(byte),
                    typeof(Int16),
                    typeof(Int32),
                    typeof(Int64),
                    typeof(Single),
                    typeof(double),
                    typeof(DateTime),
                    typeof(TimeSpan),
                    typeof(DateTimeOffset),
                    typeof(char?),
                    typeof(bool?),
                    typeof(byte?),
                    typeof(Int16?),
                    typeof(Int32?),
                    typeof(Int64?),
                    typeof(Single?),
                    typeof(double?),
                    typeof(DateTime?),
                    typeof(TimeSpan?),
                    typeof(DateTimeOffset?)
                });
        }

        static DataTypeOption[] BaseTransforms()
        {
            return new DataTypeOption[]
            {
                new DataTypeOption(typeof(string), new Func<object, object[],string>((o,a)=>o==null?"":(string)o)),
                new DataTypeOption(typeof(bool), new Func<object, object[],string>((o,a)=>o.Equals(true)?"1":"0")),
                new DataTypeOption(typeof(bool?), new Func<object, object[],string>((o,a)=>o==null
                    ?""
                    :o.Equals(true)?"1":"0"))
            };
        }
        //stackoverflow.com/questions/906499/getting-type-t-from-ienumerablet#906538
        public static Type GetGenericIEnumerable(IEnumerable e)
        {
            Type T = e.GetType()
                    .GetInterfaces()
                    .FirstOrDefault(t => t.IsGenericType == true
                        && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return T == null
                ? null
                : T.GetGenericArguments()[0];
        }

        public static int GetCount(IEnumerable e)
        {
            if (e is ICollection ic)
            {
                return ic.Count;
            }
            int count = 0;
            var enumerator = e.GetEnumerator();
            while (enumerator.MoveNext()) { count++; }
            return count;
        }
    }
    public class DataTypeOption
    {
        public DataTypeOption(Type propertyType, Func<object, object[], string> getString)
        {
            PropertyType = propertyType;
            GetString = getString;
        }
        public readonly Type PropertyType;
        public readonly Func<object, object[], string> GetString;
    }
}
