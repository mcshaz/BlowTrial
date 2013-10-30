using BlowTrial.Infrastructure.Interfaces;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace BlowTrial.ViewModel
{
    public abstract class ClosableWindowViewModel : DataRepositoryViewModel
    {
        #region constructor
        protected ClosableWindowViewModel(IRepository repository) : base(repository)
        {
        }
        #endregion

        #region closeCmd
        protected RelayCommand CloseCommand { get; set; }

        void Close_Execute(object parameter)
        {
            //Close this window
            CloseWindow();
        }
        #endregion

        #region CloseWindowFlag
        bool? _CloseWindowFlag;
        public bool? CloseWindowFlag
        {
            get { return _CloseWindowFlag; }
            set
            {
                _CloseWindowFlag = value;
                NotifyPropertyChanged("CloseWindowFlag");
            }
        }

        public virtual void CloseWindow(bool? result = true)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                CloseWindowFlag = CloseWindowFlag == null
                    ? true
                    : !CloseWindowFlag;
            }));
        }
        #endregion
    }
}
