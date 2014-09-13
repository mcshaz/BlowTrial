using AutoMapper;
using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using BlowTrial.View;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LinqKit;

namespace BlowTrial.ViewModel
{
    public class AllViolationsViewModel : WorkspaceViewModel
    {
        public AllViolationsViewModel(IRepository repository)
            : base(repository) 
        {
            var participantMap = AllParticipantsViewModel.GetParticipantBaseMapExpression();
            var allViolModels = (from v in _repository.ProtocolViolations.Include("Participant").AsNoTracking().AsExpandable()
                                 select new ProtocolViolationModel
                                 {
                                    Id = v.Id,
                                    Participant = participantMap.Invoke(v.Participant),
                                    Details = v.Details,
                                    ReportingInvestigator = v.ReportingInvestigator,
                                    ReportingTimeLocal = v.ReportingTimeLocal,
                                    ViolationType = v.ViolationType
                                 }).ToList();
            foreach (var v in allViolModels)
            {
                v.Participant.StudyCentre = _repository.FindStudyCentre(v.Participant.CentreId);
            }
            AllViolations = new ObservableCollection<ProtocolViolationViewModel>(allViolModels.Select(v => new ProtocolViolationViewModel(_repository, v)));
            _repository.ProtocolViolationAddOrUpdate += ViolationAddOrUpdate;
            ShowViolationDetails = new RelayCommand(ShowViolationWindow, param => SelectedViolation != null && _violationWindow == null);
        }

        private void ViolationAddOrUpdate(object sender, Domain.Providers.ProtocolViolationEventArgs e)
        {
            if (e.EventType == Domain.Providers.CRUD.Created)
            {
                if (e.Violation.Participant == null)
                {
                    e.Violation.Participant = _repository.FindParticipant(e.Violation.ParticipantId);
                }
                var violModel = Mapper.Map<ProtocolViolationModel>(e.Violation);

                violModel.Participant.StudyCentre = _repository.FindStudyCentre(violModel.Participant.CentreId);
                AllViolations.Add(new ProtocolViolationViewModel(_repository, violModel));
            }
            else
            {
                var alteredViol = AllViolations.First(v => v.Id == e.Violation.Id);
                alteredViol.ViolationType = e.Violation.ViolationType;
                alteredViol.Details = e.Violation.Details;
            }
        }
        public ObservableCollection<ProtocolViolationViewModel> AllViolations { get; private set; }
        ProtocolViolationView _violationWindow;
        ProtocolViolationViewModel _selectedViolation;
        public ProtocolViolationViewModel SelectedViolation 
        { 
            get
            {
                return _selectedViolation;
            }
            set
            {
                if (_selectedViolation == value) { return; }
                if (_violationWindow == null)
                {
                    _selectedViolation = value;
                }
                else if (((ProtocolViolationViewModel)_violationWindow.DataContext).OkToProceed())
                {
                    _violationWindow.DataContext = _selectedViolation = value;
                }
                NotifyPropertyChanged("SelectedViolation");
            }
        }

        public RelayCommand ShowViolationDetails { get; private set; }

        void ShowViolationWindow(object param)
        {
            _violationWindow = new ProtocolViolationView(SelectedViolation);
            _violationWindow.Closed += OnViolationWindow_Closed;
            EventHandler violCloseHandler = null;
            violCloseHandler = delegate
            {
                _violationWindow.Close();
                SelectedViolation.RequestClose -= violCloseHandler;
            };

            SelectedViolation.RequestClose += violCloseHandler;
            _violationWindow.Show();
        }
        void OnViolationWindow_Closed(object sender, EventArgs e)
        {
            _violationWindow.Closed -= OnViolationWindow_Closed;
            _violationWindow = null;
        }
    }
}
