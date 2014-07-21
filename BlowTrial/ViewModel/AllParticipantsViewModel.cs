using AutoMapper;
using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Providers;
using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure;
using BlowTrial.Infrastructure.Converters;
using BlowTrial.Infrastructure.CustomSorters;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Infrastructure.ThreadHelpers;
using BlowTrial.Models;
using BlowTrial.Properties;
using BlowTrial.View;
using MvvmExtraLite.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Data;

namespace BlowTrial.ViewModel
{
    public class AllParticipantsViewModel : WorkspaceViewModel, ICleanup
    {
        #region Fields
        ParticipantUpdateView _updateWindow;
        ParticipantListItemViewModel _selectedParticipant;
        AgeUpdatingService _ageUpdater;
        DeferredAction _deferredAction;
        string _searchString;
        #endregion // Fields

        #region Constructor
        void SetDisplayName()
        {
            base.DisplayName = string.Format(Strings.AllParticipantsViewModel_DisplayName,
                    AllParticipants.Count,
                    ((IList)AllParticipants.SourceCollection).Count);
        }

        public AllParticipantsViewModel(IRepository repository):base(repository)
        {
            GetAllParticipants();
            SetDisplayName();
            _repository.ParticipantAdded += OnParticipantAdded;
            _repository.ParticipantUpdated += HandleParticipantUpdate;

            SortGridView = new RelayCommand(SortParticipants);
            ShowUpdateDetails = new RelayCommand(ShowUpdateWindow, param => SelectedParticipant != null && _updateWindow==null);
            CreateProtocolViolation = new RelayCommand(ShowProtocolViolation, param => SelectedParticipant != null);
            ShowUpdateEnrolment = new RelayCommand(ShowEnrolDetails, param => SelectedParticipant != null);

            SearchDelay = TimeSpan.FromMilliseconds(400);

            Mediator.Register("MainWindowClosing", OnMainWindowClosing);
        }

        void GetAllParticipants()
        {
            /*
            var participantVMs = Array.ConvertAll(_repository.Participants.ToArray(),
                new Converter<Participant, ParticipantListItemViewModel>(p => new ParticipantListItemViewModel(Mapper.Map<ParticipantModel>(p))));
             * */
            var now = DateTime.Now;
            var dt28prior = now.AddDays(-28);
            var dataRequired = ParticipantBaseModel.GetDataRequiredExpression(dt28prior).Compile();

            List<ParticipantListItemViewModel> participantVMs = new List<ParticipantListItemViewModel>(_repository.Participants.Count());
            foreach (var p in _repository.Participants.Include("VaccinesAdministered").OrderByDescending(dp=>dp.Id) 
                         .Select(GetParticipantBaseMapExpression()))
            {
                p.DataRequired = dataRequired(p);
                p.StudyCentre = _repository.FindStudyCentre(p.CentreId);
                var newVm = new ParticipantListItemViewModel(p);
                newVm.PropertyChanged += OnListItemChanged;
                participantVMs.Add(newVm);
            }

            _ageUpdater = new AgeUpdatingService(participantVMs);
            _ageUpdater.OnAgeIncrement += OnNewAge;

            AllParticipants = new ListCollectionView(participantVMs);
            AllParticipants.CustomSort = new ParticipantIdSortDesc();
            //put in reverse to speed things up, this will merely put new entries at the top


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
                _selectedParticipant = value;
                if (_updateWindow != null && value != null && ((ParticipantProgressViewModel)_updateWindow.DataContext).OkToProceed())
                {
                    _updateWindow.DataContext = GetSelectedProgressViewModel(value.Id);
                }
                NotifyPropertyChanged("SelectedParticipant");
            }
        }

        public string SearchString
        {
            get 
            {
                return _searchString;
            }
            set
            {
                if (_searchString == value) { return; }
                _searchString = value;
                NotifyPropertyChanged("SearchString");

                if (this._deferredAction == null)
                {
                    this._deferredAction = DeferredAction.Create(SetFilter);
                }

                // Defer applying search criteria until time has elapsed.
                _deferredAction.Defer(SearchDelay);
                
            }
        }

        void SetFilter()
        {
            const StringComparison compareBy = StringComparison.Ordinal;
            if (string.IsNullOrEmpty(_searchString))
            {
                if (_groupByDataRequired) 
                {
                    AllParticipants.Filter = new Predicate<object>(item => {
                        var vm = (ParticipantListItemViewModel)item;
                        return _selectedDataRequired.Contains(vm.DataRequired) && _selectedCentres.Contains(vm.StudyCentre);
                    });
                }
                else { AllParticipants.Filter = null; }
            }
            else
            {
                if (_groupByDataRequired)
                {
                    AllParticipants.Filter = new Predicate<object>(item => {
                        var vm = (ParticipantListItemViewModel)item;
                        return _selectedDataRequired.Contains(vm.DataRequired) && _selectedCentres.Contains(vm.StudyCentre) && vm.SearchableString.IndexOf(_searchString, compareBy) >= 0;
                    });
                }
                else
                {
                    AllParticipants.Filter = new Predicate<object>(item => ((ParticipantListItemViewModel)item).SearchableString.IndexOf(_searchString, compareBy) >= 0);
                }
            }
            SetDisplayName();
        }

        public TimeSpan SearchDelay { get; set; }
        #endregion

        #region Public Interface
        #endregion // Public Interface

        #region ICommands
        public RelayCommand ShowUpdateDetails { get; private set; }

        void ShowUpdateWindow(object param)
        {
            var updatableParticipant = GetSelectedProgressViewModel(SelectedParticipant.Id);
            _updateWindow = new ParticipantUpdateView(updatableParticipant);
            _updateWindow.Closed += OnUpdateWindow_Closed;
            _updateWindow.Show();
        }

        ParticipantProgressViewModel GetSelectedProgressViewModel(int participantId)
        {
                var part = Mapper.Map<ParticipantProgressModel>(_repository.FindParticipantAndVaccines(participantId));
                return new ParticipantProgressViewModel(_repository, part);
        }

        public RelayCommand SortGridView { get; private set; }
        Dictionary<string,bool> _isAscending = new Dictionary<string,bool>();
        void SortParticipants(object param)
        {
            string propertyName = (string)param;
            bool isAscendingCol;
            if (_isAscending.TryGetValue(propertyName, out isAscendingCol))
            {
                isAscendingCol = _isAscending[propertyName] = !isAscendingCol;
            }
            else
            {
                isAscendingCol = true;
                _isAscending.Add(propertyName, true);
            }
            switch(propertyName)
            {
                case "Id":
                    AllParticipants.CustomSort = (isAscendingCol)?(IComparer)new ParticipantIdSorter(): new ParticipantIdSortDesc();
                    break;
                case "Name":
                    AllParticipants.CustomSort = (isAscendingCol)?(IComparer)new ParticipantNameSorter(): new ParticipantNameSortDesc();
                    break;
                case "HospitalIdentifier":
                    AllParticipants.CustomSort = (isAscendingCol)?(IComparer)new ParticipantHospitalIdSorter(): new ParticipantHospitalIdSortDesc();
                    break;
                case "DateTimeBirth":
                    AllParticipants.CustomSort = (isAscendingCol)?(IComparer)new ParticipantDateTimeBirthSorter(): new ParticipantDateTimeBirthSortDesc();
                    break;
                case "RegisteredAt":
                    AllParticipants.CustomSort = (isAscendingCol)?(IComparer)new ParticipantRegisteredAtSorter(): new ParticipantRegisteredAtSortDesc();
                    break;
                case "UserMarkedFinished":
                    AllParticipants.CustomSort = (isAscendingCol) ? (IComparer)new ParticipantMarkedFinishedSorter() : new ParticipantMarkedFinishedSortDesc();
                    break;
            }
        }
        IEnumerable<DataRequiredOption>_selectedDataRequired;
        IEnumerable<StudyCentreModel> _selectedCentres;
        bool _groupByDataRequired;
        public bool GroupByDataRequired { 
            get 
            { 
                return _groupByDataRequired; 
            } 
            set 
            {
                if (value == _groupByDataRequired) { return; }
                _groupByDataRequired = value;
                if (_groupByDataRequired)
                {
                    var vm = new SelectDataRequiredOptionsViewModel(_repository, _selectedDataRequired, _selectedCentres);
                    var win = new SelectDataRequiredOptionsView();
                    win.DataContext = vm;
                    EventHandler handler = null;
                    handler = delegate
                    {
                        win.Close();
                        vm.RequestClose -= handler;
                    };
                    vm.RequestClose += handler;
                    win.ShowDialog();
                    if (vm.WasCancelled)
                    {
                        _groupByDataRequired = false;
                        return;
                    }
                    _selectedDataRequired = vm.SelectedDataRequired.ToList();
                    _selectedCentres = vm.SelectedCentres.ToList();
                    if (_selectedDataRequired.Any())
                    {
                        AllParticipants.GroupDescriptions.Add(new PropertyGroupDescription("DataRequiredString"));
                    }
                    if (_repository.LocalStudyCentres.Skip(1).Any())
                    {
                        AllParticipants.GroupDescriptions.Add(new PropertyGroupDescription("StudyCentre", new StudyCentreModelToNameConverter()));
                    }
                }
                else
                {
                    AllParticipants.GroupDescriptions.Clear();
                }
                SetFilter();
                NotifyPropertyChanged("GroupByDataRequired");
            } 
        }
        public RelayCommand ShowUpdateEnrolment { get; private set; }
        void ShowEnrolDetails(object param)
        {
            var window = new PatientDemographicUpdateView();
            var model =  Mapper.Map<PatientDemographicsModel>(_repository.FindParticipantAndVaccines(SelectedParticipant.Id));
            model.StudyCentre = SelectedParticipant.StudyCentre;
            var vm = new PatientDemographicsViewModel(_repository, model);
            window.DataContext = vm;
            EventHandler enrolCloseHandler = null;
            enrolCloseHandler = delegate
            {
                window.Close();
                vm.RequestClose -= enrolCloseHandler;
            };

            vm.RequestClose += enrolCloseHandler;
            window.ShowDialog();
        }

        static void UpdateDemographics(Participant p, ParticipantListItemViewModel vm)
        {
            vm.AgeDays = (DateTime.Now - p.DateTimeBirth).Days;

            //to do - inform ageUpdatingService
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
        void HandleParticipantUpdate(object sender, ParticipantEventArgs e)
        {
            var assdVM = ((List<ParticipantListItemViewModel>)AllParticipants.SourceCollection)
                .First(p=>p.Id == e.Participant.Id);
            AllParticipants.EditItem(assdVM);
            UpdateDemographics(e.Participant, assdVM);
            assdVM.VaccinesAdministered = e.Participant.VaccinesAdministered;
            
            if (_updateWindow != null && ((ParticipantProgressViewModel)_updateWindow.DataContext).Id == assdVM.Id)
            {
                ParticipantProgressViewModel sp = (ParticipantProgressViewModel)_updateWindow.DataContext;
                UpdateDemographics(e.Participant, sp);
                sp.OutcomeAt28Days = e.Participant.OutcomeAt28Days;
                sp.CauseOfDeath = e.Participant.CauseOfDeath;
                sp.DeathOrLastContactDateTime = e.Participant.DeathOrLastContactDateTime;
                sp.DischargeDateTime = e.Participant.DischargeDateTime;
                sp.LastContactWeight = e.Participant.LastContactWeight;
                sp.LastWeightDate = e.Participant.LastWeightDate;
                sp.OtherCauseOfDeathDetail = e.Participant.OtherCauseOfDeathDetail;
                sp.BcgAdverse = e.Participant.BcgAdverse;
                sp.BcgAdverseDetail = e.Participant.BcgAdverseDetail;
                sp.BcgPapuleAtDischarge = e.Participant.BcgPapuleAtDischarge;
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

        void OnListItemChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UserMarkedFinished")
            {
                var p = (ParticipantListItemViewModel)sender;
                _repository.UpdateParticipant(p.Id, p.UserMarkedFinished);
            }
        }

        private void OnParticipantAdded(object sender, ParticipantEventArgs e)
        {
            var partBase = ParticipantBaseMap(e.Participant);
            partBase.RecalculateDataRequired();
            partBase.StudyCentre = _repository.FindStudyCentre(partBase.CentreId);
            var viewModel = new ParticipantListItemViewModel(partBase, _repository);
            _ageUpdater.AddParticipant(viewModel);
            AllParticipants.AddNewItem(viewModel);
            AllParticipants.CommitNew();
            SetDisplayName();
        }

        void OnUpdateWindow_Closed(object sender, EventArgs e)
        {
            _updateWindow.Closed -= OnUpdateWindow_Closed;
            _updateWindow = null;
        }

        void OnMainWindowClosing(object args)
        {
            if (_updateWindow == null) { return; }
            if (_updateWindow != null && _updateWindow.DataContext != null && ((ParticipantProgressViewModel)_updateWindow.DataContext).OkToProceed())
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
                return _participantBaseMap ?? (_participantBaseMap = GetParticipantBaseMapExpression().Compile());
            }
        }
        static Expression<Func<Participant, ParticipantBaseModel>> GetParticipantBaseMapExpression()
        {
            return p => new ParticipantBaseModel
                         {
                             CentreId = p.CentreId,
                             HospitalIdentifier = p.HospitalIdentifier,
                             DateTimeBirth = p.DateTimeBirth,
                             Id = p.Id,
                             TrialArm = p.TrialArm,
                             IsMale = p.IsMale,
                             Name = p.Name,
                             RegisteredAt = p.RegisteredAt,
                             OutcomeAt28Days = p.OutcomeAt28Days,
                             DischargeDateTime = p.DischargeDateTime,
                             DeathOrLastContactDateTime = p.DeathOrLastContactDateTime,
                             CauseOfDeath = p.CauseOfDeath,
                             VaccinesAdministered = p.VaccinesAdministered,
                             UserMarkedFinished = p.UserMarkedFinished
                         };
        }
        #endregion // Events

        #region ICleanup Implementation
        public void Cleanup()
        {
            _ageUpdater.Cleanup();
            if (_deferredAction != null) { _deferredAction.Dispose(); }
        }
        #endregion

        #region  finalizer
        ~AllParticipantsViewModel()
        {
            _repository.ParticipantAdded -= OnParticipantAdded;
            _repository.ParticipantUpdated -= HandleParticipantUpdate;
            
            Cleanup();
            Mediator.Unregister("MainWindowClosing", OnMainWindowClosing);
        }

        #endregion // finalizer
    }
}