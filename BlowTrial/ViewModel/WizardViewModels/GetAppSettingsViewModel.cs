using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Linq;
using BlowTrial.Models;

namespace BlowTrial.ViewModel
{
    /// <summary>
    /// The main ViewModel class for the wizard.
    /// This class contains the various pages shown
    /// in the workflow and provides navigation
    /// between the pages.
    /// </summary>
    public class GetAppSettingsViewModel :WorkspaceViewModel
    {
        #region Fields

        RelayCommand _cancelCommand;

        WizardPageViewModel _currentPage;
        RelayCommand _moveNextCommand;
        RelayCommand _movePreviousCommand;
        IList<WizardPageViewModel> _pages;

        StudySitesModel _sitesModel;
        BackupDirectionModel _backupModel;

        #endregion // Fields

        #region Constructor

        public GetAppSettingsViewModel()
        {
            _backupModel = new BackupDirectionModel();
            this.CurrentPage = this.Pages[0];
        }

        #endregion // Constructor

        #region Commands

        #region CancelCommand

        /// <summary>
        /// Returns the command which, when executed, cancels the order 
        /// and causes the Wizard to be removed from the user interface.
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new RelayCommand(param => this.CancelOrder());

                return _cancelCommand;
            }
        }

        void CancelOrder()
        {
                
            base.OnRequestClose();
        }

        #endregion // CancelCommand

        #region MovePreviousCommand

        /// <summary>
        /// Returns the command which, when executed, causes the CurrentPage 
        /// property to reference the previous page in the workflow.
        /// </summary>
        public ICommand MovePreviousCommand
        {
            get
            {
                if (_movePreviousCommand == null)
                    _movePreviousCommand = new RelayCommand(
                        param => this.MoveToPreviousPage(),
                        param => this.CanMoveToPreviousPage);

                return _movePreviousCommand;
            }
        }

        bool CanMoveToPreviousPage
        {
            get { return 0 < this.CurrentPageIndex; }
        }

        void MoveToPreviousPage()
        {
            if (this.CanMoveToPreviousPage)
                this.CurrentPage = this.Pages[this.CurrentPageIndex - 1];
        }

        #endregion // MovePreviousCommand

        #region MoveNextCommand

        /// <summary>
        /// Returns the command which, when executed, causes the CurrentPage 
        /// property to reference the next page in the workflow.  If the user
        /// is viewing the last page in the workflow, this causes the Wizard
        /// to finish and be removed from the user interface.
        /// </summary>
        public ICommand MoveNextCommand
        {
            get
            {
                if (_moveNextCommand == null)
                    _moveNextCommand = new RelayCommand(
                        param => this.MoveToNextPage(),
                        param => this.CanMoveToNextPage);

                return _moveNextCommand;
            }
        }

        bool CanMoveToNextPage
        {
            get { return this.CurrentPage != null && this.CurrentPage.IsValid(); }
        }

        void MoveToNextPage()
        {
            if (this.CanMoveToNextPage)
            {
                if (this.CurrentPageIndex < this.Pages.Count - 1)
                {
                    this.CurrentPage = this.Pages[this.CurrentPageIndex + 1];
                }
                else
                {
                    this.OnRequestClose();
                }
            }
        }

        #endregion // MoveNextCommand

        #endregion // Commands

        #region Properties

        /// <summary>
        /// Returns the page ViewModel that the user is currently viewing.
        /// </summary>
        public WizardPageViewModel CurrentPage
        {
            get { return _currentPage; }
            private set
            {
                if (value == _currentPage)
                    return;

                if (_currentPage != null)
                    _currentPage.IsCurrentPage = false;

                _currentPage = value;

                if (_currentPage != null)
                    _currentPage.IsCurrentPage = true;

                NotifyPropertyChanged("CurrentPage","IsOnLastPage");
            }
        }

        /// <summary>
        /// Returns true if the user is currently viewing the last page 
        /// in the workflow.  This property is used by CoffeeWizardView
        /// to switch the Next button's text to "Finish" when the user
        /// has reached the final page.
        /// </summary>
        public bool IsOnLastPage
        {
            get { return this.CurrentPageIndex == this.Pages.Count - 1; }
        }

        /// <summary>
        /// Returns a read-only collection of all page ViewModels.
        /// </summary>
        public IList<WizardPageViewModel> Pages
        {
            get
            {
                if (_pages == null)
                    this.CreatePages();

                return _pages;
            }
        }

        #endregion // Properties

        #region Private Helpers

        void CreatePages()
        {
            _pages = new List<WizardPageViewModel>();
            
            var backupDirectionVM = new BackupDirectionViewModel(_backupModel);
            backupDirectionVM.PropertyChanged += backupDirVM_PropertyChanged;
            _pages.Add(backupDirectionVM);
            _pages.Add(new CloudDirectoryViewModel(_backupModel));
        }

        void backupDirVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BackupToCloud")
            {
                var backupDirVM = (BackupDirectionViewModel)sender;
                if (backupDirVM.BackupToCloud == null) { return; }
                var studyModel = _pages[_pages.Count-1] as StudySitesViewModel;
                if (backupDirVM.BackupToCloud.Value && studyModel!=null)
                {
                    _pages.Remove(studyModel);
                }
                else if (!backupDirVM.BackupToCloud.Value && studyModel == null)
                {
                    _sitesModel = new StudySitesModel();
                    _pages.Add(new StudySitesViewModel(_sitesModel));
                }
            }

        }

        int CurrentPageIndex
        {
            get
            {

                if (this.CurrentPage == null)
                {
                    Debug.Fail("Why is the current page null?");
                    return -1;
                }

                return this.Pages.IndexOf(this.CurrentPage);
            }
        }

        #endregion // Private Helpers
    }
}
