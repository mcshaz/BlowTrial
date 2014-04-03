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
using CloudFileTransfer;
using System.IO;
using BlowTrial.Infrastructure.Extensions;


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
        TransferLog _log;

        #endregion // Fields

        #region Constructor

        public MainWindowViewModel() : this(new Repository(()=>new TrialDataContext())) { }
        public MainWindowViewModel(IRepository repository) : base(repository)
        {
            this.Version = BlowTrial.App.CurrentClickOnceVersion ?? ("Development Version: " + App.CurrentAppVersion.ToVersionString());
            ShowCloudDirectoryCmd = new RelayCommand(param => ShowCloudDirectory(), param => IsAuthorised);
            ShowSiteSettingsCmd = new RelayCommand(param => ShowSiteSettings(), param => _backupService != null && _backupService.IsToBackup);
            LogoutCmd = new RelayCommand(param => Logout(), Param => IsAuthorised);
            ShowCreateCsvCmd = new RelayCommand(param => showCreateCsv(), param => IsAuthorised);
            CreateNewUserCmd = new RelayCommand(param => ShowCreateNewUser(), param=>IsAuthorised);
            ToggleEnvelopeCmd = new RelayCommand(param => ToggleEnvelopeRandomising(), param => IsAuthorised);
            ShowRandomisingMessagesCmd = new RelayCommand(param => ShowRandomisingMessages(), param => IsAuthorised);
            RequestReverseUpdateCmd = new RelayCommand(param => ShowRequestReverseUpdate(), param => _backupService != null && !_backupService.IsToBackup);
            ShowLogin();
        }

        #endregion // Constructor

        #region Properties
        public string ProjectName
        {
            get { return Strings.Blowtrial_ProjectName; }
        }
        bool _isEnvelopeRandomising;
        bool IsEnvelopeRandomising { 
            get 
            { 
                return _isEnvelopeRandomising; 
            } 
            set
            {
                if (value == _isEnvelopeRandomising) { return; }
                _isEnvelopeRandomising = value;
                NotifyPropertyChanged("ToggleEnvelopeString");
            }
        }
        public string Version { get; private set; }
        public string ToggleEnvelopeString
        {
            get
            { 
                return IsEnvelopeRandomising?Strings.MainWindow_StopEnvelopeRandomisingCaption:Strings.MainWindow_StopComputerRandomisingCaption;
            }
        }
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
        public RelayCommand ToggleEnvelopeCmd { get; private set; }
        public RelayCommand ShowRandomisingMessagesCmd { get; private set; }
        public RelayCommand RequestReverseUpdateCmd { get; private set; }

        void ToggleEnvelopeRandomising()
        {
            //var result = MessageBox.Show(Strings.MainWindow_StopEnvelopeRandomisingMsg,Strings.MainWindow_StopEnvelopeRandomisingCaption, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
           // if (result == MessageBoxResult.OK)
            var result = MessageBox.Show("If you continue, the new patient window will be closed and any data lost", "Change Envelope randomising status", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel) { return; }
            WorkspaceViewModel newPatientVM = Workspaces.FirstOrDefault(w => w is PatientDemographicsViewModel);
            if (newPatientVM != null)
            {
                newPatientVM.CloseCmd.Execute(null);
            }
            IsEnvelopeRandomising = !IsEnvelopeRandomising;
            BlowTrialDataService.ChangeEnvelopeRandomising(IsEnvelopeRandomising);
            if (IsEnvelopeRandomising)
            {
                RandomisingEngine.UnsetComputerisedBlocks(_repository);
            }
            else
            {
                RandomisingEngine.BalanceUnsetBlocks(_repository);
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
            if (IsReplaceDbRequest())
            {
                OnRequestClose();
            }
            else
            {
                Cleanup();
                ShowLogin();
            }
        }
        void Cleanup()
        {
            if (_backupService != null)
            {
                _backupService.OnBackup -= OnBackupInterval;
                _backupService.Cleanup();
                _backupService = null;
            }
            var allPart = (AllParticipantsViewModel)Workspaces.FirstOrDefault(w=>w is AllParticipantsViewModel);
            if (allPart != null)
            {
                allPart.Cleanup();
            }
            _log = null;
        }
        bool IsReplaceDbRequest()
        {
            return _log != null && _log.UpdateIsRequested(_repository.LocalStudyCentres.First().DuplicateIdCheck);
        }
        void ReplaceDb()
        {
            var pw = new BlowTrial.View.PleaseWait();
            pw.Show();
            string currentPath = TrialDataContext.GetDbPath();
            if (_repository != null)
            {
                _repository.Dispose();
                _repository = null;
            }
            System.IO.File.Move(currentPath, currentPath.InsertDateStampToFileName());
            TransferFile.AllowUpdate(_log, fi =>
                {
                    fi.MoveTo(currentPath);
                });
            pw.Close();
        }


        void RegisterNewPatient()
        {
            PatientDemographicsViewModel newPatientVM = (PatientDemographicsViewModel)Workspaces.FirstOrDefault(w => w is PatientDemographicsViewModel);
            if (newPatientVM==null)
            {
                var newPatient = new PatientDemographicsModel { WasEnvelopeRandomised =  IsEnvelopeRandomising};
                newPatientVM = new PatientDemographicsViewModel(_repository, newPatient);
                this.Workspaces.Add(newPatientVM);
            }
            this.SetActiveWorkspace(newPatientVM);
        }

        void ShowCreateNewUser()
        {
            IsAuthorised = false;
            var allParticipantsVM = new CreateNewUserViewModel(new MembershipContext());
            allParticipantsVM.ChangeToThisUserOnSave = GetCurrentPrincipal().Identity.Name == "Admin";
            this.Workspaces.Add(allParticipantsVM);
            this.SetActiveWorkspace(allParticipantsVM);
        }

        void ShowRequestReverseUpdate()
        {
            RequestReverseUpdateViewModel updateVm = new RequestReverseUpdateViewModel(_repository, LogFileName);
            var updateWindow = new BlowTrial.View.RequestReverseUpdateView();
            EventHandler onRequestClose = null;
            updateVm.RequestClose += onRequestClose = (o, e) =>
            {
                updateWindow.Close();
                updateVm.RequestClose -= onRequestClose;
                onRequestClose = null;
            };

            updateWindow.DataContext = updateVm;
            updateWindow.ShowDialog();
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
        }

        void ShowRandomisingMessages()
        {
            var messagesVM = (RandomisedMessagesViewModel)Workspaces.FirstOrDefault(w => w is RandomisedMessagesViewModel);
            if (messagesVM == null)
            {
                var messages = BlowTrialDataService.GetRandomisingMessage();
                RandomisedMessagesModel model = (messages==null)
                    ? new RandomisedMessagesModel()
                    : Mapper.Map<RandomisedMessagesModel>(messages);
                messagesVM = new RandomisedMessagesViewModel(model);

                this.Workspaces.Add(messagesVM);
            }
            this.SetActiveWorkspace(messagesVM);
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
                cloudVM.OnSave += CloudVmSaved;
                this.Workspaces.Add(cloudVM);
            }
            this.SetActiveWorkspace(cloudVM);
        }
        void CloudVmSaved(object sender, EventArgs e)
        {
            var CloudModel = ((CloudDirectoryViewModel)sender).CloudModel;
            _repository.CloudDirectories = CloudModel.CloudDirectories;
            _backupService.IntervalMins = CloudModel.BackupIntervalMinutes.Value;
        }
        void ShowLogin()
        {
            if (Workspaces.Any(w => w is AuthenticationViewModel)) { return; }
            AuthenticationViewModel loginVM = new AuthenticationViewModel(new AuthenticationService());
            this.Workspaces.Add(loginVM);
            this.SetActiveWorkspace(loginVM);
        }

        void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(this.Workspaces.Contains(workspace));
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
            if (collectionView != null)
            {
                collectionView.MoveCurrentTo(workspace);
            }
            base.DisplayName = workspace.DisplayName;
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
        internal const string LogFileName = "SyncRequestLog.csv";
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
                        IsEnvelopeRandomising = BlowTrialDataService.IsEnvelopeRandomising(m);
                        var backDetails = BlowTrialDataService.GetBackupDetails(m);
                        if (backDetails.BackupData.IsBackingUpToCloud)
                        {
                            _log = new TransferLog(backDetails.CloudDirectories.First() + '\\' + LogFileName);
                            _backupService.OnBackup += OnBackupInterval;
                        }
                    }
                }
                else
                {
                    OnRequestClose();
                }
            }
        }

        void OnBackupInterval(object sender, EventArgs e)
        {
            if (_log.UpdateIsRequested(_repository.LocalStudyCentres.First().DuplicateIdCheck))
            {
                MessageBox.Show(Strings.BackupService_DBupdateRequestExplanation, Strings.BackupService_DBupdateRequestHeader);
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
            else if (senderType == typeof(CloudDirectoryViewModel)) { ((CloudDirectoryViewModel)sender).OnSave -= CloudVmSaved; }
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
            if (!e.Cancel && IsReplaceDbRequest())
            {
                ReplaceDb();
            }
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
                Cleanup();
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