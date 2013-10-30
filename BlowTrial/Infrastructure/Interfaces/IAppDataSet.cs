using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Interfaces
{
    public interface IAppDataSet
    {
        DbSet<AppData> AppDataSet { get; set; }
        Database Database { get; }
        int SaveChanges();
     }
}
