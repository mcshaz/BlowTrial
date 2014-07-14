using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrialUnitTests
{
    public class Counter<T> : IEnumerable<KeyValuePair<T, int>>, IEnumerable//, ICollection<KeyValuePair<T,int>>, ICollection
    {
        readonly IDictionary<T, int> _counts;
        public Counter() : this(new Dictionary<T, int>())
        {
        }
        public Counter(IDictionary<T,int> dictionary)
        {
            _counts = dictionary;
        }
        public Counter(IEnumerable<T> elements) : this()
        {
            foreach(T el in elements)
            {
                _counts.Add(el, 0);
            }
        }
        public Counter(IEqualityComparer<T> comparer)
        {
            _counts = new Dictionary<T, int>(comparer);
        }
        public ICollection<int> Values
        {
            get
            {
                return _counts.Values;
            }
        }
        public ICollection<T> Keys
        {
            get
            {
                return _counts.Keys;
            }
        }
        public int this[T val]
        {
            get
            {
                int returnVar;
                _counts.TryGetValue(val, out returnVar);
                return returnVar;
            }
            set
            {
                if (_counts.ContainsKey(val))
                {
                    _counts[val] = value;
                }
                else
                {
                    _counts.Add(val, value);
                }
            }
        }

        #region methods
        public void Increment(params T[] elements)
        {
            Increment((IEnumerable<T>)elements);
        }
        public void Increment(IEnumerable<T> elements)
        {
            foreach (T e in elements)
            {
                this[e]++;
            }
        }
        public bool ContainsKey(T key)
        {
            return _counts.ContainsKey(key);
        }
        public int Total()
        {
            return _counts.Values.Sum();
        }
        public Counter<int> CountOfCounts()
        {
            return new Counter<int>(new SortedDictionary<int, int>(
                _counts.Values.GroupBy(v=>v).ToDictionary(v=>v.Key,v=>v.Count())));
        }
        #endregion

        public int Count { get { return _counts.Count; } }

        //bool IsSynchronized { get { return _counts.IsS} }

        //object SyncRoot { get { return _counts.syn }  }

        public void CopyTo(KeyValuePair<T,int>[] array, int index)
        {
            _counts.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _counts.GetEnumerator();
        }
        public IEnumerator<KeyValuePair<T,int>> GetEnumerator()
        {
            return _counts.GetEnumerator();
        }
    }
}
