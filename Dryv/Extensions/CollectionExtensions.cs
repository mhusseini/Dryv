using System;
using System.Collections.Generic;
using System.Linq;

namespace Dryv.Extensions
{
    public static class CollectionExtensions
    {
        internal static bool ElementsEqual<T>(this IList<T> enumerable, params T[] items)
        {
            return enumerable.Count == items.Length && !items.Where((t, i) => !Equals(enumerable[i], t)).Any();
        }

        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
        {
            return new HashSet<T>(items);
        }

        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }

        internal static IEnumerable<T> Iterrate<T>(this T item, Func<T, T> next)
        where T : class
        {
            do
            {
                yield return item;
                item = next(item);
            } while (item != null);
        }

        internal static bool TryRemove<T>(this ICollection<T> list, T item)
        {
            if (!list.Contains(item))
            {
                return false;
            }

            list.Remove(item);
            return true;
        }
    }
}