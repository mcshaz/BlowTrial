using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlowTrial.Domain.Providers;
using BlowTrial.Models;
using BlowTrial.Domain.Tables;
using System.Collections.Generic;

namespace BlowTrialUnitTests
{
    [TestClass]
    public class ObjectDumperTests
    {
        [TestMethod]
        public void TestObjectDumper()
        {
            var va = new VaccineAdministeredModel() { AdministeredAtDateTime = DateTime.Now, Id = 1, VaccineId = 1 };
            var p = new ParticipantProgressModel
            {
                AdmissionDiagnosis = "asd",
                AdmissionWeight = 1900,
                AgeDays = 3, 
                TrialArm = BlowTrial.Domain.Outcomes.RandomisationArm.RussianBCG,
                DateTimeBirth = DateTime.Now, 
                VaccineModelsAdministered = new VaccineAdministeredModel[] {va, va},
                ProtocolViolations = new ProtocolViolation[0]
            };
            va.AdministeredTo = p;
            Console.Write(GenericToDataString.ObjectDumper.Dump(p));
            Console.Write(GenericToDataString.ObjectDumper.Dump(null));
            IEnumerable<VaccineAdministered> vas = null;
            Console.Write(GenericToDataString.ObjectDumper.Dump(vas));
            vas = new VaccineAdministered[] { new VaccineAdministered { Id = 1, ParticipantId = 3 } };
            Console.Write(GenericToDataString.ObjectDumper.Dump(vas));
            var c = new Dictionary<int, string>();
            c.Add(1, "one");
            c.Add(2, "two");
            Console.Write(GenericToDataString.ObjectDumper.Dump(c));
            var d = new Dictionary<int,VaccineAdministeredModel>();
            d.Add(1, va);
            Console.Write(GenericToDataString.ObjectDumper.Dump(d));
        }
    }
}
