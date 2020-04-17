using System;
using System.Collections.Generic;

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
    }
}