using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BlowTrial.Security
{
    class MembershipRepository : DbContext, IMembershipContext
    {
        MembershipRepository():base("Investigators")
        {
        }
        public DbSet<Investigator> Investigators { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
