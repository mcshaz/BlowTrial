namespace BlowTrial.Migrations
{
    using BlowTrial.Domain.Providers;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class TrialDataConfiguration : DbMigrationsConfiguration<BlowTrial.Domain.Providers.TrialDataContext>
    {
        public TrialDataConfiguration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BlowTrial.Domain.Providers.TrialDataContext context)
        {
            if (!context.Vaccines.Any())
            {
                context.Vaccines.Add(DataContextInitialiser.Bcg); //Guid.ParseExact("8eeb3307-445c-475f-b150-a51a66559ae2","D")
                context.Vaccines.Add(DataContextInitialiser.Opv); //Guid.ParseExact("161a6935-3e1d-4362-9b23-eb6f49512fc9", "D") 
                context.Vaccines.Add(DataContextInitialiser.HepB); // Guid.ParseExact("a756168a-2e0b-404b-a903-bc0cfd02f33f", "D")
                context.SaveChanges();
            }
        }
    }
}
