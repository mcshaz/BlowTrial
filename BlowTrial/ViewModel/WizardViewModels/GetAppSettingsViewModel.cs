using BlowTrial.Models;
using BlowTrial.Properties;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace BlowTrial.ViewModel
{
    public sealed class GetAppSettingsViewModel : ViewModelBase
    {
        #region fields


        #endregion

        #region constructors
        public GetAppSettingsViewModel()
        {
            BackupVM = new BackupDirectionViewModel();
            CloudVM = new CloudDirectoryViewModel(new CloudDirectoryModel());
            StudySitesVM = new StudySitesViewModel();
            Finish = new RelayCommand(OnFinish);
            PageChanged = new RelayCommand(OnPageChange);
        }
        #endregion

        #region properties
        public BackupDirectionViewModel BackupVM { get; private set; }
        public CloudDirectoryViewModel CloudVM { get; private set; }
        public StudySitesViewModel StudySitesVM { get; private set; }
        public ICommand Finish { get; private set; }
        public ICommand PageChanged { get; private set; }
        public bool IsValid
        {
            get
            {
                if (!BackupVM.IsValid || !CloudVM.IsValid) { return false; }
                if (BackupVM.BackupToCloud == true)
                {
                    return StudySitesVM.IsValid;
                }
                return true;
            }
        }
        #endregion
        #region methods
        void OnFinish(object param)
        {

        }
        //Hack alert - should come up with something friendlier!
        void OnPageChange(object param)
        {

        }
        #endregion

    }

}
