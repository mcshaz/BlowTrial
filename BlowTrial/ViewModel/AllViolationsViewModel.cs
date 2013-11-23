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

namespace BlowTrial.ViewModel
{
    public class AllViolationsViewModel : WorkspaceViewModel
    {
        public AllViolationsViewModel(IRepository repository)
            : base(repository) 
        {
            AllViolations = new ObservableCollection<ProtocolViolationViewModel>(
                Mapper.Map<List<ProtocolViolationModel>>(_repository.ProtocolViolations.ToList())
                    .Select(v=>new ProtocolViolationViewModel(_repository,v))
                );
            _repository.ProtocolViolationAdded += violationAdded;
            ShowViolationDetails = new RelayCommand(ShowViolationWindow, param => SelectedViolation != null && _violationWindow == null);
        }

        private void violationAdded(object sender, Domain.Providers.ProtocolViolationEventArgs e)
        {
            AllViolations.Add(new ProtocolViolationViewModel(_repository,Mapper.Map<ProtocolViolationModel>(e.NewViolation)));
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
