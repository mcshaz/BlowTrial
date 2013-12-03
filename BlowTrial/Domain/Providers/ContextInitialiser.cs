using BlowTrial.Domain.Tables;
using BlowTrial.Migrations;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Providers
{
    class MembershipContextInitialiser : MigrateDatabaseToLatestVersion<MembershipContext, MembershipConfiguration>
    {

    }
    class DataContextInitialiser : MigrateDatabaseToLatestVersion<TrialDataContext,TrialDataConfiguration>
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
