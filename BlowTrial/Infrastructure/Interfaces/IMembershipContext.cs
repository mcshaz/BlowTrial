using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Interfaces
{
    public interface IMembershipContext
    {
        DbSet<Investigator> Investigators { get; set; }
        DbSet<Role> Roles { get; set; }
        int SaveChanges();
        void Dispose();
    }
}
