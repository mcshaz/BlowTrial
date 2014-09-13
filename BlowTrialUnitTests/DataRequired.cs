using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using System.Collections.Generic;
using BlowTrial.Domain.Tables;
using BlowTrial.ViewModel;
using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Interfaces;
using BlowTrial.Domain.Providers;

namespace BlowTrialUnitTests
{
    [TestClass]
    public class DataRequired
    {
        [TestMethod]
        public void TestRequiredChange()
        {
            //var context = new Mock<ITrialDataContext>();
            //var repo = new Repository(() => context.Object);
            var emptyViol = new ProtocolViolation[0];
            var now = DateTime.Now;
            var part = new ParticipantProgressModel
            {
                Id =1,
                TrialArm = RandomisationArm.RussianBCG, 
                VaccinesAdministered = new VaccineAdministered[0], 
                ProtocolViolations = emptyViol,
                RegisteredAt = now.AddDays(-1),
                DateTimeBirth = now.AddHours(-26)
            };
            var partVM = new ParticipantListItemViewModel(part);
            var changeList = new HashSet<string>();
            partVM.PropertyChanged += (o, e) => changeList.Add(e.PropertyName);
            Assert.AreEqual(DataRequiredOption.BcgDataRequired,partVM.DataRequired);
            partVM.ProtocolViolations = new ProtocolViolation[]{ 
                new ProtocolViolation
                {
                    Details = "test",
                    Id = 1,
                    ParticipantId = 1,
                    ReportingTimeLocal = now,
                    ViolationType = ViolationTypeOption.MajorWrongTreatment
                }};
            Assert.IsTrue(changeList.Contains("DataRequired"),"DataRequired was not included as a PropertyChanged argument");
            Assert.AreEqual(DataRequiredOption.AwaitingOutcomeOr28, partVM.DataRequired);
            changeList.Clear();
            partVM.ProtocolViolations = emptyViol;
            Assert.IsTrue(changeList.Contains("DataRequired"), "DataRequired was not included as a PropertyChanged argument");
            Assert.AreEqual(DataRequiredOption.BcgDataRequired, partVM.DataRequired);
            changeList.Clear();
            partVM.VaccinesAdministered = new VaccineAdministered[]{
                new VaccineAdministered {
                    AdministeredAt = now, 
                    ParticipantId=1, 
                    Id=1, 
                    VaccineGiven= DataContextInitialiser.RussianBcg,
                    VaccineId = DataContextInitialiser.RussianBcg.Id
                }};
            Assert.IsTrue(changeList.Contains("DataRequired"), "DataRequired was not included as a PropertyChanged argument");
            Assert.AreEqual(DataRequiredOption.AwaitingOutcomeOr28,partVM.DataRequired);
        }
    }
}
