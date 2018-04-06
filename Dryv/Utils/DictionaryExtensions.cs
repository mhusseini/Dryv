using System;
using System.Collections.Generic;

namespace Dryv.Utils
{
    internal static class DictionaryExtensions
    {
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