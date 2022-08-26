using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MindSculptor.Tools.Extensions
{
    public static class IAsyncEnumerableExtensions
    {
        public async static Task<bool> AnyAsync<T>(this IAsyncEnumerable<T> source)
        {
            await foreach (var item in source.ConfigureAwait(false))
                return true;
            return false;
        }

        public async static IAsyncEnumerable<T> EnumerateAsync<T>(this IAsyncEnumerable<T> source)
        {
            await foreach (var item in source.ConfigureAwait(false))
                yield return item;
        }

        public async static IAsyncEnumerable<T> WhereAsync<T>(this IAsyncEnumerable<T> source, Func<T, bool> predicate)
        {
            await foreach (var item in source.ConfigureAwait(false))
                if (predicate(item))
                    yield return item;
        }

        public async static IAsyncEnumerable<T> WhereAsync<T>(this IAsyncEnumerable<T> source, Func<T, Task<bool>> predicate)
        {
            await foreach (var item in source.ConfigureAwait(false))
                if (await predicate(item).ConfigureAwait(false))
                    yield return item;
        }

        public async static IAsyncEnumerable<T> WhereAsync<T>(this IAsyncEnumerable<T> source, Func<T, ValueTask<bool>> predicate)
        {
            await foreach (var item in source.ConfigureAwait(false))
                if (await predicate(item).ConfigureAwait(false))
                    yield return item;
        }

        public async static IAsyncEnumerable<TOut> SelectAsync<TIn, TOut>(this IAsyncEnumerable<TIn> source, Func<TIn, TOut> selector)
        {
            await foreach (var item in source.ConfigureAwait(false))
                yield return selector(item);
        }

        public async static IAsyncEnumerable<TOut> SelectAsync<TIn, TOut>(this IAsyncEnumerable<TIn> source, Func<TIn, Task<TOut>> selector)
        {
            await foreach (var item in source.ConfigureAwait(false))
                yield return await selector(item).ConfigureAwait(false);
        }

        public async static IAsyncEnumerable<TOut> SelectAsync<TIn, TOut>(this IAsyncEnumerable<TIn> source, Func<TIn, ValueTask<TOut>> selector)
        {
            await foreach (var item in source.ConfigureAwait(false))
                yield return await selector(item).ConfigureAwait(false);
        }

        public async static Task<T> FirstAsync<T>(this IAsyncEnumerable<T> source)
        {
            await foreach (var item in source.ConfigureAwait(false))
                return item;
            throw new InvalidOperationException("The source sequence is empty.");
        }

        public async static Task<VerifiedResult<T>> TryGetFirstAsync<T>(this IAsyncEnumerable<T> source)
        {
            await foreach (var item in source.ConfigureAwait(false))
                return VerifiedResult<T>.Successful(item);
            return VerifiedResult<T>.Failure;
        }

        public async static Task<T> SingleAsync<T>(this IAsyncEnumerable<T> source)
        {
            await using (var enumerator = source.GetAsyncEnumerator())
            {
                if (!await enumerator.MoveNextAsync())
                    throw new InvalidOperationException("The source sequence is empty.");
                var value = enumerator.Current;
                if (await enumerator.MoveNextAsync())
                    throw new InvalidOperationException("The input sequence contains more than one element.");
                return value;
            }
        }

        public async static Task<VerifiedResult<T>> TryGetSingleAsync<T>(this IAsyncEnumerable<T> source)
        {
            await using (var enumerator = source.GetAsyncEnumerator())
            {
                if (!await enumerator.MoveNextAsync())
                    return VerifiedResult<T>.Failure;
                var value = enumerator.Current;
                if (await enumerator.MoveNextAsync())
                    throw new InvalidOperationException("The input sequence contains more than one element.");
                return VerifiedResult<T>.Successful(value);
            }
        }

        public async static Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
        {
            var list = new List<T>();
            await foreach (var item in source.ConfigureAwait(false))
                list.Add(item);
            return list;
        }

        public async static Task<Dictionary<TKey, T>> ToDictionaryAsync<TKey, T>(this IAsyncEnumerable<T> source, Func<T, TKey> keySelector)
                where TKey : notnull
            => await source.ToDictionaryAsync(keySelector, value => value).ConfigureAwait(false);

        public async static Task<Dictionary<TKey, TValue>> ToDictionaryAsync<T, TKey, TValue>(this IAsyncEnumerable<T> source, Func<T, TKey> keySelector, Func<T, TValue> valueSelector)
            where TKey : notnull
        {
            var dictionary = new Dictionary<TKey, TValue>();
            await foreach (var item in source.ConfigureAwait(false))
                dictionary.Add(keySelector(item), valueSelector(item));
            return dictionary;
        }
    }
}
