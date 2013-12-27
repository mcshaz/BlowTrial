using BlowTrial.Helpers;
using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Threading;

namespace BlowTrial.Infrastructure
{
    public class BackupService : ICleanup
    {
        #region Constructors
        private const int millisecsPerMin = 1000*60;
        public BackupService(IRepository repo, IBackupData backupData)
        {
            _repo = repo;
            var backupDetails = BlowTrialDataService.GetBackupDetails(backupData);
            _repo.CloudDirectories = backupDetails.CloudDirectories;
            //use DispatcherTimer rather than dispatcher as CE does not handle multiple connections inevitable with multi threading
            _timer = new DispatcherTimer(DispatcherPriority.Normal);
            _timer.Interval = new TimeSpan(0, backupDetails.BackupData.BackupIntervalMinutes, 0);
            if (backupDetails.BackupData.IsBackingUpToCloud)
            {
                _handler = new EventHandler(Backup);
                _isToBackup = true;
            }
            else
            {
                _handler += new EventHandler(Restore);
                Restore(this, new EventArgs());
            }
            _timer.Tick += _handler;
            _timer.Start();
        }

        #endregion

        #region fields
        readonly IRepository _repo;
        readonly DispatcherTimer _timer;
        readonly EventHandler _handler;
        readonly bool _isToBackup;
        #endregion

        #region Properties
        public int IntervalMins
        {
            get
            {
                return (int)_timer.Interval.TotalMinutes;
            }
            set
            {
                _timer.Interval = new TimeSpan(0,value,0);
            }
        }
        public bool IsToBackup
        {
            get { return _isToBackup; }
        }
        #endregion

        #region Methods
        private void Backup(object sender, EventArgs e)
        {
            _repo.Backup();
        }
        private void Restore(object sender, EventArgs e)
        {
            _repo.Restore();
        }
        #endregion

        #region ICleanup implementation
        bool _isCleanedup;
        public void Cleanup()
        {
            if (!_isCleanedup)
            {
                _timer.Tick -= _handler;
                _timer.Stop();
                if (_isToBackup)
                {
                    Backup(this, new EventArgs());
                }
                _isCleanedup = true;
            }
        }
        #endregion
    }
}
