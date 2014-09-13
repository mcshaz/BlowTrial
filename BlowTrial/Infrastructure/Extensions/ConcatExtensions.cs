using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.Infrastructure.Extensions
{
    public static class ConcatExtensions
    {
        public static IEnumerable<T> ConcatItems<T>(this IEnumerable<T> first,params T[] secondItems)
        {
            return first.Concat(secondItems);
        }
    }
}
