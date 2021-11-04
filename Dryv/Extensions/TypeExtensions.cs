using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dryv.Reflection;

namespace Dryv.Extensions
{
    internal static class TypeExtensions
    {
        private static BindingFlags DefaultBindingFlags =>
            BindingFlags.Public | BindingFlags.FlattenHierarchy |
            BindingFlags.Instance | BindingFlags.NonPublic |
            BindingFlags.Static;

        public static Type GetBaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }

        public static IEnumerable<TypeInfo> WithImplementedInterfaces(this TypeInfo type)
        {
            return GetWithInterfacesCore(type).Distinct();
        }

        private static IEnumerable<TypeInfo> GetWithInterfacesCore(this TypeInfo type)
        {
            yield return type;

            foreach (var t in from iface in type.ImplementedInterfaces
                              from t in GetWithInterfacesCore(iface.GetTypeInfo())
                              select t)
            {
                yield return t;
            }
        }

        public static IList<FieldInfo> GetFields(this Type type, BindingFlags flags)
        {
            return GetFields(type.GetTypeInfo(), flags);
        }

        public static IList<FieldInfo> GetFields(this TypeInfo typeInfo, BindingFlags flags)
        {
            var isPublic = flags.HasFlag(BindingFlags.Public);
            var isNonPublic = flags.HasFlag(BindingFlags.NonPublic);
            var isFlatten = flags.HasFlag(BindingFlags.FlattenHierarchy);
            var isStatic = flags.HasFlag(BindingFlags.Static) || !flags.HasFlag(BindingFlags.Instance);

            var items = isFlatten
                ? typeInfo.DeclaredFields
                : from t in typeInfo.Iterate(i => i.BaseType.GetTypeInfo())
                  from i in t.DeclaredFields
                  select i;

            return (from i in items
                    where isPublic && i.IsPublic ||
                          isNonPublic && !i.IsPublic ||
                          isStatic && i.IsStatic
                    select i).ToList();
        }

        public static IList<Type> GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }

        public static MemberInfo GetMember(this Type type, string name)
        {
            return type.GetTypeInfo().DeclaredMembers.FirstOrDefault(m => m.Name == name);
        }

        public static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetMethods().FirstOrDefault(m => m.Name == name);
        }

        public static MethodInfo GetMethod(this Type type, string name, params Type[] parameters)
        {
            return type.GetMethods().FirstOrDefault(m => m.Name == name && m.GetParameters().Select(p => p.ParameterType).ToList().ElementsEqual(parameters));
        }

        public static IList<MethodInfo> GetMethods(this Type type)
        {
            return GetMethods(type, DefaultBindingFlags);
        }

        public static IList<MethodInfo> GetMethods(this Type type, BindingFlags flags)
        {
            return GetMethods(type.GetTypeInfo(), flags);
        }

        public static IList<MethodInfo> GetMethods(this TypeInfo typeInfo, BindingFlags flags)
        {
            var isPublic = flags.HasFlag(BindingFlags.Public);
            var isNonPublic = flags.HasFlag(BindingFlags.NonPublic);
            var isFlatten = flags.HasFlag(BindingFlags.FlattenHierarchy);
            var isStatic = flags.HasFlag(BindingFlags.Static) || !flags.HasFlag(BindingFlags.Instance);

            var items = isFlatten
                ? typeInfo.DeclaredMethods
                : from t in typeInfo.Iterate(i => i.BaseType.GetTypeInfo())
                  from i in t.DeclaredMethods
                  select i;

            return (from i in items
                    where isPublic && i.IsPublic ||
                          isNonPublic && !i.IsPublic ||
                          isStatic && i.IsStatic
                    select i).ToList();
        }

        public static IList<PropertyInfo> GetProperties(this Type type)
        {
            return GetProperties(type, DefaultBindingFlags);
        }

        public static IList<PropertyInfo> GetProperties(this Type type, BindingFlags flags)
        {
            return GetProperties(type.GetTypeInfo(), flags);
        }

        public static IList<PropertyInfo> GetProperties(this TypeInfo typeInfo, BindingFlags flags)
        {
            var isNonPublic = flags.HasFlag(BindingFlags.NonPublic);
            var isPublic = flags.HasFlag(BindingFlags.Public) || !isNonPublic;
            var isFlatten = flags.HasFlag(BindingFlags.FlattenHierarchy);
            var isStatic = flags.HasFlag(BindingFlags.Static);
            var isInstance = flags.HasFlag(BindingFlags.Instance) || !isStatic;

            var items = isFlatten
                ? from t in typeInfo.Iterate(i => i.BaseType?.GetTypeInfo())
                  from i in t.DeclaredProperties
                  select i
                : typeInfo.DeclaredProperties;

            return (from i in items
                    where (isPublic && i.GetMethod.IsPublic ||
                           isNonPublic && !i.GetMethod.IsPublic)
                          &&
                          (isStatic && i.GetMethod.IsStatic ||
                           isInstance && !i.GetMethod.IsStatic)
                    select i).ToList();
        }

        public static bool IsAssignableFrom(this Type type, Type other)
        {
            return type.GetTypeInfo().IsAssignableFrom(other.GetTypeInfo());
        }

        public static bool IsSystemType(this Type type)
        {
            return type.Namespace == "System";
        }

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

        internal static bool IsNavigationMember(this MemberInfo member)
        {
            return member is PropertyInfo pi
                ? pi.IsNavigationProperty()
                : member is FieldInfo fi && fi.IsNavigationField();

        }

        internal static bool IsNavigationField(this FieldInfo property)
        {
            var type = property.GetElementType().GetTypeInfo();
            return !type.IsValueType && type.Namespace != "System";
        }

        internal static bool IsNavigationProperty(this PropertyInfo property)
        {
            var type = property.GetElementType().GetTypeInfo();
            return !type.IsValueType && type.Namespace != "System";
        }

        internal static bool IsNavigationList(this PropertyInfo property)
        {
            var result = false;
            var type = property.PropertyType;
            var typeInfo = type.GetTypeInfo();

            while (type != typeof(string) && EnumerableTypeInfo.IsAssignableFrom(typeInfo) && typeInfo.IsGenericType)
            {
                type = typeInfo.GenericTypeArguments.First();
                typeInfo = type.GetTypeInfo();
            }

            if (type == property.PropertyType)
            {
                return false;
            }

            return !typeInfo.IsValueType && type.Namespace != "System";
        }
    }
}