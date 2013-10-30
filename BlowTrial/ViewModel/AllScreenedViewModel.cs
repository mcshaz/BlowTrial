using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BlowTrial.ViewModel
{
    public class AllScreenedViewModel : WorkspaceViewModel
    {
        public AllScreenedViewModel(IRepository repository) : base(repository) 
        {
            AllScreened = new ObservableCollection<ScreenedPatient>(_repository.ScreenedPatients.ToList());
            _repository.ScreenedPatientAdded += _repository_ScreenedPatientAdded;
        }

        private void _repository_ScreenedPatientAdded(object sender, Domain.Providers.ScreenedPatientEventArgs e)
        {
            AllScreened.Add(e.NewScreenedPatient);
        }
        public ObservableCollection<ScreenedPatient> AllScreened { get; private set; }
    }
}
