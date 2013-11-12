using BlowTrial.Domain.Tables;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Providers
{
    class MembershipContextInitialiser : DropCreateDatabaseIfModelChanges<MembershipContext>
    {
        internal const string Administrator = "Administrator";
        protected override void Seed(MembershipContext context)
        {
            var admin = new Role
            {
                Name = Administrator
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
    class DataContextInitialiser : DropCreateDatabaseIfModelChanges<TrialDataContext>
    {
        protected override void Seed(TrialDataContext context)
        {
            context.Vaccines.Add(Bcg); //Guid.ParseExact("8eeb3307-445c-475f-b150-a51a66559ae2","D")
            context.Vaccines.Add(Opv); //Guid.ParseExact("161a6935-3e1d-4362-9b23-eb6f49512fc9", "D") 
            context.Vaccines.Add(HepB); // Guid.ParseExact("a756168a-2e0b-404b-a903-bc0cfd02f33f", "D")
            context.SaveChanges();
        }
        public static readonly Vaccine Bcg =
            new Vaccine
            {
                Id = 1,
                Name = Strings.Vaccine_Bcg
            };
        public static readonly Vaccine Opv =
            new Vaccine
            {
                Id = 2,
                Name = Strings.Vaccine_Opv
            };
        public static readonly Vaccine HepB =
            new Vaccine
            {
                Id = 3,
                Name = Strings.Vaccine_HepB
            };
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
