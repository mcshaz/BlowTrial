using System;
using System.Reflection;

namespace GenericToDataString
{
    public class DataTypeOption
    {
        public const string DefaultNull = "";
        protected DataTypeOption(Type T, string nullString = DefaultNull)
            :this(
                T, 
                new Func<object, object[],string>((o,a)=>o.ToString()),
                nullString)
        {}

        protected DataTypeOption(Type T, Func<object, string> nonNullObjectToString, string nullString = DefaultNull)
            :this(
                T,
                new Func<object, object[],string>((o,a) => nonNullObjectToString(o)),
                nullString)
        {}

        protected DataTypeOption(Type T, Func<object, object[], string> nonNullObjectToString, string nullString = DefaultNull)
        {
            PropertyType = T;
            _defaultNull = DefaultNull;
            ConversionFunction = new Func<object, object[] ,string>((o, a) => (o == null) ? _defaultNull : nonNullObjectToString(o, a));
        }

        public Type PropertyType { get; private set; }
        string _defaultNull;
        internal Func<object, object[], string> ConversionFunction { get; private set; }
    }
    public class DataTypeOption<T> : DataTypeOption
    {
        public DataTypeOption(string nullString = DataTypeOption.DefaultNull)
            : base(typeof(T), nullString)
        { }
        public DataTypeOption(Func<T, string> nonNullObjectToString, string nullString = DataTypeOption.DefaultNull)
            : base(typeof(T), new Func<object,string>(o=>nonNullObjectToString((T)o)),nullString)
        { }
        public DataTypeOption(Func<T, object[],string> nonNullObjectToString, string nullString = DataTypeOption.DefaultNull)
            : base(typeof(T), new Func<object, object[],string>((o,a) => nonNullObjectToString((T)o,a)), nullString)
        {
        }
    }
}
