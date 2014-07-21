using SpookilySharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrialUnitTests
{
    class EnumListEqualityComparer<T> : IEqualityComparer<IList<T>> where T : struct, IConvertible
    {
        public EnumListEqualityComparer()
        {
            Type tType = typeof(T);
            if (!tType.IsEnum) { throw new ArgumentException("T must be an enumerated type"); }
        }

        public bool Equals(IList<T> x, IList<T> y) 
        {
            if (x.Count != y.Count) { return false; }
            for (int i=0;i<x.Count;i++)
            {
                if (!x[i].Equals(y[i])) { return false; }
            }
            return true;
        }

        public int GetHashCode(IList<T> list) {
            var hasher = new SpookyHash();//use methods with seeds if you need to prevent HashDos
            foreach (var item in list)
            {
                hasher.Update(item.GetHashCode());//or relevant feeds of item, etc.
            }
            return hasher.Final().GetHashCode();
            /*
            int hash = list.Any()?0:9;
            unchecked
            {
                foreach (var o in list)
                {
                int code = o.ToInt32(CultureInfo.InvariantCulture);
                    hash *= 251; // multiply by a prime number
                    hash += code; // add next hash code
                }
            }
            return hash;
            */
        }
    }
}
