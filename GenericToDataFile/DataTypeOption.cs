using System;
using System.IO;
using System.Reflection;

namespace GenericToDataString
{
    public class DataTypeOption
    {
        public const string emptyString = "";
        public DataTypeOption(Type T, string nullString = emptyString)
            :this(
                T,
                new Action<object, object[], TextWriter>((o, a, t) => t.Write(o)),
                nullString)
        {}

        /*
        public DataTypeOption(Type T, Action<object, TextWriter> nonNullObjectToString, string nullString = emptyString)
            :this(
                T,
                new Action<object, object[], TextWriter>((o, a, t) => nonNullObjectToString(o, t)),
                nullString)
        {}
         * */

        public DataTypeOption(Type T, Action<object, object[], TextWriter> nonNullObjectToString, string nullString = emptyString)
        {
            PropertyType = T;
            DefaultNull = nullString;
            ConversionFunction = nonNullObjectToString;
        } 

        public readonly Type PropertyType;
        public readonly string DefaultNull;
        public readonly Action<object, object[], TextWriter> ConversionFunction;
        private void Convert(object o, object[] attr, TextWriter writer)
        {
            if (o.Equals(null))
            {
                writer.Write(DefaultNull);
            }
            else
            {
                ConversionFunction(o, attr, writer);
            }
        }
    }
}
