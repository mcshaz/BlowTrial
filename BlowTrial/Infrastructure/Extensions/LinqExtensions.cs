using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.Extensions
{
    static class LinqExtensions
    {
        //http://stackoverflow.com/questions/1779129/how-to-take-all-but-the-last-element-in-a-sequence-using-linq
        public static IEnumerable<T> DropLast<T>(this IEnumerable<T> source, int n)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (n < 0)
                throw new ArgumentOutOfRangeException("n",
                    "Argument n should be non-negative.");

            Queue<T> buffer = new Queue<T>(n + 1);

            foreach (T x in source)
            {
                buffer.Enqueue(x);

                if (buffer.Count == n + 1)
                    yield return buffer.Dequeue();
            }
        }

        public static void RemoveFirst<T>(this IList<T> source, Func<T,bool> predicate)
        {
            for (int i=0;i<source.Count;i++)
            {
                if (predicate(source[i]))
                {
                    source.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// faster than select().toarray();
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TResult[] Map<TSource, TResult>(this IList<TSource> source, Func<TSource, TResult> predicate)
        {
            TResult[] result = new TResult[source.Count];
            for (int i = 0; i < source.Count; i++)
            {
                result[i] = predicate(source[i]);
            }
            return result;
        }

        public static bool AllEqual<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> predicate)
        {
            var en = source.GetEnumerator();
            if (!en.MoveNext())
            {
                return true;
            }
            var primary = predicate(en.Current);
            while (en.MoveNext())
            {
                if (!primary.Equals(predicate(en.Current)))
                {
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<T> AllExceptFirstMax<T>(this IEnumerable<T> source, Func<T, int> predicate)
        {
            List<int> resultList = new List<int>();
            int max=int.MinValue;
            int removeAt=0;
            int i=0;
            List<T> returnVar = new List<T>();
            var en = source.GetEnumerator();
            while (en.MoveNext())
            {
                if (predicate(en.Current) > max)
                {
                    removeAt = i;
                }
                returnVar.Add(en.Current);
                i++;
            }
            returnVar.RemoveAt(removeAt);
            return returnVar;
        }
    }
}
