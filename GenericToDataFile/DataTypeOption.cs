using System;
using System.Reflection;

namespace GenericToDataString
{
    public class DataTypeOption
    {
        protected DataTypeOption(Type T)
        {
            this.Type = T;
            NullString = string.Empty;
        }
        public Type Type { get; private set; }
        public string StringFormatter { get; set; }
        // internal char Delimiter { get; set; }
        Func<object,string> _objectToString;
        internal virtual Func<object,string> ObjectToString
        {
            get
            {
                if (_objectToString == null)
                {
                    if (string.IsNullOrEmpty(StringFormatter))
                    {
                        _objectToString = ApplyStringConversionOptions(new Func<object, string>(o => o.ToString()));
                    }
                    else
                    {
                        MethodInfo toStringInfo = this.Type.GetMethod("ToString", new Type[] { typeof(string) });
                        object[] param = new object[] { StringFormatter };
                        _objectToString = ApplyStringConversionOptions(new Func<object, string>(o => (string)toStringInfo.Invoke(o, param)));
                    }
                }
                return _objectToString;
            }
            set
            {
                _objectToString = ApplyStringConversionOptions(value);
            }
        }
        public string NullString { get; set; }

        Func<object, string> ApplyStringConversionOptions(Func<object, string> startingFunc)
        {
            return new Func<object, string>(o => (o == null) ? NullString : startingFunc(o));;
        }
    }
    public class DataTypeOption<T> : DataTypeOption
    {
        public DataTypeOption(Func<T, string> stringConversion) : this()
        {
            base.ObjectToString = new Func<object, string>(o => stringConversion.Invoke((T)o));
        }
        public DataTypeOption() : base(typeof(T))
        { }
    }
}
