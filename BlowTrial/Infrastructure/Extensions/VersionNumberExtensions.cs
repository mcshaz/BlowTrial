using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.Infrastructure.Extensions
{
    public static class VersionNumberExtensions
    {
        public static Version GetVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }
        public static int GetVersionInt()
        {
            return GetVersion().ToInt();
        }
        public static int ToInt(this Version version)
        {
            return BitConverter.ToInt32(new byte[] { (byte)version.Major, (byte)version.Minor, 0, 0 }, 0);
        }
        public static string ToVersionString(this int version)
        {
            return string.Join(".", BitConverter.GetBytes(version));
        }
    }
}
