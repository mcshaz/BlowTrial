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
    }
}
