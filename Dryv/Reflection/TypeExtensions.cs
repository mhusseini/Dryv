using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dryv.Utils;

namespace Dryv.Reflection
{
    internal static class TypeExtensions
    {
        public static Type GetBaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }

        public static IList<FieldInfo> GetFields(this Type type)
            => GetFields(
            type,
            BindingFlags.Public | BindingFlags.FlattenHierarchy |
            BindingFlags.Instance | BindingFlags.NonPublic |
            BindingFlags.Static);

        public static IList<FieldInfo> GetFields(this Type type, BindingFlags flags)
            => GetFields(type.GetTypeInfo(), flags);

        public static IList<FieldInfo> GetFields(this TypeInfo typeInfo) => GetFields(
            typeInfo,
            BindingFlags.Public | BindingFlags.FlattenHierarchy |
            BindingFlags.Instance | BindingFlags.NonPublic |
            BindingFlags.Static);

        public static IList<FieldInfo> GetFields(this TypeInfo typeInfo, BindingFlags flags)
        {
            var isPublic = flags.HasFlag(BindingFlags.Public);
            var isNonPublic = flags.HasFlag(BindingFlags.NonPublic);
            var isFlatten = flags.HasFlag(BindingFlags.FlattenHierarchy);
            var isStatic = flags.HasFlag(BindingFlags.Static) || !flags.HasFlag(BindingFlags.Instance);

            var items = isFlatten
                ? typeInfo.DeclaredFields
                : from t in typeInfo.Iterrate(i => i.BaseType.GetTypeInfo())
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

        public static IEnumerable<Type> GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces;
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
            => GetMethods(
                type,
                BindingFlags.Public | BindingFlags.FlattenHierarchy |
                BindingFlags.Instance | BindingFlags.NonPublic |
                BindingFlags.Static);

        public static IList<MethodInfo> GetMethods(this Type type, BindingFlags flags)
            => GetMethods(type.GetTypeInfo(), flags);

        public static IList<MethodInfo> GetMethods(this TypeInfo typeInfo)
            => GetMethods(
                typeInfo,
                BindingFlags.Public | BindingFlags.FlattenHierarchy |
                BindingFlags.Instance | BindingFlags.NonPublic |
                BindingFlags.Static);

        public static IList<MethodInfo> GetMethods(this TypeInfo typeInfo, BindingFlags flags)
        {
            var isPublic = flags.HasFlag(BindingFlags.Public);
            var isNonPublic = flags.HasFlag(BindingFlags.NonPublic);
            var isFlatten = flags.HasFlag(BindingFlags.FlattenHierarchy);
            var isStatic = flags.HasFlag(BindingFlags.Static) || !flags.HasFlag(BindingFlags.Instance);

            var items = isFlatten
                ? typeInfo.DeclaredMethods
                : from t in typeInfo.Iterrate(i => i.BaseType.GetTypeInfo())
                  from i in t.DeclaredMethods
                  select i;

            return (from i in items
                    where isPublic && i.IsPublic ||
                          isNonPublic && !i.IsPublic ||
                          isStatic && i.IsStatic
                    select i).ToList();
        }

        public static IList<PropertyInfo> GetProperties(this Type type)
                                                            => GetProperties(
            type,
            BindingFlags.Public | BindingFlags.FlattenHierarchy |
            BindingFlags.Instance | BindingFlags.NonPublic |
            BindingFlags.Static);

        public static IList<PropertyInfo> GetProperties(this Type type, BindingFlags flags)
            => GetProperties(type.GetTypeInfo(), flags);

        public static IList<PropertyInfo> GetProperties(this TypeInfo typeInfo)
            => GetProperties(
            typeInfo,
            BindingFlags.Public | BindingFlags.FlattenHierarchy |
            BindingFlags.Instance | BindingFlags.NonPublic |
            BindingFlags.Static);

        public static IList<PropertyInfo> GetProperties(this TypeInfo typeInfo, BindingFlags flags)
        {
            var isNonPublic = flags.HasFlag(BindingFlags.NonPublic);
            var isPublic = flags.HasFlag(BindingFlags.Public) || !isNonPublic;
            var isFlatten = flags.HasFlag(BindingFlags.FlattenHierarchy);
            var isStatic = flags.HasFlag(BindingFlags.Static);
            var isInstance = flags.HasFlag(BindingFlags.Instance) || !isStatic;

            var items = isFlatten
                ? from t in typeInfo.Iterrate(i => i.BaseType?.GetTypeInfo())
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

        public static bool IsClass(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        public static bool IsInterface(this Type type)
        {
            return type.GetTypeInfo().IsInterface;
        }
        public static bool IsSystemType(this Type type)
        {
            return type.Namespace == "System";
        }
    }
}