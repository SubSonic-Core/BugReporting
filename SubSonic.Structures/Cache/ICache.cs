using System;

namespace SubSonic.Cache
{
    public interface ICache<TCacheKey, TCacheValue>
    {
        bool ContainsKey(TCacheKey key);
        /// <summary>
        /// Get the value from cache, if missing get it from a source of truth
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getter"></param>
        /// <returns></returns>
        TCacheValue Get(TCacheKey key, Func<TCacheKey, TCacheValue> getter);
    }
}
