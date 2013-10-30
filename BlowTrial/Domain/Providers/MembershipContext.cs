using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Providers
{
    public class MembershipContext : DbContext, IMembershipContext, IAppDataSet
    {
        public const string MembershipDbName = "MembershipDb";
        public MembershipContext() : base(MembershipDbName) { }
        public DbSet<Investigator> Investigators { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AppData> AppDataSet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.Configuration.LazyLoadingEnabled = false;
            base.Configuration.AutoDetectChangesEnabled = false;
            base.OnModelCreating(modelBuilder);
        }
    }
}
