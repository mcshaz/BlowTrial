using AutoMapper;
using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Providers;
using BlowTrial.Domain.Tables;
using BlowTrial.Helpers;
using BlowTrial.Infrastructure;
using BlowTrial.Infrastructure.Converters;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using BlowTrial.Properties;
using BlowTrial.View;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Data;
using System.Windows.Threading;

namespace BlowTrial.ViewModel
{
    public class AllParticipantsViewModel : WorkspaceViewModel, ICleanup
    {
        #region Fields
        ParticipantUpdateView _updateWindow;
        ParticipantListItemViewModel _selectedParticipant;
        AgeUpdatingService _ageUpdater;
        #endregion // Fields

        #region Constructor

        public AllParticipantsViewModel(IRepository repository, bool isBackingUpToCloud):base(repository)
        {
            base.DisplayName = Strings.AllParticipantsViewModel_DisplayName;
            _repository.ParticipantAdded += OnParticipantAdded;
            GetAllParticipants();
            if (isBackingUpToCloud)
            {
                _repository.ParticipantUpdated += HandleCloudUpdate;
            }

            SortGridView = new RelayCommand(SortParticipants);
            ShowUpdateDetails = new RelayCommand(ShowUpdateWindow, param => SelectedParticipant != null && _updateWindow==null);
            CreateProtocolViolation = new RelayCommand(ShowProtocolViolation, param => SelectedParticipant != null);
            ShowUpdateEnrolment = new RelayCommand(ShowEnrolDetails, param => SelectedParticipant != null);

            Mediator.Register("MainWindowClosing", OnMainWindowClosing);
        }

        void GetAllParticipants()
        {
            /*
            var participantVMs = Array.ConvertAll(_repository.Participants.ToArray(),
                new Converter<Participant, ParticipantListItemViewModel>(p => new ParticipantListItemViewModel(Mapper.Map<ParticipantModel>(p))));
             * */
            var parts = _repository.Participants.Include("VaccinesAdministered") //.Include("VaccinesAdministered.VaccineGiven")
                         .Select(ParticipantBaseMapExpression()).ToArray();
            var now = DateTime.Now;
            var dt28prior = now.AddDays(-28);
            var dataRequired = ParticipantBaseModel.GetDataRequiredExpression(dt28prior).Compile();
            foreach (var p in parts)
            {
                p.DataRequired = dataRequired(p);
                p.StudyCentre = _repository.FindStudyCentre(p.CentreId);
            }

            var participantVMs = new List<ParticipantListItemViewModel>(parts.Length);
            participantVMs.AddRange(parts.Select(p => new ParticipantListItemViewModel(p)));
            _ageUpdater = new AgeUpdatingService(participantVMs);
            _ageUpdater.OnAgeIncrement += OnNewAge;

            AllParticipants = new ListCollectionView(participantVMs);
            AllParticipants.GroupDescriptions.Add(new PropertyGroupDescription("DataRequiredString"));
            AllParticipants.SortDescriptions.Add(new SortDescription("DataRequiredSortOrder", ListSortDirection.Ascending));

            if(_repository.LocalStudyCentres.Skip(1).Any())
            {
                AllParticipants.GroupDescriptions.Add(new PropertyGroupDescription("StudyCentre", new StudyCentreModelToNameConverter()));
                //AllParticipants.SortDescriptions.Add(new SortDescription("StudyCentreName", ListSortDirection.Ascending));
            }

            //creating dispatchertimer so that screen is rendered before setting up the birthtime updating algorithms

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
                if (_updateWindow == null)
                {
                    _selectedParticipant = value;
                }
                else if (((ParticipantProgressViewModel)_updateWindow.DataContext).OkToProceed())
                {
                    OnClosingOrChangingWindowContext();
                    _updateWindow.DataContext = _selectedParticipant = GetSelectedProgressViewModel(value);
                }
                NotifyPropertyChanged("SelectedParticipant");
            }
        }
        
        #endregion

        #region Public Interface
        #endregion // Public Interface

        #region ICommands
        public RelayCommand ShowUpdateDetails { get; private set; }

        void ShowUpdateWindow(object param)
        {
            var updatableParticipant = GetSelectedProgressViewModel(SelectedParticipant);
            _selectedParticipant = updatableParticipant;
            _updateWindow = new ParticipantUpdateView(updatableParticipant);
            _updateWindow.Closed += OnUpdateWindow_Closed;
            updatableParticipant.OnSave += ParticipantProgressSaved;
            _updateWindow.Show();
        }

        ParticipantProgressViewModel GetSelectedProgressViewModel(ParticipantListItemViewModel p)
        {
            ParticipantProgressViewModel updatableParticipant = p as ParticipantProgressViewModel;
            if (updatableParticipant == null)
            {
                var currentList = (List<ParticipantListItemViewModel>)AllParticipants.SourceCollection;
                int indx = currentList.IndexOf(SelectedParticipant);
                var part = Mapper.Map<ParticipantProgressModel>(_repository.FindParticipant(p.Id));
                currentList[indx] = updatableParticipant = new ParticipantProgressViewModel(_repository, part);
            }
            return updatableParticipant;
        }

        void ParticipantProgressSaved(object sender, EventArgs e)
        {
            var p = (ParticipantProgressViewModel)_updateWindow.DataContext;
            AllParticipants.EditItem(p);
            AllParticipants.CommitEdit();
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
        public RelayCommand ShowUpdateEnrolment { get; private set; }
        void ShowEnrolDetails(object param)
        {
            var window = new PatientDemographicUpdateView();
            var model =  Mapper.Map<PatientDemographicsModel>(_repository.FindParticipant(SelectedParticipant.Id));
            model.StudyCentre = SelectedParticipant.StudyCentre;
            var vm = new PatientDemographicsViewModel(_repository, model);
            window.DataContext = vm;
            _repository.ParticipantUpdated += HandleDemographicUpdate;
            EventHandler enrolCloseHandler = null;
            enrolCloseHandler = delegate
            {
                window.Close();
                vm.RequestClose -= enrolCloseHandler;
            };

            vm.RequestClose += enrolCloseHandler;
            window.ShowDialog();
            _repository.ParticipantUpdated -= HandleDemographicUpdate;
        }

        void HandleDemographicUpdate(object sender, ParticipantEventArgs e)
        {
            AllParticipants.EditItem(SelectedParticipant);
            UpdateDemographics(e.Participant, SelectedParticipant);
            AllParticipants.CommitEdit();
        }
        static void UpdateDemographics(Participant p, ParticipantListItemViewModel vm)
        {
            vm.AgeDays = (DateTime.Now - p.DateTimeBirth).Days;
            vm.DateTimeBirth = p.DateTimeBirth;
            vm.IsMale = p.IsMale;
            vm.Name = p.Name;
            vm.RegisteredAt = p.RegisteredAt;
            vm.HospitalIdentifier = p.HospitalIdentifier;
            vm.RegisteredAt = p.RegisteredAt;

            var sp = vm as ParticipantProgressViewModel;
            if (sp != null)
            {
                sp.PhoneNumber = p.PhoneNumber;
                sp.MothersName = p.MothersName;
                sp.AdmissionWeight = p.AdmissionWeight;
            }
        }
        void HandleCloudUpdate(object sender, ParticipantEventArgs e)
        {
            var assdVM = ((List<ParticipantListItemViewModel>)AllParticipants.SourceCollection)
                .First(p=>p.Id == e.Participant.Id);
            AllParticipants.EditItem(assdVM);
            UpdateDemographics(e.Participant, assdVM);
            assdVM.VaccinesAdministered = e.Participant.VaccinesAdministered;
            var sp = assdVM as ParticipantProgressViewModel;
            if (sp != null)
            {
                sp.OutcomeAt28Days = e.Participant.OutcomeAt28Days;
                sp.CauseOfDeath = e.Participant.CauseOfDeath;
                sp.DeathOrLastContactDateTime = e.Participant.DeathOrLastContactDateTime;
                sp.DischargeDateTime = e.Participant.DischargeDateTime;
                sp.LastContactWeight = e.Participant.LastContactWeight;
                sp.LastWeightDate = e.Participant.LastWeightDate;
                sp.OtherCauseOfDeathDetail = e.Participant.OtherCauseOfDeathDetail;
                sp.BcgAdverse = e.Participant.BcgAdverse;
                sp.BcgAdverseDetail = e.Participant.BcgAdverseDetail;
                sp.BcgPapule = e.Participant.BcgPapule;
                sp.IsParticipantModelChanged = false;
                sp.IsVaccineAdminChanged = false; //ensure save changes is not enabled
            }

            AllParticipants.CommitEdit();
        }

        public RelayCommand CreateProtocolViolation { get; private set;}
        void ShowProtocolViolation(object param)
        {
            var viol = new ProtocolViolationModel
            {
                Participant = SelectedParticipant.Participant
            };
            var violVM = new ProtocolViolationViewModel(_repository, viol);
            var violView = new ProtocolViolationView(violVM);

            EventHandler violCloseHandler = null;
            violCloseHandler = delegate
            {
                violView.Close();
                violVM.RequestClose -= violCloseHandler;
            };

            violVM.RequestClose += violCloseHandler;
            violView.ShowDialog();
        }
        #endregion

        #region Methods


        #endregion //Methods

        #region Events
        void OnNewAge(object sender, AgeIncrementingEventArgs e)
        {
            AllParticipants.EditItem(e.ParticipantViewModel);
            e.ParticipantViewModel.AgeDays = e.NewAge;
            AllParticipants.CommitEdit();
        }

        private void OnParticipantAdded(object sender, ParticipantEventArgs e)
        {
            var viewModel = new ParticipantProgressViewModel(_repository, Mapper.Map<ParticipantProgressModel>(e.Participant));
            _ageUpdater.AddParticipant(viewModel);
            AllParticipants.AddNewItem(viewModel);
            AllParticipants.CommitNew();
        }

        void OnUpdateWindow_Closed(object sender, EventArgs e)
        {
            _updateWindow.Closed -= OnUpdateWindow_Closed;
            OnClosingOrChangingWindowContext();
            _updateWindow = null;
        }

        void OnClosingOrChangingWindowContext()
        {
            var p = _updateWindow.DataContext as ParticipantProgressViewModel;
            if (p != null) { p.OnSave -= ParticipantProgressSaved; }
        }

        void OnMainWindowClosing(object args)
        {
            if (_updateWindow == null) { return; }
            if (((ParticipantProgressViewModel)SelectedParticipant).OkToProceed())
            {
                _updateWindow.Close();
            }
            else
            {
                ((System.ComponentModel.CancelEventArgs)args).Cancel = true;
            }
        }

        Func<Participant, ParticipantBaseModel> _participantBaseMap;
        Func<Participant, ParticipantBaseModel> ParticipantBaseMap
        {
            get
            {
                return _participantBaseMap ?? (_participantBaseMap = ParticipantBaseMapExpression().Compile());
            }
        }
        static Expression<Func<Participant, ParticipantBaseModel>> ParticipantBaseMapExpression()
        {
            return p => new ParticipantBaseModel
                         {
                             CentreId = p.CentreId,
                             HospitalIdentifier = p.HospitalIdentifier,
                             DateTimeBirth = p.DateTimeBirth,
                             Id = p.Id,
                             IsInterventionArm = p.IsInterventionArm,
                             IsMale = p.IsMale,
                             Name = p.Name,
                             RegisteredAt = p.RegisteredAt,
                             OutcomeAt28Days = p.OutcomeAt28Days,
                             DischargeDateTime = p.DischargeDateTime,
                             DeathOrLastContactDateTime = p.DeathOrLastContactDateTime,
                             CauseOfDeath = p.CauseOfDeath,
                             VaccinesAdministered = p.VaccinesAdministered,
                         };
        }
        #endregion // Events

        #region ICleanup Implementation
        public void Cleanup()
        {
            _ageUpdater.Cleanup();
        }
        #endregion

        #region  finalizer
        ~AllParticipantsViewModel()
        {
            _repository.ParticipantAdded -= OnParticipantAdded;
            _repository.ParticipantUpdated -= HandleCloudUpdate;
            
            Mediator.Unregister("MainWindowClosing", OnMainWindowClosing);
        }

        #endregion // finalizer
    }
}