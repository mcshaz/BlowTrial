namespace BlowTrial.Migrations.Membership
{
    using BlowTrial.Domain.Providers;
    using BlowTrial.Domain.Tables;
    using BlowTrial.Properties;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class MembershipConfiguration : DbMigrationsConfiguration<BlowTrial.Domain.Providers.MembershipContext>
    {
        public MembershipConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BlowTrial.Domain.Providers.MembershipContext context)
        {
            if (!context.Investigators.Any())
            {
                var admin = new Role
                {
                    Name = "Administrator"
                };
                context.Roles.Add(admin);
                context.SaveChanges();
                var usr = new Investigator
                {
                    Id = Guid.NewGuid(),
                    LastLoginAt = DateTime.Now,
                    Username = "Admin",
                    Password = "abubVufPWwivQXlNzDiTXCAYGX2PgGPkde5qGLg/TBU=", //open seasame
                    Roles = context.Roles.ToList()
                };
                context.Investigators.Add(usr);
            }
            context.SaveChanges();
        }
    }
}
