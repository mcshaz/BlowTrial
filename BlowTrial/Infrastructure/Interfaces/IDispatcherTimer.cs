using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BlowTrial.Infrastructure.Interfaces
{
    public class DispatcherTimerAdaper : DispatcherTimer, IDispatcherTimer 
    { 
        public DispatcherTimerAdaper() : base()
        { }
        public DispatcherTimerAdaper(DispatcherPriority priority) : base(priority)
        { }
    }
    public class MoqTimerEventArgs : EventArgs
    {
        public MoqTimerEventArgs(bool isClockTicking) : this(DateTime.Now, isClockTicking)
        { }
        public MoqTimerEventArgs(DateTime startAt, bool isClockTicking)
        {
            StartAt = startAt;
            _isClockTicking = isClockTicking;
        }
        DateTime _startAt;
        public DateTime StartAt
        {
            get { return _startAt; }
            set 
            { 
                LastNowCall = DateTime.Now;
                _startAt = value;
            }
        }
        readonly bool _isClockTicking;
        public DateTime LastNowCall {get; set;}
        public DateTime Now
        {
            get
            {
                if (_isClockTicking)
                {
                    var now = DateTime.Now;
                    var returnVar = _startAt + (now - LastNowCall);
                    LastNowCall = now;
                    return returnVar;
                }
                else
                {
                    return _startAt;
                }
            }
        }
    }
    public interface IDispatcherTimer
    {
        TimeSpan Interval { get; set; }
        bool IsEnabled { get; set; }
        object Tag { get; set; }
        event EventHandler Tick;
        void Start();
        void Stop();
    }
}
