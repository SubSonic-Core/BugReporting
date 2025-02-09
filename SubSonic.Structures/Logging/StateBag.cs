using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubSonic.Logging
{
    public class StateBag
        : IDisposable
    {
        private readonly Dictionary<string, object> _bag = new Dictionary<string, object>();

        public IDictionary<string, object> Bag => _bag;

        public bool IsEmpty => _bag.Count == 0;

        public void Dispose()
        {
            if (!IsEmpty)
            {
                _bag.Clear();
            }
        }

        public bool TryAdd(string key, object value)
        {
            return _bag.TryAdd(key, value);
        }

        public void AddCollection(IEnumerable<KeyValuePair<string, object>> collection)
        {
            foreach (KeyValuePair<string, object> item in collection)
            {
                TryAdd(item.Key, item.Value);
            }
        }

        public TValue? GetValue<TValue>(string key)
        {
            if (Bag.ContainsKey(key))
            {
                return (TValue)Bag[key];
            }

            return default;
        }
    }
}
