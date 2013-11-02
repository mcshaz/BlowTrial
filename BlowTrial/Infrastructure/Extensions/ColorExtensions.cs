using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace BlowTrial.Infrastructure.Extensions
{
    public static class ColorExtensions
    {
        public static int ToInt(this Color colour)
        {
            //return int.Parse(colour.ToString(), System.Globalization.NumberStyles.HexNumber);
            return BitConverter.ToInt32(new byte[] { colour.A, colour.R, colour.G, colour.B }, 0);
        }
        public static Color ToColor(this int colour)
        {
            var b = BitConverter.GetBytes(colour);
            return Color.FromArgb(b[0], b[1], b[2], b[3]);
        }
    }
}
