using BlowTrial.Domain.Tables;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BlowTrial.Domain.Providers
{
    class MembershipContextInitialiser : MigrateDatabaseToLatestVersion<MembershipContext, BlowTrial.Migrations.Membership.MembershipConfiguration>
    {
    }
    public class DataContextInitialiser : MigrateDatabaseToLatestVersion<TrialDataContext, BlowTrial.Migrations.TrialData.TrialDataConfiguration>
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
        public static readonly Vaccine BcgMoreau =
            new Vaccine
            {
                Id = 6,
                Name = Strings.Vaccine_BcgBrazil
            };

        public const int MaxReservedVaccineId = 20;

        public static int[] SeedVaccineIds(AllocationGroups group)
        {
            var returnList = new List<int>(5);
            returnList.Add(Opv.Id);
            returnList.Add(HepB.Id);
            switch(group)
            {
                case AllocationGroups.NotApplicable:
                    throw new ArgumentException("NotApplicatble should never be used as an allocationGroup");
                case AllocationGroups.India2Arm:
                case AllocationGroups.India3ArmBalanced:
                case AllocationGroups.India3ArmUnbalanced:
                    returnList.Add(RussianBcg.Id);
                    returnList.Add(DanishBcg.Id);
                    break;
                case AllocationGroups.Brazil2Arm:
                    returnList.Add(BcgMoreau.Id);
                    break;
            }
            var returnVar = new int[returnList.Count];
            returnList.CopyTo(returnVar);
            return returnVar;
        }

        public static int[] BcgVaccineIds = new int[] {RussianBcg.Id,DanishBcg.Id,BcgMoreau.Id };

        public static bool IsBcg(int vaccineId)
        {
            return BcgVaccineIds.Contains(vaccineId);
        }
    }
}
