using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dryv.Utils
{
    internal static class DictionaryExtensions
    {
        public static ConcurrentDictionary<TKey, List<TValue>> Clone<TKey, TValue>(this ConcurrentDictionary<TKey, List<TValue>> dict)
        {
            return new ConcurrentDictionary<TKey, List<TValue>>(dict.ToDictionary(i => i.Key, i => new List<TValue>(i.Value)));
        }

        public static TValue GetOrAdd<TValue>(this IDictionary<object, object> dictionary, object key, Func<object, TValue> add)
        {
            if (dictionary.TryGetValue(key, out var o) && o is TValue value)
            {
                return value;
            }

            value = add(key);
            dictionary[key] = value;
            return value;
        }
    }
}