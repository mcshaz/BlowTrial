using BlowTrial.Domain.Tables;
using BlowTrial.Properties;
using System.Data.Entity;

namespace BlowTrial.Domain.Providers
{
    class MembershipContextInitialiser : MigrateDatabaseToLatestVersion<MembershipContext, BlowTrial.Migrations.Membership.MembershipConfiguration>
    {
    }
    class DataContextInitialiser : MigrateDatabaseToLatestVersion<TrialDataContext, BlowTrial.Migrations.TrialData.TrialDataConfiguration>
    {
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
    }
}
