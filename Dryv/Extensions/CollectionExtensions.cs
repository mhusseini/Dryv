using System.Collections.Generic;
using System.Linq;

namespace Dryv.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }

        internal static bool ElementsEqual<T>(this IList<T> enumerable, params T[] items)
        {
            return enumerable.Count == items.Length && !items.Where((t, i) => !Equals(enumerable[i], t)).Any();
        }
    }
}