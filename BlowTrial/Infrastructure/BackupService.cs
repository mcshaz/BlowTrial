using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Threading;

namespace BlowTrial.Infrastructure
{
    public class BackupService
    {
        #region Constructors
        private const int millisecsPerMin = 1000*60;
        public BackupService(IRepository repo, int intervalMins, bool isToBackup)
        {
            _repo = repo;
            //use DispatcherTimer rather than dispatcher as CE does not handle multiple connections inevitable with multi threading
            _timer = new DispatcherTimer(DispatcherPriority.Normal);
            _timer.Interval = new TimeSpan(0, intervalMins, 0);
            if (isToBackup)
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
        public string Directory 
        { 
            get
            {
                return _repo.BackupDirectory;
            }
            set
            {
                _repo.BackupDirectory = value;
            }
        }
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

        #region finalizer

        ~BackupService()
        {
            if (_isToBackup)
            {
                Backup(this, new EventArgs());
            }
            _timer.Tick -= _handler;
            _timer.Stop(); 
        }
        #endregion // IDiposable
    }
}
