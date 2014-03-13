using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Interfaces
{
    public interface IAppData
    {
        DbSet<BackupData> BackupDataSet { get; }
        DbSet<CloudDirectory> CloudDirectories { get; }
        DbSet<RandomisingMessage> RandomisingMessages { get; }
        Database Database { get; }
        //DbEntityEntry<BackupData> Entry<BackupData>(object entity);
        
        int SaveChanges();
     }
}
