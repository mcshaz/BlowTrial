using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Helpers
{
    public class IntegerRange
    {
        public IntegerRange(int min, int max)
        {
            if (min>= max)
            {
                throw new ArgumentException("min must be less than max");
            }
            Min = min;
            Max = max;
        }
        public int Min { get; private set; }
        public int Max { get; private set; }
    }
}
