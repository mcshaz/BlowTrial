using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Interfaces
{
    public interface IBackupData
    {
        DbSet<BackupData> BackupDataSet { get; }
        DbSet<CloudDirectory> CloudDirectories { get; }
        Database Database { get; }
        int SaveChanges();
     }
}
