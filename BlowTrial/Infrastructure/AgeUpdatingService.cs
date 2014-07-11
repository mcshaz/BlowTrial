using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Collections.Specialized;

namespace BlowTrial.Infrastructure
{
    public class AgeUpdatingService : ICleanup
    {
        #region fields
        readonly IDispatcherTimer _timer;
        int _nextIndex;
        readonly List<KeyValuePair<TimeSpan, ParticipantListItemViewModel>> _participants;
        readonly IComparer<KeyValuePair<TimeSpan, ParticipantListItemViewModel>> _comparer = new KeyComparer<TimeSpan, ParticipantListItemViewModel>();
        readonly TimeSpan _adjustIntoFuture = TimeSpan.FromSeconds(1);
        #endregion

        #region constructor
        public AgeUpdatingService(IList<ParticipantListItemViewModel> participants, DispatcherPriority priority = DispatcherPriority.Normal)
            : this (participants, new DispatcherTimerAdaper(priority))
        {
        }
        public AgeUpdatingService(IList<ParticipantListItemViewModel> participants, IDispatcherTimer timer)
        {
            _timer = timer;
            _timer.Tick += OnTick;
            _participants = new List<KeyValuePair<TimeSpan, ParticipantListItemViewModel>>(participants.Count);
            var now = DateTime.Now;
            for (int i=0; i<participants.Count;i++)
            {
                ParticipantListItemViewModel p = participants[i];
                p.AgeDays = (now - p.DateTimeBirth).Days;
                if (p.AgeDays<=28 & p.IsKnownDead != true)
                {
                    _participants.Add(new KeyValuePair<TimeSpan, ParticipantListItemViewModel>(p.DateTimeBirth.TimeOfDay, p));
                }
            }
            
            if (_participants.Any())
            {
                _participants.Sort(_comparer);

                _nextIndex = _participants.BinarySearch(new KeyValuePair<TimeSpan, ParticipantListItemViewModel>(now.TimeOfDay, null), _comparer);
                if (_nextIndex < 0)
                {
                    _nextIndex = ~_nextIndex;
                }
                TimeSpan rightNow = DateTime.Now.TimeOfDay;
                TimeSpan interval;
                if (_nextIndex >= _participants.Count)
                {
                    _nextIndex = 0;
                    interval = TimeSpan.FromDays(1) - rightNow + _participants[0].Key;
                }
                else
                {
                    interval = _participants[_nextIndex].Key <= rightNow
                        ? new TimeSpan(0)
                        : _participants[_nextIndex].Key - rightNow;
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
                ParticipantListItemViewModel p =_participants[_nextIndex].Value;
                if (OnAgeIncrement == null)
                {
                    p.AgeDays++;
                }
                else
                {
                    OnAgeIncrement(this, new AgeIncrementingEventArgs(p, p.AgeDays + 1));
                }
                if (p.AgeDays>28 || p.IsKnownDead == true)
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
        void StartTimer(TimeSpan interval)
        {
            _timer.Interval = interval;
            System.Diagnostics.Debug.WriteLine("next interval set at:{0}", interval);
            _timer.Start();
        }
        public void AddParticipant(ParticipantListItemViewModel p)
        {
            DateTime now = DateTime.Now;
            p.AgeDays = (now - p.DateTimeBirth).Days;
            if (p.AgeDays > 28 || p.IsKnownDead == true) { return; }
            var newItem = new KeyValuePair<TimeSpan, ParticipantListItemViewModel>(p.DateTimeBirth.TimeOfDay, p);
            var index = _participants.BinarySearch(newItem, _comparer);
            if (index < 0)
            {
                index = ~index;
            }
            else
            {
                _participants.Insert(index, newItem);
                return;
            }
            _participants.Insert(index, newItem);

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
                        p.AgeDays++;
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
        internal AgeIncrementingEventArgs(ParticipantListItemViewModel p, int newAge)
        {
            ParticipantViewModel = p;
            NewAge = newAge;
        }
        public ParticipantListItemViewModel ParticipantViewModel {get; private set;}
        public int NewAge { get; private set; }
    }
}
