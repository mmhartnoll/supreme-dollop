using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MindSculptor.Tools.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T obj)
        {
            yield return obj;
        }

        public static IEnumerable<T> Enumerate<T>(this IEnumerable<T> source)
        {
            foreach (var item in source)
                yield return item;
        }

        public static VerifiedResult<T> TryGetSingle<T>(this IEnumerable<T> source)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return VerifiedResult<T>.Failure;
                var value = enumerator.Current;
                if (enumerator.MoveNext())
                    throw new InvalidOperationException("The input sequence contains more than one element.");
                return VerifiedResult<T>.Successful(value);
            }
        }

        public async static IAsyncEnumerable<TOut> SelectAsync<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, Task<TOut>> outputFunction)
        {
            foreach (var item in source)
                yield return await outputFunction.Invoke(item).ConfigureAwait(false);
        }
    }
}
