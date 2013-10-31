using BlowTrial.Domain.Interfaces;
using BlowTrial.Domain.Providers;
using BlowTrial.Infrastructure.Interfaces;
using MvvmExtraLite.Helpers;
using System;
using System.Security.Principal;
using System.Windows.Input;

namespace BlowTrial.ViewModel
{
    /// <summary>
    /// This ViewModelBase subclass requests to be removed 
    /// from the UI when its CloseCmd executes.
    /// This class is abstract.
    /// </summary>
    public abstract class WorkspaceViewModel: ViewModelBase
    {
        #region Fields
        protected IRepository _repository;
        RelayCommand _closeCmd;
         #endregion

        #region Constructor

        protected WorkspaceViewModel(IRepository repository)
        {
            if (repository==null)
            {
                throw new ArgumentException();
            }
            _repository = repository;
        }

        protected WorkspaceViewModel() { }

        #endregion // Constructor

        #region Properties
        #endregion // Properties

        #region Methods

        #endregion

        #region CloseCmd

        /// <summary>
        /// Returns the command that, when invoked, attempts
        /// to remove this workspace from the user interface.
        /// </summary>
        public virtual ICommand CloseCmd
        {
            get
            {
                if (_closeCmd == null)
                    _closeCmd = new RelayCommand(param => OnRequestClose());

                return _closeCmd;
            }
        }

        #endregion // CloseCmd

        #region RequestClose [event]

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;
        protected void OnRequestClose()
        {
            if (RequestClose != null)
            {
                RequestClose(this, EventArgs.Empty);
            }
        }

        #endregion // RequestClose [event]

    }
}