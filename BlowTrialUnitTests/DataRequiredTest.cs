using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlowTrial.Domain.Tables;
using BlowTrial.Domain.Outcomes;
using BlowTrial.Infrastructure.Interfaces;
using System.Linq;
using BlowTrial.Domain.Providers;
using System.Data.Entity;

namespace BlowTrialUnitTests
{
    [TestClass]
    public class DataRequiredTest
    {
        [TestMethod]
        public void TestVaccineMissing()
        {
            var testP = new Participant
            {
                TrialArm = RandomisationArm.RussianBCG,
                VaccinesAdministered = new VaccineAdministered[] { new VaccineAdministered { VaccineId = 1 } }
            };
            DateTime twentyEightPrior = DateTime.Now.AddDays(-28);

            var a = new Func<IParticipant, DataRequiredOption> (p=>
                ((p.OutcomeAt28Days >= OutcomeAt28DaysOption.DischargedBefore28Days && !p.DischargeDateTime.HasValue)
                            || (DeathOrLastContactRequiredIf.Contains(p.OutcomeAt28Days) && (p.DeathOrLastContactDateTime == null || (KnownDeadOutcomes.Contains(p.OutcomeAt28Days) && p.CauseOfDeath == CauseOfDeathOption.Missing))))
                        ? DataRequiredOption.DetailsMissing
                        : (p.TrialArm != RandomisationArm.Control && !p.ProtocolViolations.Any(pv => pv.ViolationType == ViolationTypeOption.MajorWrongTreatment) && !p.VaccinesAdministered.Any(v => v.VaccineGiven.IsBcg))
                            ? DataRequiredOption.BcgDataRequired
                            : (p.OutcomeAt28Days == OutcomeAt28DaysOption.Missing)
                                ? DbFunctions.DiffDays(p.DateTimeBirth, DateTime.Now) < 28
                                    ? DataRequiredOption.AwaitingOutcomeOr28
                                    : DataRequiredOption.OutcomeRequired
                                : KnownDeadOutcomes.Contains(p.OutcomeAt28Days)
                                    ? p.MaternalBCGScar == MaternalBCGScarStatus.Missing
                                        ? DataRequiredOption.MaternalBCGScarDetails
                                        : DataRequiredOption.Complete
                                    : DbFunctions.DiffDays(p.VaccinesAdministered.Any(
                                                v => v.VaccineGiven.IsBcg)
                                                    ? p.VaccinesAdministered.First(v => v.VaccineGiven.IsBcg).AdministeredAt : p.DischargeDateTime ?? p.DeathOrLastContactDateTime ?? p.RegisteredAt, DateTime.Now) < 42
                                        ? DataRequiredOption.Awaiting6WeeksToElapse
                                        : p.FollowUpBabyBCGReaction == FollowUpBabyBCGReactionStatus.Missing
                                            ? p.PermanentlyUncontactable
                                                ? DataRequiredOption.Complete
                                                : p.UnsuccesfulFollowUps.Any()
                                                    ? p.UnsuccesfulFollowUps.Count > BlowTrial.Models.ParticipantBaseModel.MaxFollowUpAttempts
                                                        ? DataRequiredOption.Complete
                                                        : DataRequiredOption.FailedInitialContact
                                                    : DataRequiredOption.AwaitingInfantScarDetails
                                            : DataRequiredOption.Complete);
            /*
            var a = new Func<IParticipant, DataRequiredOption>(p => ((p.OutcomeAt28Days >= OutcomeAt28DaysOption.DischargedBefore28Days && !p.DischargeDateTime.HasValue)
                            || (DeathOrLastContactRequiredIf.Contains(p.OutcomeAt28Days) && (p.DeathOrLastContactDateTime == null || (KnownDeadOutcomes.Contains(p.OutcomeAt28Days) && p.CauseOfDeath == CauseOfDeathOption.Missing))))
                        ? DataRequiredOption.DetailsMissing
                        : (p.TrialArm != RandomisationArm.Control && !p.VaccinesAdministered.Any(v => DataContextInitialiser.BcgVaccineIds.Contains(v.VaccineId)))
                            ? DataRequiredOption.BcgDataRequired
                            : (p.OutcomeAt28Days == OutcomeAt28DaysOption.Missing)
                                ? (p.DateTimeBirth > twentyEightPrior) //DbFunctions.DiffDays(p.DateTimeBirth, now) < 28
                                    ? DataRequiredOption.AwaitingOutcomeOr28
                                    : DataRequiredOption.OutcomeRequired
                                : DataRequiredOption.Complete);
            */

            Assert.AreNotEqual(DataRequiredOption.BcgDataRequired, a(testP));
        }

        protected static OutcomeAt28DaysOption[] DeathOrLastContactRequiredIf = new OutcomeAt28DaysOption[]
        {
            OutcomeAt28DaysOption.DiedInHospitalBefore28Days,
            OutcomeAt28DaysOption.DischargedAndKnownToHaveDied,
            OutcomeAt28DaysOption.DischargedAndLikelyToHaveDied,
            OutcomeAt28DaysOption.DischargedAndLikelyToHaveSurvived,
            OutcomeAt28DaysOption.DischargedAndOutcomeCompletelyUnknown
        };

        protected static OutcomeAt28DaysOption[] KnownDeadOutcomes = new OutcomeAt28DaysOption[]
        {
            OutcomeAt28DaysOption.DiedInHospitalBefore28Days,
            OutcomeAt28DaysOption.DischargedAndKnownToHaveDied
        };
    }
}
