using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudFileTransfer;
using System.IO;
using System.Windows;

namespace BlowTrial.ViewModel
{
    public class RequestReverseUpdateViewModel : WorkspaceViewModel
    {
        string _logFileName;
        BlowTrial.View.PleaseWait _pleaseWait;
        public RequestReverseUpdateViewModel(IRepository repository, string logFileName)
            : base(repository)
        { 
            Sites = repository.GetFilenamesAndCentres().Select(d=>
                new UpdateItemVM
                {
                     Directory = Path.GetDirectoryName(d.Key),
                     DisplayName = string.Join(",",d.Value.Select(c=>c.Name)),
                     Models = d.Value
                });
            SaveCmd = new RelayCommand(Save, o => SelectedSite != null);
            _logFileName = logFileName;
        }
        public UpdateItemVM SelectedSite { get; set; }
        public IEnumerable<UpdateItemVM> Sites { get; private set; }

        
        public RelayCommand SaveCmd { get; private set; }
        void Save(object o)
        {
            TransferFile.RequestUpdate(new TransferLog(Path.Combine(SelectedSite.Directory, _logFileName)), SelectedSite.Models.First().DuplicateIdCheck, CreateFileForTransfer, 200,  OnFinishedTransfer);
            MessageBox.Show("A request to update has been sent. The time taken to acknowledge such a request will depend on whether the trial site has their BLOW-trial application running, and how often the application is backing up to the cloud. You will be notified when the update is occuring. Please do not close or log out until this notification has occured.");
            OnRequestClose();
        }
        string CreateFileForTransfer()
        {
            _pleaseWait = new BlowTrial.View.PleaseWait();
            _pleaseWait.Show();
            string newFilepath = _repository.BackupLimitedDbTo(SelectedSite.Directory, SelectedSite.Models.ToArray());
            return Path.GetFileName(newFilepath);
        }
        void OnFinishedTransfer()
        {
            _pleaseWait.Close();
            MessageBox.Show("Update to site database successful");
        }
    }

    public class UpdateItemVM
    {
        public string Directory {get; set;}
        public string DisplayName {get; set;}
        public IEnumerable<StudyCentreModel> Models {get; set;}
    }

}
