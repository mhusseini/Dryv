using System;
using System.Collections.Generic;
using System.Linq;

namespace Dryv.Extensions
{
    internal static class IterationExtensions
    {
        public static IEnumerable<T> Iterate<T>(this T current, Func<T, T> next)
            where T : class
        {
            do
            {
                yield return current;
            } while ((current = next(current)) != default);
        }

        public static IEnumerable<T> Iterate<T>(this T current, Func<T, IEnumerable<T>> next)
            where T : class
        {
            yield return current;
            foreach (var item in from item in next(current)
                                 from child in Iterate(item, next)
                                 select child)
            {
                yield return item;
            }
        }
    }
}