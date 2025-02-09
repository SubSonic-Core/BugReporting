using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace SubSonic.Cache
{
    public class ExpiringCacher<TCacheKey, TCacheValue>
        : IExpiringCache<TCacheKey, TCacheValue>
        where TCacheKey : notnull
    {
        private readonly IOptionsMonitor<ExpiringCacheOptions> _options;

        private readonly ConcurrentDictionary<TCacheKey, ExpiringCacheValue<TCacheValue>> _cache = new ConcurrentDictionary<TCacheKey, ExpiringCacheValue<TCacheValue>>();

        public ExpiringCacher(IOptionsMonitor<ExpiringCacheOptions> options)
        {
            _options = options;
        }

        public ExpiringCacheOptions Options => _options.Get(typeof(TCacheValue).Name);

        public bool ContainsKey(TCacheKey key) => _cache.ContainsKey(key);

        public ExpiringCacheValue<TCacheValue> Set(TCacheValue value) => new ExpiringCacheValue<TCacheValue>(Options.ExpiresIn, value);

        public virtual TCacheValue Get(TCacheKey key, Func<TCacheKey, TCacheValue> getter)
        {
            var item = _cache.GetOrAdd(key, (key) => Set(getter(key)));

            if (item.IsExpired && !_cache.TryUpdate(key, Set(getter(key)), item))
            {
                var exception = new InvalidOperationException("failed to replace expired cache value");

                exception.Data[key] = item;

                throw exception;
            }
            else if (!_cache.TryGetValue(key, out item))
            {
                throw new InvalidOperationException($"{key} does not exist");
            }

            return item.Value;
        }

        public void RemoveExpiredValues()
        {
            foreach(var item in _cache)
            {
                if (item.Value.IsExpired)
                {
                    _cache.TryRemove(item);
                }
            }
        }
    }
}
