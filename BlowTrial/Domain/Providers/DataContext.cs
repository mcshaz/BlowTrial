using BlowTrial.Domain.Interfaces;
using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServerCompact;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Providers
{
    [DbConfigurationType(typeof(ContextCeConfiguration))]
    public class DataContext : DbContext, IDataContext
    {
        public const string ParticipantDbName = "ParticipantData_Centre_";
        public DataContext() : base(ParticipantDbName + System.Configuration.ConfigurationManager.AppSettings["CentreId"]) { }
        public DbSet<ProtocolViolation> ProtocolViolations { get; set; }
        public DbSet<Vaccine> Vaccines { get; set; }
        public DbSet<VaccineAdministered> VaccinesAdministered { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<ScreenedPatient> ScreenedPatients { get; set; }
        public string DbName { get { return ParticipantDbName + ".sdf"; } }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.Configuration.LazyLoadingEnabled = false;
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
    }

    public class ContextCeConfiguration : DbConfiguration
    {
        public ContextCeConfiguration()
        {
            SetProviderServices(
                SqlCeProviderServices.ProviderInvariantName,
                SqlCeProviderServices.Instance);
        }
    }
}
