using BlowTrial.Infrastructure.Exceptions;
using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.Infrastructure.Extensions
{
    internal static class IQueryableExtensions
    {
        internal static int GetNextId(this IQueryable<ISharedRecord> recordSet, int studyCentreId, int maxIdForSite)
        {
            int returnVar = (from r in recordSet
                             where r.Id >= studyCentreId && r.Id <= maxIdForSite
                             select r.Id).DefaultIfEmpty().Max();
            if (returnVar == 0) { returnVar = studyCentreId; }
            returnVar++;
            if (returnVar > maxIdForSite)
            {
                throw new DataKeyOutOfRangeException("Database has exceeded maximum size for site");
            }
            return returnVar;
        }
    }
}
