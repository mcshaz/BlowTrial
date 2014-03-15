using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.Infrastructure.Extensions
{
    public static class GestAgeExtensions
    {
        public static string ToCgaString(this double gestAge, int ageDays)
        {
            int cga = (int)(gestAge * 7) + ageDays;
            return (cga / 7).ToString() + '.' + (cga % 7).ToString();
        }
    }
}
