using System;
using System.Collections.Generic;

namespace Dryv.Utils
{
    internal static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }

        public static bool TryRemove<T>(this ICollection<T> list, T item)
        {
            if (!list.Contains(item))
            {
                return false;
            }

            list.Remove(item);
            return true;
        }

        public static IEnumerable<T> Iterrate<T>(this T item, Func<T, T> next)
        where T : class
        {
            do
            {
                yield return item;
                item = next(item);
            } while (item != null);
        }
    }
}