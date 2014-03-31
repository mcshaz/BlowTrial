using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
                new DataTypeOption<string>(s => ToStataString(s)),
                new DataTypeOption<char>(c => ToStataString(c.ToString())),
                new DataTypeOption<char[]>(c => ToStataString(new string(c))),
                new DataTypeOption<bool>(t => t ? "1" : "0"),
                new DataTypeOption<DateTime>((d, atts)=>
                {
                    var dataType = (DataTypeAttribute)atts.FirstOrDefault(a => a.GetType() == typeof(DataTypeAttribute));
                    if (dataType != null)
                    {
                        if (dataType.DataType == DataType.Date)
                        {
                            return TicksToStataDate(d.Ticks);
                        }
                    }
                    return TicksToStataDateTime(d.Ticks);
                }),
                new DataTypeOption<TimeSpan>(ts=>ts.Milliseconds.ToString()), // note these 2 are not tested yet as they 
                new DataTypeOption<DateTimeOffset>(d=>TicksToStataDateTime(d.UtcTicks))); // are never used by myself within data repositories
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
            typeOptions = typeOptions ?? new DataTypeOption[0];
            var propTypeOptions = GetDatabaseTypes();
            foreach (DataTypeOption dto in typeOptions)
            {
                if (propTypeOptions.ContainsKey(dto.PropertyType))
                {
                    propTypeOptions[dto.PropertyType] = dto;
                }
                else
                {
                    propTypeOptions.Add(dto.PropertyType, dto);
                }
            }

            PropertyInfo[] allProps = collectionType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<PropertyDetail> headers = new List<PropertyDetail>(allProps.Length);
            List<Func<object, string>> propertyConverters = new List<Func<object, string>>(allProps.Length);

            foreach (PropertyInfo pi in allProps)
            {
                if (pi.CanRead
                    && !Attribute.IsDefined(pi, typeof(NotMappedAttribute)))
                {
                    Type baseType = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                    DataTypeOption dto;
                    if (propTypeOptions.TryGetValue(baseType, out dto))
                    {
                        var atts = pi.GetCustomAttributes(false);
                        propertyConverters.Add(new Func<object,string>(o => dto.ConversionFunction(pi.GetValue(o, null),atts)));
                        DisplayAttribute attr = (DisplayAttribute)atts.FirstOrDefault(a=>a.GetType() == typeof(DisplayAttribute));
                        headers.Add(new PropertyDetail{ Name = pi.Name, BaseType = baseType, Attributes = atts });
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

        static IDictionary<Type, DataTypeOption> GetDatabaseTypes()
        {
            return (new DataTypeOption[]
            {
                new DataTypeOption<char>(),
                new DataTypeOption<string>(),
                new DataTypeOption<char[]>(c=>new string(c)),
                new DataTypeOption<bool>(),
                new DataTypeOption<byte>(),
                new DataTypeOption<Int16>(),
                new DataTypeOption<Int32>(),
                new DataTypeOption<Int64>(),
                new DataTypeOption<Single>(),
                new DataTypeOption<double>(),
                new DataTypeOption<DateTime>(),
                new DataTypeOption<TimeSpan>(),
                new DataTypeOption<DateTimeOffset>(),
            }).ToDictionary(dto=>dto.PropertyType);
        }

        public static DataTypeOption GetBoolToBinaryConverter()
        {
            return new DataTypeOption<bool>(t => t ? "1" : "0");
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
            ICollection ic = e as ICollection;
            if (ic != null)
            {
                return ic.Count;
            }
            int count = 0;
            var enumerator = e.GetEnumerator();
            while (enumerator.MoveNext()) { count++; }
            return count;
        }
    }
}
