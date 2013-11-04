using BlowTrial.Domain.Interfaces;
using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServerCompact;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Providers
{
    [DbConfigurationType(typeof(ContextCeConfiguration))]
    public class TrialDataContext : DbContext, ITrialDataContext
    {
        const string ParticipantDbName = "ParticipantData";
        const string ParticipantDb = ParticipantDbName + ".sdf";
        public static string GetDbPath()
        {
            return ContextCeConfiguration.GetSqlCePath(ParticipantDb);
        }
        const string DbPassword = "ABC";

        static public string GetConnectionString(string dbPath)
        {
            return (new SqlCeConnectionStringBuilder
            {
                  Password = DbPassword,
                  DataSource = dbPath
            }).ToString();
        }

        public TrialDataContext() : this(GetDbPath()) { }
        public TrialDataContext(string dbPath) : base(GetConnectionString(dbPath)) { }
        public DbSet<ProtocolViolation> ProtocolViolations { get; set; }
        public DbSet<Vaccine> Vaccines { get; set; }
        public DbSet<VaccineAdministered> VaccinesAdministered { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<ScreenedPatient> ScreenedPatients { get; set; }
        public DbSet<StudyCentre> StudyCentres { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.Configuration.LazyLoadingEnabled = false;
            base.Configuration.ProxyCreationEnabled = false;
            base.Configuration.AutoDetectChangesEnabled = false;
            base.OnModelCreating(modelBuilder);
            /*
            modelBuilder.Entity<ProtocolViolation>()
                .HasRequired(p => p.ReportingInvestigator)
                .WithMany(u => u.ReportedViolations)
                .HasForeignKey(p => p.ReportingInvestigatorId)
                .WillCascadeOnDelete(false);
            */
        }

        public string BackupDb()
        {
            //non ce sql to create .bak goes here instead
            return GetDbPath();
        }
        public DateTime DbLastModifiedUtc()
        {
            return System.IO.File.GetLastWriteTimeUtc(GetDbPath());
        }
        //create an instance method purely for reflection
        public string DbName
        {
            get { return ParticipantDbName; }
        }
        public ITrialDataContext AttachDb(string backupFilePath)
        {
            return AddDbFromBackup(backupFilePath);
        }
        public static TrialDataContext AddDbFromBackup(string backupFilePath)
        {
             //non ce sql to attach .bak goes here instead
            return new TrialDataContext(backupFilePath);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

    public class ContextCeConfiguration : DbConfiguration
    {
        public ContextCeConfiguration()
        {
            SetProviderServices(
                SqlCeProviderServices.ProviderInvariantName,
                SqlCeProviderServices.Instance);
            SetDatabaseInitializer<TrialDataContext>(new DataContextInitialiser());
            SetDatabaseInitializer<MembershipContext>(new MembershipContextInitialiser());
            
            /*
            SetDefaultConnectionFactory()
             * */
        }
        public static string GetSqlCePath(string sqlCeName)
        {
            if (!sqlCeName.EndsWith(".sdf")) { sqlCeName += ".sdf"; }
            return string.Format(@"{0}\{1}", AppDomain.CurrentDomain.GetData("DataDirectory"), sqlCeName);
        }
    }
}
