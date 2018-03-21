using System;
using System.Collections.Generic;

namespace Dryv.Utils
{
    internal static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
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