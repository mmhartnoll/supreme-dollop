using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.Tools
{
    public class AsyncCachedLookup<TKey, TValue>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> lookupCache = new Dictionary<TKey, TValue>();
        private readonly Func<TKey, Task<VerifiedResult<TValue>>> lookupFunction;

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

        private AsyncCachedLookup(Func<TKey, Task<VerifiedResult<TValue>>> lookupFunction)
            => this.lookupFunction = lookupFunction;

        public static AsyncCachedLookup<TKey, TValue> Create(Func<TKey, Task<TValue>> lookupFunction)
            => new AsyncCachedLookup<TKey, TValue>(
                async key => 
                {
                    var value = await lookupFunction.Invoke(key)
                        .ConfigureAwait(false);
                    return value == null ? 
                        VerifiedResult<TValue>.Failure : 
                        VerifiedResult<TValue>.Successful(value);
                });

        public static AsyncCachedLookup<TKey, TValue> Create(Func<TKey, Task<VerifiedResult<TValue>>> lookupFunction)
            => new AsyncCachedLookup<TKey, TValue>(lookupFunction);

        public async Task<TValue> GetValueAsync(TKey key)
        {
            var result = await TryGetValueAsync(key)
                .ConfigureAwait(false);
            if (result.Success)
                return result.Value;
            throw new KeyNotFoundException(key.ToString());
        }

        public async Task<VerifiedResult<TValue>> TryGetValueAsync(TKey key)
        {
            if (lookupCache.TryGetValue(key, out var value))
                return VerifiedResult<TValue>.Successful(value);
            else
                try
                {
                    await semaphore.WaitAsync()
                        .ConfigureAwait(false);
                    if (lookupCache.TryGetValue(key, out value))
                        return VerifiedResult<TValue>.Successful(value);
                    var result = await lookupFunction.Invoke(key)
                        .ConfigureAwait(false);
                    if (!result.Success)
                        return result;
                    lookupCache.Add(key, result.Value);
                    return result;
                }
                finally
                {
                    semaphore.Release();
                }
        }
    }
}
