using System;
using System.Collections.Concurrent;

namespace Dryv.Cache
{
    public class InMemoryCache : ICache
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, object>> Inner = new ConcurrentDictionary<Type, ConcurrentDictionary<string, object>>();

        public TItem GetOrAdd<TItem>(string key, Func<TItem> factory)
        {
            return (TItem)Inner
                .GetOrAdd(typeof(TItem), _ => new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase))
                .GetOrAdd(key, _ => factory());
        }
    }
}