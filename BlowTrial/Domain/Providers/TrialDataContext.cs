using BlowTrial.Domain.Interfaces;
using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServerCompact;
using System.Data.Entity.Validation;
using System.Data.SqlServerCe;
using System.Diagnostics;
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
        public override int SaveChanges()
        {
            try
            {
                CheckValidation();
            }
            catch (DbEntityValidationException)
            {
                throw;
            }
            DateTime? updateTime = null;
            foreach (DbEntityEntry ent in this.ChangeTracker.Entries())
            {
                var sr = ent.Entity as ISharedRecord;
                if (sr != null)
                {
                    if (sr.Id == Guid.Empty) 
                    {
                        if (ent.State != EntityState.Added) { throw new InvalidOperationException("Guid was empty on a non add opertation!"); }
                        sr.Id = new Guid(); 
                    }
                    if (ent.State == EntityState.Added || ent.State==EntityState.Modified) 
                    { 
                        sr.RecordLastModified = (updateTime ?? (updateTime = DateTime.UtcNow)).Value; 
                    }
                }
            }
            return base.SaveChanges();
        }
        [Conditional("DEBUG")]
        void CheckValidation(string details = "")
        {
            var valCollection = this.GetValidationErrors();
            if (!valCollection.Any()) { return; }
            var valInfo = new System.Text.StringBuilder("The following entities did not meet following validation requirements: " + details + Environment.NewLine);
            foreach (var valResults in valCollection)
            {
                valInfo.AppendFormat("Class -> {0}{1}", valResults.Entry.Entity.GetType().FullName, Environment.NewLine);
                foreach (var valErr in valResults.ValidationErrors)
                {
                    valInfo.AppendFormat("----Property: {0} Error: {1}{2}",
                    valErr.PropertyName,
                    valErr.ErrorMessage,
                    Environment.NewLine);
                }
                valInfo.AppendLine("------------------------------");
            }
            throw new DbEntityValidationException(valInfo.ToString());
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
