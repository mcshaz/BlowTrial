using BlowTrial.Domain.Tables;
using BlowTrial.Properties;
using System.Data.Entity;
using System.Linq;

namespace BlowTrial.Domain.Providers
{
    class MembershipContextInitialiser : MigrateDatabaseToLatestVersion<MembershipContext, BlowTrial.Migrations.Membership.MembershipConfiguration>
    {
    }
    class DataContextInitialiser : MigrateDatabaseToLatestVersion<TrialDataContext, BlowTrial.Migrations.TrialData.TrialDataConfiguration>
    {
        public static readonly Vaccine RussianBcg =
            new Vaccine
            {
                Id = 1,
                Name = Strings.Vaccine_RussianBcg
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
        public static readonly Vaccine DanishBcg =
            new Vaccine{
                Id = 5,
                Name=Strings.Vaccine_DanishBcg
            };

        public static int[] SeedVaccineIds = new Vaccine[] { RussianBcg, Opv, HepB,DanishBcg }.Select(v => v.Id).ToArray();
    }
}
