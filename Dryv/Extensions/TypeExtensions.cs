using System;
using System.Linq;
using System.Reflection;
using Dryv.Reflection;

namespace Dryv.Extensions
{
    internal static class TypeExtensions
    {
        public static string GetNonGenericName(this Type type)
        {
            return type.FullName;
            //return type.GenericTypeArguments.Length == 0 ? type.FullName : type.GetGenericArguments().First().FullName;
        }
    }
}