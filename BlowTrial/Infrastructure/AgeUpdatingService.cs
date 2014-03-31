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
        readonly DispatcherTimer _timer;
        int _nextIndex;
        readonly List<KeyValuePair<TimeSpan, ParticipantListItemViewModel>> _participants;
        readonly IComparer<KeyValuePair<TimeSpan, ParticipantListItemViewModel>> _comparer;
        readonly TimeSpan _adjustIntoFuture = new TimeSpan(TimeSpan.TicksPerSecond);
        #endregion

        #region constructor
        public AgeUpdatingService(IEnumerable<ParticipantListItemViewModel> participants, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            var now = DateTime.Now;
            foreach (var p in participants)
            {
                p.AgeDays = (now - p.DateTimeBirth).Days;
            }
            var ic = participants as ICollection<ParticipantListItemViewModel>;
            if (ic == null)
            {
                _participants = new List<KeyValuePair<TimeSpan, ParticipantListItemViewModel>>();
            }
            else
            {
                _participants = new List<KeyValuePair<TimeSpan, ParticipantListItemViewModel>>(ic.Count);
            }
            _participants.AddRange(from p in participants
                                   where p.AgeDays<=28 & p.IsKnownDead != true
                                   select new KeyValuePair<TimeSpan, ParticipantListItemViewModel>(p.DateTimeBirth.TimeOfDay, p));
            _comparer = new KeyComparer<TimeSpan, ParticipantListItemViewModel>();
            _participants.Sort(_comparer);

            _nextIndex = _participants.BinarySearch(new KeyValuePair<TimeSpan, ParticipantListItemViewModel>(now.TimeOfDay, null) , _comparer);
            if (_nextIndex < 0)
            {
                _nextIndex = ~_nextIndex;
            }
            TimeSpan rightNow = DateTime.Now.TimeOfDay;
            TimeSpan interval = _participants[_nextIndex].Key <= rightNow
                ?new TimeSpan(0)
                :_participants[_nextIndex].Key - rightNow;
            _timer = new DispatcherTimer(priority)
            {
                Interval = interval
            };
            _timer.Tick += OnTick;
            _timer.Start();
        }
        #endregion

        #region EventHandlers
        void OnTick(object sender, EventArgs e)
        {
            TimeSpan nowTime = DateTime.Now.TimeOfDay.Add(_adjustIntoFuture);
            if (_participants.Count == 0) { return; }
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
                    if (_participants.Count == 0) { return; }
                }
                else
                {
                    _nextIndex++;
                }
                if (_nextIndex >= _participants.Count)
                {
                    _nextIndex = 0;
                }
            } while(nowTime >= _participants[_nextIndex].Key);
            TimeSpan nextInterval = _participants[_nextIndex].Key - DateTime.Now.TimeOfDay;
            if (nextInterval.Ticks < 0) { nextInterval += new TimeSpan(24, 0, 0); }
            _timer.Interval = nextInterval;
        }
        #endregion

        #region public eventhandlers
        public EventHandler<AgeIncrementingEventArgs> OnAgeIncrement;
        #endregion

        #region methods
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
                if (newItem.Key > now.TimeOfDay)
                {
                    var rightNow = DateTime.Now;
                    TimeSpan nextInterval = newItem.Key - rightNow.TimeOfDay;
                    if (nextInterval.Ticks <= 0) 
                    {
                        if (newItem.Key <= rightNow.TimeOfDay)
                        {
                            p.AgeDays++;
                            _nextIndex++;
                            return;
                        }
                        else
                        {
                            nextInterval += new TimeSpan(24, 0, 0);
                        }
                    }
                    _timer.Interval = nextInterval;
                }
                else
                {
                    _nextIndex++;
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
