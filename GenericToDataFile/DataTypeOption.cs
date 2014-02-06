using System;
using System.Reflection;

namespace GenericToDataString
{
    public class DataTypeOption
    {
        public const string DefaultNull = "";
        protected DataTypeOption(Type T, string nullString = DefaultNull)
            :this(T, new Func<object, string>(o=>o.ToString()), nullString)
        {
        }
        protected DataTypeOption(Type T, Func<object, string> nonNullObjectToString, string nullString = DefaultNull)
        {
            PropertyType = T;
            ObjectToString = new Func<object, string>(o => (o == null) ? nullString : nonNullObjectToString(o));
        }
        public Type PropertyType { get; private set; }
        internal Func<object, string> ObjectToString { get; private set; }
        public string Convert(object o)
        {
            return ObjectToString(o);
        }
    }
    public class DataTypeOption<T> : DataTypeOption
    {
        public DataTypeOption(Func<T, string> stringConversion, string nullString = DataTypeOption.DefaultNull) 
            : base(typeof(T),new Func<object, string>(o => stringConversion((T)o)),nullString)
        {
        }
        public DataTypeOption(string nullString = DataTypeOption.DefaultNull)
            : base(typeof(T), nullString)
        { }
    }
}
