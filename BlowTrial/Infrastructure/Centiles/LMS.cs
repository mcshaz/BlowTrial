using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Centiles
{
    public class LMS
    {
        public double L { get; set; }
        public double M { get; set; }
        public double S { get; set; }
        /// <summary>
        /// linear interpolation between 2 LMS values
        /// </summary>
        /// <param name="with"></param>
        /// <param name="fraction"> proportion of linear interpolation applied to With</param>
        /// <returns></returns>
        public LMS LinearInterpolate(LMS with, double fraction)
        {
            if (fraction <0 || fraction >1)
            {
                throw new ArgumentOutOfRangeException("fraction",fraction,"must be between 0 and 1");
            }
            double oppFraction = 1 - fraction;
            return new LMS
            {
                L = oppFraction * this.L + fraction * with.L,
                M = oppFraction * this.M + fraction * with.M,
                S = oppFraction * this.S + fraction * with.S
            };
        }

        public double ZfromParams(double param)
        {
            return Stats.ZfromParams(param, M, S, L);
        }

        public double CumSnormfromParams(double param)
        {
            return Stats.CumSnorm(ZfromParams(param));
        }
    }
}
