using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.ViewModel
{
    public abstract class DataRepositoryViewModel : ViewModelBase, IDisposable
    {
        #region Fields
        protected readonly IRepository _repository;
         #endregion
        #region Constructor

        protected DataRepositoryViewModel(IRepository repository)
        {
            if (repository==null)
            {
                throw new ArgumentException();
            }
            _repository = repository;
        }
        protected DataRepositoryViewModel() { }

        #endregion // Constructor

        #region IDisposable Implementation
        //http://msdn.microsoft.com/en-us/library/vstudio/b1yfkh5e%28v=vs.100%29.aspx
        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }
        ~DataRepositoryViewModel()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_repository != null) { _repository.Dispose(); }
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
        #endregion // IDiposable

    }
}
