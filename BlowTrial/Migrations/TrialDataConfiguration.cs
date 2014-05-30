namespace BlowTrial.Migrations.TrialData
{
    using BlowTrial.Domain.Outcomes;
    using BlowTrial.Domain.Providers;
    using BlowTrial.Infrastructure;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class TrialDataConfiguration : DbMigrationsConfiguration<BlowTrial.Domain.Providers.TrialDataContext>
    {
        public TrialDataConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BlowTrial.Domain.Providers.TrialDataContext context)
        {
            /*
            if (!(from v in context.Vaccines
                where v.Id == DataContextInitialiser.DanishBcg.Id
                select v).Any())
            */

            context.Vaccines.AddOrUpdate(v=>v.Id, DataContextInitialiser.RussianBcg); //Guid.ParseExact("8eeb3307-445c-475f-b150-a51a66559ae2","D")
            context.Vaccines.AddOrUpdate(v => v.Id, DataContextInitialiser.Opv); //Guid.ParseExact("161a6935-3e1d-4362-9b23-eb6f49512fc9", "D") 
            context.Vaccines.AddOrUpdate(v => v.Id, DataContextInitialiser.HepB); // Guid.ParseExact("a756168a-2e0b-404b-a903-bc0cfd02f33f", "D")
            context.Vaccines.AddOrUpdate(v => v.Id, DataContextInitialiser.DanishBcg);
            context.SaveChanges(true);

            /*
            if(context.StudyCentres.Any(s=>s.Id == 1) && !context.Participants.Any(p=>p.WasEnvelopeRandomised))
            {
                context.Database.ExecuteSqlCommand(string.Format("UPDATE [Participants] SET [WasEnvelopeRandomised] = 1 WHERE Id <= {0} OR MultipleSiblingId <= {0};", EnvelopeDetails.MaxEnvelopeNumber));
            }
            context.Database.ExecuteSqlCommand("UPDATE [Participants] SET [PhoneNumber] = null WHERE SUBSTRING(PhoneNumber,LEN(PhoneNumber)-8,8) = '99999999';");
            
            
            if (!context.Participants.Any(p=>p.RandomisationCategory!=RandomisationCategories.NotSet))
            {
                ExecuteSqlCommands(context.Database, Envelope.UpdateCategoriesSql);
                ExecuteSqlCommands(context.Database, string.Format(
                    "UPDATE [Participants] SET [RandomisationCategory] = 1 WHERE [RandomisationCategory]=0 AND IsMale = 1 AND AdmissionWeight < {0};" +
                    "UPDATE [Participants] SET [RandomisationCategory] = 2 WHERE [RandomisationCategory]=0 AND IsMale = 0 AND AdmissionWeight < {0};" +
                    "UPDATE [Participants] SET [RandomisationCategory] = 3 WHERE [RandomisationCategory]=0 AND IsMale = 1 AND AdmissionWeight >= {0} AND AdmissionWeight < {1};" +
                    "UPDATE [Participants] SET [RandomisationCategory] = 4 WHERE [RandomisationCategory]=0 AND IsMale = 0 AND AdmissionWeight >= {0} AND AdmissionWeight < {1};" +
                    "UPDATE [Participants] SET [RandomisationCategory] = 5 WHERE [RandomisationCategory]=0 AND IsMale = 1 AND AdmissionWeight >= {1};" +
                    "UPDATE [Participants] SET [RandomisationCategory] = 6 WHERE [RandomisationCategory]=0 AND IsMale = 0 AND AdmissionWeight >= {1};",
                    RandomisingEngine.BlockWeight1, RandomisingEngine.BlockWeight2));
            }
            */
        }
        static void ExecuteSqlCommands(Database db, string commands)
        {
            foreach (string s in commands.Split(new string[]{ "go","Go","GO",";" },StringSplitOptions.None))
            {
                if (s.Trim('\r','\n',' ','\t') != "")
                {
                    db.ExecuteSqlCommand(s);
                }
            }
        }
    }
}
