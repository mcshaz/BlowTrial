using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using BlowTrial.Utilities;

namespace BlowTrial.Infrastructure
{
    public class AgeUpdatingService: ICleanup
    {
        #region fields
        readonly IDispatcherTimer _timer;
        int _nextIndex;
        readonly OrderedList<KeyValuePair<TimeSpan, IBirthday>> _participants;
        readonly TimeSpan _adjustIntoFuture = TimeSpan.FromSeconds(1);
        #endregion

        #region constructor
        public AgeUpdatingService(IEnumerable<IBirthday> participants, DispatcherPriority priority = DispatcherPriority.Normal, int capacity=4)
            : this (participants, new DispatcherTimerAdaper(priority), capacity)
        {
        }
        public AgeUpdatingService(IEnumerable<IBirthday> participants, IDispatcherTimer timer, int capacity=4)
        {
            _timer = timer;
            _timer.Tick += OnTick;
            _participants = new OrderedList<KeyValuePair<TimeSpan, IBirthday>>(capacity, new KeyComparer<TimeSpan, IBirthday>());
            _participants.AddRange(participants.Select(p => new KeyValuePair<TimeSpan, IBirthday>(p.DateTimeBirth.TimeOfDay, p)));
            
            if (_participants.Any())
            {
                TimeSpan nowTime = DateTime.Now.TimeOfDay;
                _nextIndex = _participants.BinarySearch(new KeyValuePair<TimeSpan, IBirthday>(nowTime, null));
                if (_nextIndex < 0)
                {
                    _nextIndex = ~_nextIndex;
                }
                TimeSpan interval;
                if (_nextIndex >= _participants.Count)
                {
                    _nextIndex = 0;
                    interval = TimeSpan.FromDays(1) - nowTime + _participants[0].Key;
                }
                else
                {
                    interval = _participants[_nextIndex].Key - nowTime;
                }
                StartTimer(interval);
            }
        }
        #endregion

        #region Properties
        public TimeSpan NextInterval // purely for testing
        {
            get
            {
                return _timer.Interval;
            }
            
        }
        #endregion

        #region EventHandlers
        void OnTick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
#if DEBUG
            var moqArgs = e as MoqTimerEventArgs;
            if (moqArgs==null) //ie not running a test
            {
                System.Diagnostics.Debug.WriteLine("tick at:{0} interval had been:{1}",now,_timer.Interval);
            }
            else
            {
                now = moqArgs.Now;
            }
            
#endif
            TimeSpan nowTime = now.TimeOfDay.Add(_adjustIntoFuture);
            //if (_participants.Count == 0) { _timer.Stop(); return; }
            do
            {
                if (OnAgeIncrement == null)
                {
                    _nextIndex++;
                }
                else
                {
                    IBirthday p = _participants[_nextIndex].Value;
                    p.AgeDays = (now - p.DateTimeBirth).Days;
                    AgeIncrementingEventArgs arg = new AgeIncrementingEventArgs(p);
                    OnAgeIncrement(this, arg);
                    if (arg.Remove)
                    {
                        _participants.RemoveAt(_nextIndex);
                        if (_participants.Count == 0)
                        {
                            _timer.Stop();
                            return;
                        }
                    }
                    else
                    {
                        _nextIndex++;
                    }
                }
                if (_nextIndex >= _participants.Count)
                {
                    _nextIndex = 0;
                    break;
                }
            } while(_participants[_nextIndex].Key <= nowTime);
            nowTime = DateTime.Now.TimeOfDay;
#if DEBUG
            if (moqArgs!=null)
            {
                nowTime = moqArgs.Now.TimeOfDay;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("next interval set at:{0}", _participants[_nextIndex].Key - nowTime);
            }
#endif
            TimeSpan nextInterval = _participants[_nextIndex].Key - nowTime;
            if (nextInterval.Ticks < 0) { nextInterval += TimeSpan.FromDays(1); }
            _timer.Interval = nextInterval;
        }
        #endregion

        #region public eventhandlers
        public EventHandler<AgeIncrementingEventArgs> OnAgeIncrement;
        #endregion

        #region methods
        /*
        void SetAllAges()
        {
            DateTime now = DateTime.Now;
            _participants.ForEach(p=>p.Value.AgeDays = (now - p.Value.DateTimeBirth).Days);
        }
        */
        void StartTimer(TimeSpan interval)
        {
            _timer.Interval = interval;
            System.Diagnostics.Debug.WriteLine("next interval set at:{0}", interval);
            _timer.Start();
        }
        public void AddParticipant(IBirthday p)
        {
            DateTime now = DateTime.Now;
            var newItem = new KeyValuePair<TimeSpan, IBirthday>(p.DateTimeBirth.TimeOfDay, p);
            var index = _participants.Add(newItem);
            while (--index>=0 && _participants[index].Key == p.DateTimeBirth.TimeOfDay) { }
            index++;
            if (_nextIndex == index)
            {
                if (index == 0) 
                {
                    TimeSpan interval = newItem.Key - now.TimeOfDay;
                    if (interval.Ticks < 0)
                    {
                        interval += TimeSpan.FromDays(1);
                    }
                    if (_participants.Count == 1)
                    {
                        _nextIndex = 0;
                        StartTimer(interval);
                    }
                }
                else if (newItem.Key >= now.TimeOfDay)
                {
                    var rightNow = DateTime.Now;
                    TimeSpan nextInterval = newItem.Key - rightNow.TimeOfDay;
                    if (nextInterval.Ticks <= 0)
                    {
                        //logically the only way to get here ie newitem time > now[start of function] & newitem time < now[right now] 
                        //is for time to have elapsed while function was executing
                        OnTick(this, null);
                        _nextIndex++;
                        return;
                    }
                    _timer.Interval = nextInterval;
                }
                else
                {
                    _nextIndex++;
                }
            }
            else if (_nextIndex == 0 && index == _participants.Count - 1)
            {
                if (now.TimeOfDay < newItem.Key)
                {
                    _nextIndex = index;
                    _timer.Interval = newItem.Key - now.TimeOfDay;
                }
            }
            else if (_nextIndex > index)
            {
                _nextIndex++;
            }
        }

        #endregion

        #region ICleanup implementation
        bool _isCleanedup;
        public void Cleanup()
        {
            if (!_isCleanedup)
            {
                _timer.Tick -= OnTick;
                _timer.Stop();
                _isCleanedup = true;
            }
        }
        #endregion
    }

    public class KeyComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>>
        where TKey:IComparable
    {
        public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return x.Key.CompareTo(y.Key);
        }
    }

    public class AgeIncrementingEventArgs : EventArgs
    {
        internal AgeIncrementingEventArgs(IBirthday p)
        {
            Participant = p;
        }
        public IBirthday Participant {get; private set;}
        public bool Remove { get; set; }
    }
}
