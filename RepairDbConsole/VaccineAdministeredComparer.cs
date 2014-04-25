using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairDbConsole
{
    public class VaccineAdministeredComparer : IEqualityComparer<VaccineAdministered> 
    {
        // Summary:
        //     Determines whether the specified objects are equal.
        //
        // Parameters:
        //   x:
        //     The first object of type T to compare.
        //
        //   y:
        //     The second object of type T to compare.
        //
        // Returns:
        //     true if the specified objects are equal; otherwise, false.
        public bool Equals(VaccineAdministered x, VaccineAdministered y)
        {
            return x.VaccineId == y.VaccineId && x.AdministeredAt == y.AdministeredAt;
        }
        //
        // Summary:
        //     Returns a hash code for the specified object.
        //
        // Parameters:
        //   obj:
        //     The System.Object for which a hash code is to be returned.
        //
        // Returns:
        //     A hash code for the specified object.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The type of obj is a reference type and obj is null.
        public int GetHashCode(VaccineAdministered obj)
        {
            return obj.VaccineId.GetHashCode() + obj.AdministeredAt.GetHashCode();
        }
    }
}
