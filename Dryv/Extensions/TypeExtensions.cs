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

        public static bool IsEnumOrNullableEnum(this Type type, out Type enumType)
        {
            enumType = null;
            
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsEnum)
            {
                enumType = type;
                return true;
            }

            if (!typeInfo.IsGenericType || typeInfo.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                return false;
            }
            
            var innerType = typeInfo.GenericTypeArguments.First();
            if (!innerType.GetTypeInfo().IsEnum)
            {
                return false;
            }
            
            enumType = innerType;
            return true;

        }
    }
}