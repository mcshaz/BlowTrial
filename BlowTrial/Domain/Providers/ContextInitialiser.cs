using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Providers
{
    class MembershipContextInitialiser : DropCreateDatabaseIfModelChanges<MembershipContext>
    {
        protected override void Seed(MembershipContext context)
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
            context.SaveChanges();
        }
    }
    class DataContextInitialiser : DropCreateDatabaseIfModelChanges<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            context.Vaccines.Add(new Vaccine{ Name = Vaccine.BcgName()});
            context.Vaccines.Add(new Vaccine { Name = Vaccine.OpvName() });
            context.Vaccines.Add(new Vaccine{ Name = Vaccine.HepBName() });
            context.SaveChanges();
        }
        /*
        static void CreateTestParticipants(DataContext context)
        {
            const int daysBack = 35;
            const int newPtCount = 35;
            var startdate = DateTime.Now.AddDays(-daysBack);
            var rand = new Random();
            for (int i =0;i<newPtCount;i++)
            {
                var p = new Participant
                {
                     AdmissionWeight = rand.Next(400,2000),
                     Name = "Trial Participant" + i.ToString(),
                     Abnormalities = 
                }
            }
        }
        */
    }
}
