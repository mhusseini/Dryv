using System.Collections.Generic;
using System.Linq;

namespace Dryv.Extensions
{
    internal static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IEnumerable<IDictionary<TKey, TValue>> dictionaries)
        {
            var result = new Dictionary<TKey, TValue>();

            foreach (var i in from d in dictionaries
                              from i in d
                              select i)
            {
                result[i.Key] = i.Value;
            }

            return result;
        }
    }
}