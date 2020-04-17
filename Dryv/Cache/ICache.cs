using System;

namespace Dryv.Cache
{
    public interface ICache
    {
        TItem GetOrAdd<TItem>(string key, Func<TItem> factory);
    }
}