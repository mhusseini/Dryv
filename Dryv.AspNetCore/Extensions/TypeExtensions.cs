using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dryv.AspNetCore.Extensions
{
    public static class TypeExtensions
    {
        private static readonly TypeInfo EnumerableTypeInfo = typeof(IEnumerable).GetTypeInfo();

        internal static Type GetElementType(this PropertyInfo property)
        {
            var type = property.PropertyType;
            return GetElementType(type);
        }

        internal static Type GetElementType(this FieldInfo property)
        {
            var type = property.FieldType;
            return GetElementType(type);
        }

        internal static Type GetElementType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();

            while (type != typeof(string) && EnumerableTypeInfo.IsAssignableFrom(typeInfo) && typeInfo.IsGenericType)
            {
                type = typeInfo.GenericTypeArguments.First();
                typeInfo = type.GetTypeInfo();
            }

            return type;
        }

        internal static IList<MemberInfo> GetPropertiesAndFields(this Type type, BindingFlags bindingFlags)
        {
            var pi = type.GetProperties(bindingFlags).Cast<MemberInfo>();
            var fi = type.GetFields(bindingFlags).Cast<MemberInfo>();

            return pi.Union(fi).ToList();
        }

        internal static IList<MemberInfo> GetPropertiesAndFields(this Type type)
        {
            var pi = type.GetProperties().Cast<MemberInfo>();
            var fi = type.GetFields().Cast<MemberInfo>();

            return pi.Union(fi).ToList();
        }

        internal static Type GetMemberType(this MemberInfo member)
        {
            return member switch
            {
                PropertyInfo pi => pi.PropertyType,
                FieldInfo fi => fi.FieldType,
                _ => null
            };

        }

        internal static bool IsNavigationMember(this MemberInfo member)
        {
            return member is PropertyInfo pi
                ? pi.IsNavigationProperty()
                : member is FieldInfo fi && fi.IsNavigationField();

        }
        internal static bool IsNavigationField(this FieldInfo property)
        {
            var type = property.GetElementType();
            return !type.IsValueType && type.Namespace != "System";
        }

        internal static bool IsNavigationProperty(this PropertyInfo property)
        {
            var type = property.GetElementType();
            return !type.IsValueType && type.Namespace != "System";
        }
    }
}