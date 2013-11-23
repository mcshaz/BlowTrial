using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Interfaces
{
    public interface ITrialDataContext : IDisposable
    {
        DbSet<Vaccine> Vaccines { get; set; }
        DbSet<VaccineAdministered> VaccinesAdministered { get; set; }
        DbSet<Participant> Participants { get; set; }
        DbSet<ScreenedPatient> ScreenedPatients { get; set; }
        DbSet<ProtocolViolation> ProtocolViolations { get; set; }
        DbSet<StudyCentre> StudyCentres { get; set; }
        Database Database { get; }

        DbEntityEntry Entry(object entity);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        /// <summary>
        /// Backs up the db and returns the file path to the newly created backup
        /// </summary>
        /// <returns></returns>
        string BackupDb();
        string DbName { get; }
        DateTime DbLastModifiedUtc();
        ITrialDataContext AttachDb(string backupFilePath);
        int SaveChanges();
    }
}
