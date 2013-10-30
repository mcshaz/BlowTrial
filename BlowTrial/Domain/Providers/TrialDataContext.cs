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
        const string ParticipantDbPrefix = @"ParticipantData_Centre_";
        static string GetDbName()
        {
            return ParticipantDbPrefix + System.Configuration.ConfigurationManager.AppSettings["CentreId"] + ".sdf";
        }
        public static string GetDbPath()
        {
            return ContextCeConfiguration.GetSqlCePath(GetDbName());
        }
        static public string GetConnectionString()
        {
            return (new SqlCeConnectionStringBuilder
            {
                  Password = "ABC",
                  DataSource = GetDbPath()
            }).ToString();
        }

        public TrialDataContext() : base(GetConnectionString()) { }
        public DbSet<ProtocolViolation> ProtocolViolations { get; set; }
        public DbSet<Vaccine> Vaccines { get; set; }
        public DbSet<VaccineAdministered> VaccinesAdministered { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<ScreenedPatient> ScreenedPatients { get; set; }
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
        public string DbBackupPath
        {
            get { return GetDbPath(); }
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
