using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GenericToDataString
{
    //http://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
    public class ObjectDumper
    {
        private int _currentIndent;
        private readonly int _indentSize;
        private readonly StringBuilder _stringBuilder;
        private readonly Dictionary<object,int> _hashListOfFoundElements;
        private readonly char _indentChar;
        private readonly int _depth;
        private int _currentLine;

        private ObjectDumper(int depth, int indentSize, char indentChar)
        {
            _depth = depth;
            _indentSize = indentSize;
            _indentChar = indentChar;
            _stringBuilder = new StringBuilder();
            _hashListOfFoundElements = new Dictionary<object,int>();
        }

        public static string Dump(object element, int depth = 4,int indentSize=2,char indentChar=' ')
        {
            var instance = new ObjectDumper(depth, indentSize, indentChar);
            return instance.DumpElement(element);
        }

        private string DumpElement(object element)
        {
            if (_currentIndent > _depth) { return null; }
            if (element == null || element is ValueType || element is string)
            {
                Write(FormatValue(element));
            }
            else
            {
                var enumerableElement = element as IEnumerable;
                if (enumerableElement != null)
                {
                    foreach (object item in enumerableElement)
                    {
                        if (item is IEnumerable && !(item is string))
                        {
                            _currentIndent++;
                            DumpElement(item);
                            _currentIndent--;
                        }
                        else
                        {
                            DumpElement(item);                        
                        }
                    }
                }
                else
                {
                    Type objectType = element.GetType();
                    Write("{{{0}(HashCode:{1})}}", objectType.FullName,element.GetHashCode());
                    if (!AlreadyDumped(element))
                    {
                        _currentIndent++;
                        MemberInfo[] members = objectType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var memberInfo in members)
                        {
                            var fieldInfo = memberInfo as FieldInfo;
                            var propertyInfo = memberInfo as PropertyInfo;

                            if (fieldInfo == null && (propertyInfo == null || !propertyInfo.CanRead || propertyInfo.GetIndexParameters().Length > 0))
                                continue;

                            var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
                            object value;
                            try
                            {
                                value = fieldInfo != null
                                                   ? fieldInfo.GetValue(element)
                                                   : propertyInfo.GetValue(element, null);
                            }
                            catch(Exception e)
                            {
                                Write("{0} failed with:{1}",memberInfo.Name,(e.GetBaseException() ?? e).Message);
                                continue;
                            }

                            if (type.IsValueType || type == typeof(string))
                            {
                                Write("{0}: {1}", memberInfo.Name, FormatValue(value));
                            }
                            else
                            {
                                var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
                                Write("{0}: {1}", memberInfo.Name, isEnumerable ? "..." : "{ }");

                                _currentIndent++;
                                DumpElement(value);
                                _currentIndent--;
                            }
                        }
                        _currentIndent--;
                    }
                }
            }

            return _stringBuilder.ToString();
        }

        private bool AlreadyDumped(object value)
        {
            if (value == null)
                return false;
            int lineNo;
            if (_hashListOfFoundElements.TryGetValue(value, out lineNo))
            {
                Write("(reference already dumped - line:{0})", lineNo);
                return true;
            }
            _hashListOfFoundElements.Add(value, _currentLine);
            return false;
        }

        private void Write(string value, params object[] args)
        {
            var space = new string(_indentChar, _currentIndent * _indentSize);

            if (args != null)
                value = string.Format(value, args);

            _stringBuilder.AppendLine(space + value);
            _currentLine++;
        }

        private string FormatValue(object o)
        {
            if (o == null)
                return ("null");

            if (o is DateTime)
                return (((DateTime)o).ToShortDateString());

            if (o is string)
                return string.Format("\"{0}\"", o);

            if (o is char && (char)o == '\0') 
                return "\"\""; 

            if (o is ValueType)
                return (o.ToString());

            if (o is IEnumerable)
                return ("...");

            return ("{ }");
        }
    }
}
