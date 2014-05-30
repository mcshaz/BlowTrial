using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlowTrial.Domain.Providers;
using BlowTrial.Models;
using System.Collections.Generic;
using AutoMapper;
using BlowTrial.Infrastructure.Automapper;
using System.Linq;
using BlowTrial.ViewModel;

namespace BlowTrialUnitTests
{
    [TestClass]
    public class LiveDataTests
    {
        [TestMethod]
        public void AllLiveDemographicsUpdatable()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<PatientProfiles>();
            }); 
            List<int> idsCannotExecute = new List<int>();
            using (var liveRepo = new Repository(() => new TrialDataContext()))
            { 
                foreach (var p in liveRepo.Participants.ToList())
                {
                    PatientDemographicsModel model = Mapper.Map<PatientDemographicsModel>(p);
                    model.StudyCentre = liveRepo.FindStudyCentre(p.CentreId);
                    Assert.IsNotNull(model.StudyCentre,"Foreign key constraint violated: database does not include record for given study centre id: {0}", p.CentreId);
                    var vm = new PatientDemographicsViewModel(liveRepo, model);
                    vm.Name += " ";
                    if (!vm.UpdateDemographicsCmd.CanExecute(null)) {idsCannotExecute.Add(model.Id);}
                }
            }
            Assert.IsFalse(idsCannotExecute.Any(), "unable to update demographics for ids: {0}", string.Join(",",idsCannotExecute));
        }
    }
}
