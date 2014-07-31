using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrialUnitTests
{
    public static class LinqExtensions
    {
        [DebuggerStepThrough]
        public static IEnumerable<Tout> AggregatePairSelect<Tin, Tout>(this IEnumerable<Tin> inList, Func<Tin,Tin,Tout> predicate)
        {
            var e = inList.GetEnumerator();
            if (e.MoveNext())
            {
                Tin prev = e.Current;
                while (e.MoveNext())
                {
                    yield return predicate(prev, e.Current);
                    prev = e.Current;
                }
            }
        }
        [DebuggerStepThrough]
        public static IEnumerable<Tout> AggregatePairSelect<Tin, Tout>(this IEnumerable<Tin> inList, Tin start, Func<Tin, Tin, Tout> predicate)
        {
            return (new Tin[] { start }).Concat(inList).AggregatePairSelect(predicate);
        }
        [DebuggerStepThrough]
        public static void AggregatePairForEach<T>(this IEnumerable<T> inList, Action<T, T> predicate)
        {
            using (var e = inList.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    T prev = e.Current;
                    while (e.MoveNext())
                    {
                        predicate(prev, e.Current);
                        prev = e.Current;
                    }
                }
            }
        }
        [DebuggerStepThrough]
        public static void AggregatePairForEach<T>(this IEnumerable<T> inList, T start, Action<T, T> predicate)
        {
            (new T[] { start }).Concat(inList).AggregatePairForEach(predicate);
        }

    }
}
