using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairDbConsole
{
    static partial class ExtensionMethods
    {
        internal static string EnumerableToCSV<T>(IEnumerable<T> list, string seperator = "\t")
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(seperator, (typeof(T).GetAllReadablePropertyNames())));
            foreach (T item in list)
            {
                sb.AppendLine(string.Join(seperator,item.GetAllPropertyToStrings()));
            }
            return sb.ToString();
        }
    }
}
