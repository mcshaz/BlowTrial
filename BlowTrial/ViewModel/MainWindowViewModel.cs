using BlowTrial.Domain.Providers;
using BlowTrial.Helpers;
using BlowTrial.Infrastructure;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using BlowTrial.Properties;
using BlowTrial.Security;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using AutoMapper;
using System.Windows;


namespace BlowTrial.ViewModel
{
    /// <summary>
    /// The ViewModel for the application's main window.
    /// </summary>
    public class MainWindowViewModel : WorkspaceViewModel, IDisposable
    {
        #region Fields

        ReadOnlyCollection<CommandViewModel> _commands;
        ObservableCollection<WorkspaceViewModel> _workspaces;
        BackupService _backupService;

        #endregion // Fields

        #region Constructor

        public MainWindowViewModel() : this(new Repository(()=>new TrialDataContext())) { }
        public MainWindowViewModel(IRepository repository) : base(repository)
        {
            this.Version = BlowTrial.App.GetClickOnceVersion() ?? "Development Version";
            ShowCloudDirectoryCmd = new RelayCommand(param => ShowCloudDirectory(), param => IsAuthorised);
            ShowSiteSettingsCmd = new RelayCommand(param => ShowSiteSettings(), param => _backupService != null && _backupService.IsToBackup);
            LogoutCmd = new RelayCommand(param => Logout(), Param => IsAuthorised);
            ShowCreateCsvCmd = new RelayCommand(param => showCreateCsv(), param => IsAuthorised);
            CreateNewUserCmd = new RelayCommand(param => ShowCreateNewUser(), param=>IsAuthorised);
            bool isEnvelopeRandomising = BlowTrialDataService.IsEnvelopeRandomising();
            StopEnvelopeCmd = new RelayCommand(param => StopEnvelopeRandomising(), param => IsAuthorised && isEnvelopeRandomising);
            ShowLogin();
        }

        #endregion // Constructor

        #region Properties
        public string ProjectName
        {
            get { return Strings.Blowtrial_ProjectName; }
        }

        public String Version { get; private set; }
        #endregion // Properties

        #region Commands

        /// <summary>
        /// Returns a read-only list of commands 
        /// that the UI can display and execute.
        /// </summary>
        public ReadOnlyCollection<CommandViewModel> Commands
        {
            get
            {
                if (_commands == null)
                {
                    List<CommandViewModel> cmds = this.CreateCommands();
                    _commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return _commands;
            }
        }

        List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>
            {
                new CommandViewModel(
                    Strings.MainWindowViewModel_Command_ViewParticipants,
                    new RelayCommand(param => this.ShowAllParticipants(), param => IsAuthorised)),

                new CommandViewModel(
                    Strings.MainWindowViewModel_Command_RegisterNewPatient,
                    new RelayCommand(param => this.RegisterNewPatient(), param => IsAuthorised)),

                new CommandViewModel(
                    Strings.MainWindowViewModel_Command_ViewScreenedPatients,
                    new RelayCommand(param => this.ShowScreenedPatients(), param => IsAuthorised)),

                new CommandViewModel(
                    Strings.MainWindowViewModel_Command_ViewSummary,
                    new RelayCommand(param => this.ShowSummaryData(), param => IsAuthorised)),

                new CommandViewModel(
                    Strings.MainWindowViewModel_Command_ViewProtocolViolations,
                    new RelayCommand(param => this.ShowViolations(), param => IsAuthorised))

            };
        }
        public RelayCommand ShowCloudDirectoryCmd {get; private set;}
        public RelayCommand ShowSiteSettingsCmd { get; private set; }
        public RelayCommand ShowCreateCsvCmd { get; private set; }
        public RelayCommand LogoutCmd { get; private set; }
        public RelayCommand CreateNewUserCmd { get; private set; }
        public RelayCommand StopEnvelopeCmd { get; private set; }

        void StopEnvelopeRandomising()
        {
            var result = MessageBox.Show(Strings.MainWindow_StopEnvelopeRandomisingMsg,Strings.MainWindow_StopEnvelopeRandomisingCaption, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.OK)
            {
                BlowTrialDataService.StopEnvelopeRandomising();
                RandomisingEngine.ResetBlock(EnvelopeDetails.FirstAvailableBlockNumber, _repository);
                StopEnvelopeCmd = new RelayCommand(param => StopEnvelopeRandomising(), param => false);
            }
        }
        #endregion // Commands

        #region Workspaces

        /// <summary>
        /// Returns the collection of available workspaces to display.
        /// A 'workspace' is a ViewModel that can request to be closed.
        /// </summary>
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get 
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _workspaces.CollectionChanged += this.OnWorkspacesChanged;
                }
                return _workspaces;
            }
        }

        #endregion // Workspaces

        #region Private Helpers
        void Logout()
        {

            CustomPrincipal customPrincipal = (CustomPrincipal)GetCurrentPrincipal();
            if (customPrincipal != null)
            {
                customPrincipal.Identity = new AnonymousIdentity();
            }
            IsAuthorised = false;
            for (int i = Workspaces.Count; --i >= 0; )
            {
                var ws = (WorkspaceViewModel)Workspaces[i];
                Workspaces.Remove(ws);
            }
            _backupService = null;
            ShowLogin();
        }

        void RegisterNewPatient()
        {
            NewPatientViewModel newPatientVM = (NewPatientViewModel)Workspaces.FirstOrDefault(w => w is NewPatientViewModel);
            if (newPatientVM==null)
            {
                var newPatient = new NewPatientModel();
                newPatientVM = new NewPatientViewModel(_repository, newPatient);
                this.Workspaces.Add(newPatientVM);
            }
            this.SetActiveWorkspace(newPatientVM);
            base.DisplayName = Strings.MainWindowViewModel_Command_RegisterNewPatient;
        }
        void ShowCreateNewUser()
        {
            IsAuthorised = false;
            var allParticipantsVM = new CreateNewUserViewModel(new MembershipContext());
            allParticipantsVM.ChangeToThisUserOnSave = GetCurrentPrincipal().Identity.Name == "Admin";
            this.Workspaces.Add(allParticipantsVM);
            this.SetActiveWorkspace(allParticipantsVM);
            base.DisplayName = allParticipantsVM.DisplayName;
        }

        void ShowAllParticipants()
        {
            AllParticipantsViewModel allParticipantsVM = (AllParticipantsViewModel)Workspaces.FirstOrDefault(w => w is AllParticipantsViewModel);
            if (allParticipantsVM == null)
            {
                allParticipantsVM = new AllParticipantsViewModel(_repository);
                this.Workspaces.Add(allParticipantsVM);
            }
            this.SetActiveWorkspace(allParticipantsVM);
            base.DisplayName = Strings.MainWindowViewModel_Command_ViewParticipants;
        }

        void ShowScreenedPatients()
        {
            AllScreenedViewModel allScreenedVM = (AllScreenedViewModel)Workspaces.FirstOrDefault(w => w is AllScreenedViewModel);
            if (allScreenedVM == null)
            {
                allScreenedVM = new AllScreenedViewModel(_repository);
                this.Workspaces.Add(allScreenedVM);
            }
            this.SetActiveWorkspace(allScreenedVM);
            base.DisplayName = Strings.MainWindowViewModel_Command_ViewScreenedPatients;
        }
        void ShowSummaryData()
        {
            DataSummaryViewModel summaryVM = (DataSummaryViewModel)Workspaces.FirstOrDefault(w => w is DataSummaryViewModel);
            if (summaryVM == null)
            {
                summaryVM = new DataSummaryViewModel(_repository);
                this.Workspaces.Add(summaryVM);
            }
            this.SetActiveWorkspace(summaryVM);
            base.DisplayName = Strings.MainWindowViewModel_Command_ViewSummary;
        }
        void ShowViolations()
        {
            AllViolationsViewModel violVM = (AllViolationsViewModel)Workspaces.FirstOrDefault(w => w is AllViolationsViewModel);
            if (violVM == null)
            {
                violVM = new AllViolationsViewModel(_repository);
                this.Workspaces.Add(violVM);
            }
            this.SetActiveWorkspace(violVM);
            base.DisplayName = Strings.MainWindowViewModel_Command_ViewProtocolViolations;
        }
        void showCreateCsv()
        {
            var csvVM = (CreateCsvViewModel)Workspaces.FirstOrDefault(w => w is CreateCsvViewModel);
            if (csvVM == null)
            {
                csvVM =
                    new CreateCsvViewModel
                        (_repository,
                        new CreateCsvModel());

                this.Workspaces.Add(csvVM);
            }
            this.SetActiveWorkspace(csvVM);
            base.DisplayName = Strings.CreateCsvVM_Title;
        }
        void ShowSiteSettings()
        {
            var siteSetVM = (StudySitesViewModel)Workspaces.FirstOrDefault(w => w is StudySitesViewModel);
            if (siteSetVM == null)
            {
                siteSetVM =
                    new StudySitesViewModel
                        (new StudySitesModel(Mapper.Map<IEnumerable<StudySiteItemModel>>(_repository.LocalStudyCentres)), _repository);
                
                this.Workspaces.Add(siteSetVM);
            }
            this.SetActiveWorkspace(siteSetVM);
            base.DisplayName = Strings.CloudDirectoryVm_SelectDir;
        }
        void ShowCloudDirectory()
        {
            var cloudVM = (CloudDirectoryViewModel)Workspaces.FirstOrDefault(w => w is CloudDirectoryViewModel);
            if (cloudVM==null)
            {
                cloudVM =
                    new CloudDirectoryViewModel
                        (new CloudDirectoryModel
                            {
                                 CloudDirectories = _repository.CloudDirectories,
                                 BackupIntervalMinutes = _backupService.IntervalMins,
                                 IsBackingUpToCloud = _backupService.IsToBackup
                            }
                        );

                this.Workspaces.Add(cloudVM);
            }
            this.SetActiveWorkspace(cloudVM);
            base.DisplayName = Strings.CloudDirectoryVm_SelectDir;
        }

        void ShowLogin()
        {
            if (Workspaces.Any(w => w is AuthenticationViewModel)) { return; }
            AuthenticationViewModel loginVM = new AuthenticationViewModel(new AuthenticationService());
            this.Workspaces.Add(loginVM);
            this.SetActiveWorkspace(loginVM);
            base.DisplayName = Strings.MainWindowViewModel_Command_Login;
        }

        void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(this.Workspaces.Contains(workspace));
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
            if (collectionView != null)
            {
                collectionView.MoveCurrentTo(workspace);
            }
        }

        private bool _isAuthorised;
        public bool IsAuthorised { 
            get { return _isAuthorised; }
            set
            {
                if (value == _isAuthorised) { return; }
                _isAuthorised = value;
                NotifyPropertyChanged("IsAuthorised");
            }
        }
        
        void HandleAuthorisationClose()
        {
            var identity = GetCurrentPrincipal().Identity;
            if (identity.Name == "Admin")
            {
                ShowCreateNewUser();
            }
            else
            {
                IsAuthorised = identity.IsAuthenticated;
                if (IsAuthorised)
                {
                    using (var m = new MembershipContext())
                    {
                        _backupService = new BackupService(_repository, m);
                    }
                }
                else
                {
                    OnRequestClose();
                }
            }
        }
        void HandleCreateNewUserClose(CreateNewUserViewModel vm)
        {
            vm.MembershipContext.Dispose();
            var identity = GetCurrentPrincipal().Identity;
            if (identity == null || identity.IsAuthenticated==false || identity.Name=="Admin")
            {
                Logout();
            }
            else if (vm.ChangeToThisUserOnSave)
            {
                HandleAuthorisationClose();
            }
            else
            {
                IsAuthorised = true;
            }
        }
        void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }

        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            var senderType = sender.GetType();
            WorkspaceViewModel workspace = (WorkspaceViewModel)sender;
            this.Workspaces.Remove(workspace);
            if (senderType == typeof(AuthenticationViewModel)) { HandleAuthorisationClose(); }
            else if (senderType == typeof(CreateNewUserViewModel)) { HandleCreateNewUserClose((CreateNewUserViewModel)workspace); }
            if (Workspaces.Any())
            {
                ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);

                DisplayName = ((ViewModelBase)collectionView.CurrentItem).DisplayName;
            }
            else
            {
                DisplayName = Strings.MainWindowViewModel_WorkspaceName;
            }
        }
        #endregion // Private Helpers

        #region Event Handlers
        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Mediator.NotifyColleagues("MainWindowClosing", e);
        }
        #endregion

        #region IDisposable implementation
        //http://msdn.microsoft.com/en-us/library/vstudio/b1yfkh5e%28v=vs.100%29.aspx
        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }
        ~MainWindowViewModel()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_backupService != null)
                {
                    _backupService.Cleanup();
                }
                if (disposing)
                {
                    if (_repository != null) { _repository.Dispose(); }
                }
                // Indicate that the instance has been disposed.
                _repository = null;
                _disposed = true;
            }
        }
        #endregion // IDiposable
    }
}