using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.Infrastructure.Randomising
{
    public class RandomAdaptor : Random, IRandom { }
    public interface IRandom
    {
        double NextDouble();
    }
}
