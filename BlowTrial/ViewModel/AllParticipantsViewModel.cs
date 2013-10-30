using AutoMapper;
using BlowTrial.Domain.Providers;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using BlowTrial.Properties;
using BlowTrial.View;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;

namespace BlowTrial.ViewModel
{
    public class AllParticipantsViewModel : WorkspaceViewModel
    {
        #region Fields
        ParticipantUpdateView _updateWindow;
        ParticipantListItemViewModel _selectedParticipant;
        #endregion // Fields

        #region Constructor

        public AllParticipantsViewModel(IRepository repository):base(repository)
        {
            base.DisplayName = Strings.AllParticipantsViewModel_DisplayName;
            _repository.ParticipantAdded += OnParticipantAdded;
            GetAllParticipants();

            SortGridView = new RelayCommand(SortParticipants, param=>true);
            ShowUpdateDetails = new RelayCommand(ShowUpdateWindow, param => SelectedParticipant != null);


        }

        void GetAllParticipants()
        {
            /*
            var participantVMs = Array.ConvertAll(_repository.Participants.ToArray(),
                new Converter<Participant, ParticipantListItemViewModel>(p => new ParticipantListItemViewModel(Mapper.Map<ParticipantModel>(p))));
             * */
            var participantVMs = _repository.Participants.Include("VaccinesAdministered").Include("VaccinesAdministered.VaccineGiven").ToArray()
                .Select(p => new ParticipantListItemViewModel(Mapper.Map<ParticipantModel>(p)))
                .ToList();
            AllParticipants = new ListCollectionView(participantVMs);
            AllParticipants.GroupDescriptions.Add(new PropertyGroupDescription("DetailsPending"));
            AllParticipants.SortDescriptions.Add(new SortDescription("DataRequired", ListSortDirection.Ascending));
        }

        #endregion // Constructor

        #region Properties
        public ListCollectionView AllParticipants { get; private set; }
        public ParticipantListItemViewModel SelectedParticipant
        { 
            get { return _selectedParticipant; } 
            set
            {
                if (_selectedParticipant == value) { return; }
                if (_updateWindow == null || 
                    (_updateWindow != null && ((ParticipantUpdateViewModel)_updateWindow.DataContext).ChangeParticipantModel(value.ParticipantModel)))
                {
                    _selectedParticipant = value;
                }
                NotifyPropertyChanged("SelectedParticipant");
            }
        }
        

        #endregion

        #region Public Interface
        #endregion // Public Interface

        #region ICommands
        public RelayCommand ShowUpdateDetails { get; private set; }
        bool CanShowUpdateWindow(object param)
        {
            return _updateWindow == null;
        }
        void ShowUpdateWindow(object param)
        {
            var participantUpdateViewModel = new ParticipantUpdateViewModel(_repository, SelectedParticipant.ParticipantModel);

            _repository.ParticipantUpdated += participantUpdated;
            _updateWindow = new ParticipantUpdateView(participantUpdateViewModel);
            _updateWindow.Closed += updateWindow_Closed;
            _updateWindow.Show();
        }

        private void participantUpdated(object sender, EventArgs e)
        {
            SelectedParticipant.SuggestRequery();
        }

        public RelayCommand SortGridView { get; private set; }
        Dictionary<string,bool> _isAscending = new Dictionary<string,bool>();
        void SortParticipants(object param)
        {
            string propertyName = (string)param;
            bool isAscendingCol;
            if (!_isAscending.TryGetValue(propertyName, out isAscendingCol))
            {
                isAscendingCol = true;
                _isAscending.Add(propertyName, true);
            }
            AllParticipants.SortDescriptions.Clear();
            if (isAscendingCol)
            {
                AllParticipants.SortDescriptions.Add
		            (new SortDescription(propertyName, ListSortDirection.Ascending));
                _isAscending[propertyName] = false;
             }
            else
            {
                AllParticipants.SortDescriptions.Add
                   (new SortDescription(propertyName, ListSortDirection.Descending));
                _isAscending[propertyName] = true;
            }
        }
        #endregion

        #region Event Handling Methods

        #region Window Event Handlers
        private void updateWindow_Closed(object sender, EventArgs e)
        {
            var participantUpdateViewModel = (ParticipantUpdateViewModel)_updateWindow.DataContext; // ?? datacontext may have been disposed
            _repository.ParticipantUpdated -= participantUpdated;
            _updateWindow.Closing -= updateWindow_Closed;
            if (!_finalising)
            {
                SelectedParticipant.ParticipantModel = Mapper.Map<ParticipantModel>(_repository.Participants.Find(SelectedParticipant.ParticipantModel.Id));
            }
            _updateWindow = null;
        }
        #endregion

        private void OnParticipantAdded(object sender, ParticipantEventArgs e)
        {
            var viewModel = new ParticipantListItemViewModel(Mapper.Map<ParticipantModel>(e.NewParticipant));
            AllParticipants.AddNewItem(viewModel);
            AllParticipants.CommitNew();
        }

        #endregion // Event Handling Methods

        #region  finalizer
        bool _finalising;
        ~AllParticipantsViewModel()
        {
            //_newDayTimer.Dispose();
            _finalising = true;
            if (_updateWindow != null) { _updateWindow.Close(); } //this will detach the onclose handler and dispose the viewmodel
            _repository.ParticipantAdded -= OnParticipantAdded;
            _updateWindow = null;
            AllParticipants = null;
        }

        #endregion // finalizer
    }
}