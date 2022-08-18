using System;

namespace Dryv.Extensions
{
    internal static class TypeExtensions
    {
        public static bool IsComplexType(this Type type)
        {
            return !type.IsEnum && !type.IsPrimitive && type.Namespace != typeof(string).Namespace;
        }
    }
}