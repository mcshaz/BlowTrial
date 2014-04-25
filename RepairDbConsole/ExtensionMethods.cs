using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RepairDbConsole
{
    static partial class ExtensionMethods
    {
        static Type[] MyBasicTypes = new[] { typeof(string), typeof(int), typeof(DateTime), typeof(int?), typeof(DateTime?) };
        internal static IEnumerable<string> GetAllPropertyToStrings(this object obj)
        {
            PropertyInfo[] pis = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<string> returnVar = new List<string>(pis.Length);
            foreach (PropertyInfo pi in pis)
            {
                if (pi.CanRead)
                {
                    object v = MyBasicTypes.Contains(pi.PropertyType)?pi.GetValue(obj):"Complex Type";
                    returnVar.Add((v==null)?string.Empty:v.ToString());
                }
            }
            return returnVar;
        }

        internal static IEnumerable<string> GetAllReadablePropertyNames(this Type T)
        {

            PropertyInfo[] pis = T.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<string> returnVar = new List<string>(pis.Length);
            foreach (PropertyInfo pi in pis)
            {
                if (pi.CanRead)
                {
                    returnVar.Add(pi.Name);
                }
            }
            return returnVar;
        }
        public static T[] SubArray<T>(this T[] data, int index, int endIndex)
        {
            if (endIndex >= data.Length)
            {
                endIndex = data.Length - 1;
            }
            int length = endIndex - index + 1;
            if (length < 0)
            {
                length = 0;
            }
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
