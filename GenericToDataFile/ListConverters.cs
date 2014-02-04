using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GenericToDataString
{
    public static class ListConverters
    {
        public static string ToCSV<T>(IList<T> collection, char delimiter = ',', params DataTypeOption[] typeOptions)
        {
            return ToCSV<T>(collection, delimiter, (IEnumerable<DataTypeOption>)typeOptions);
        }
        public static string ToCSV<T>(IList<T> collection, char delimiter = ',', IEnumerable<DataTypeOption> typeOptions = null)
        {
            var convertedList = ToStringValues<T>(collection, typeOptions);
            string delimString = delimiter.ToString();
            return string.Join(Environment.NewLine,
                (new string[] { string.Join(delimString, convertedList.PropertiesDetail.Select(prop => prop.Name)) })
                .Concat(convertedList.StringValues.Select(s=>string.Join(delimString,s))));
        }
        const long StataStartDateTime = 618199776000000000;// new DateTime(1960,1,1).Ticks
        public static string ToStataDo<T>(IList<T> collection)
        {
            var convertedList = ToStringValues<T>(collection, new DataTypeOption<string>(s => '"' + s + '"'), 
                new DataTypeOption<bool>(t => t ? "1" : "0"),
                new DataTypeOption<DateTime>(d=>((d.Ticks - StataStartDateTime)/TimeSpan.TicksPerMillisecond).ToString()));
            int rows = convertedList.StringValues.Length;
            StringBuilder sb = new StringBuilder(string.Format("set obs {0}\r\n", rows));
            for (int c=0;c<convertedList.PropertiesDetail.Count;c++)
            {
                string propName = convertedList.PropertiesDetail[c].Name;
                switch (Type.GetTypeCode(convertedList.PropertiesDetail[c].BaseType))
                {
                    case TypeCode.Byte:
                    case TypeCode.Boolean:
                        sb.AppendFormat("generate byte {0} = .\r\n", propName);
                        break;
                    case TypeCode.Char:
                        sb.AppendFormat("generate str1 {0} = \"\"\r\n",propName);
                        break;
                    case TypeCode.String:
                        sb.AppendFormat("generate str{0} {1} = \"\"\r\n", (new int[]{ 1, convertedList.StringValues.Max(r => r[c].Length)}).Max(), propName);
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
                        sb.AppendFormat("generate double {0} = .\r\nformat {0} %tc\r\n",propName);
                        break;
                }
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
        /// <returns> 1st row = property names, subsequent rows = array of strings with conversion functions</returns>
        public static PropertyValues ToStringValues<T>(IList<T> collection, params DataTypeOption[] typeOptions)
        {
            return ToStringValues<T>(collection, (IEnumerable<DataTypeOption>)typeOptions);
        }
        public static PropertyValues ToStringValues<T>(IList<T> collection, IEnumerable<DataTypeOption> typeOptions)
        {
            typeOptions = typeOptions ?? new DataTypeOption[0];
            var propTypeOptions = GetDatabaseTypes();
            foreach (DataTypeOption dto in typeOptions)
            {
                if (propTypeOptions.ContainsKey(dto.Type))
                {
                    propTypeOptions[dto.Type] = dto;
                }
                else
                {
                    propTypeOptions.Add(dto.Type, dto);
                }
            }

            PropertyInfo[] allProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<PropertyDetail> headers = new List<PropertyDetail>(allProps.Length);
            List<Func<object, string>> propertyConverters = new List<Func<object, string>>(allProps.Length);

            foreach (PropertyInfo pi in allProps)
            {
                if (pi.CanRead
                    && !Attribute.IsDefined(pi, typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute)))
                {
                    Type baseType = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                    DataTypeOption dto;
                    if (propTypeOptions.TryGetValue(baseType, out dto))
                    {
                        propertyConverters.Add(new Func<object, string>(o => dto.ObjectToString(pi.GetValue(o, null))));
                        headers.Add(new PropertyDetail{ Name = pi.Name, BaseType = baseType });
                    }
                }
            }

            return new PropertyValues
            {
                PropertiesDetail = headers,
                StringValues = collection.Select(c=>propertyConverters.Select(pc=>pc.Invoke(c)).ToArray()).ToArray()
            };
        }

        public sealed class PropertyDetail
        {
            public string Name { get; internal set; }
            public Type BaseType { get; internal set; }
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
                new DataTypeOption<bool>(),
                new DataTypeOption<byte>(),
                new DataTypeOption<Int16>(),
                new DataTypeOption<Int32>(),
                new DataTypeOption<Int64>(),
                new DataTypeOption<Single>(),
                new DataTypeOption<double>(),
                new DataTypeOption<DateTime>(),
                new DataTypeOption<TimeSpan>(),
                new DataTypeOption<DateTimeOffset>()
            }).ToDictionary(dto=>dto.Type);
        }
        public static DataTypeOption GetBoolToBinaryConverter()
        {
            return new DataTypeOption<bool>(t => t ? "1" : "0");
        }
    }
}
