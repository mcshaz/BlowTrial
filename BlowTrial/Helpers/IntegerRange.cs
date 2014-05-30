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
            Min = min;
            Max = max;
        }
        public IntegerRange() { }
        int? _min;
        int? _max;
        public int Min 
        { 
            get 
            { 
                return _min.Value; 
            }
            set
            {
                if (value>=_max)
                {
                    throw new ArgumentException("min must be less than max");
                }
                _min = value;
            }
        }
        public int Max 
        { 
            get 
            { 
                return _max.Value; 
            }
            set 
            {
                if (value <= _min)
                {
                    throw new ArgumentException("min must be less than max");
                }
                _max = value;
            }
        }
    }
}
