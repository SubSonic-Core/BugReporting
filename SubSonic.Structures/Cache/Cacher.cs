using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SubSonic.Cache
{
    /// <summary>
    /// Non Expiring Cache
    /// </summary>
    /// <typeparam name="TCacheKey"></typeparam>
    /// <typeparam name="TCacheValue"></typeparam>
    public class Cacher<TCacheKey, TCacheValue>
        : ICache<TCacheKey, TCacheValue>
        where TCacheKey : notnull
    {
        private readonly ConcurrentDictionary<TCacheKey, TCacheValue> _cache = new ConcurrentDictionary<TCacheKey, TCacheValue>();

        public bool ContainsKey(TCacheKey key) => _cache.ContainsKey(key);

        public virtual TCacheValue Get(TCacheKey key, Func<TCacheKey, TCacheValue> getter)
        {
            return _cache.GetOrAdd(key, getter);
        }
    }
}
