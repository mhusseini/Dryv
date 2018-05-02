using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dryv.Utils
{
    internal static class TypeExtensions
    {
        public static string GetJavaScriptType(this Type type)
        {
            switch (type.Name)
            {
                case nameof(Int16):
                case nameof(Int32):
                case nameof(Int64):
                case nameof(UInt16):
                case nameof(UInt32):
                case nameof(UInt64):
                case nameof(Byte):
                case nameof(SByte):
                case nameof(Decimal):
                case nameof(Double):
                case nameof(Single):
                    return "number";

                case nameof(String):
                    return "string";

                case nameof(Boolean):
                    return "boolean";

                default:
                    return "object";
            }
        }

        internal static Type GetElementType(this PropertyInfo property)
        {
            var type = property.PropertyType;
            while (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
            {
                type = type.GetGenericArguments().First();
            }

            return type;
        }

        internal static IEnumerable<PropertyInfo> GetInheritedProperties(this PropertyInfo property)
        {
            return from type in property.DeclaringType.GetTypeHierarchyAndInterfaces()
                   from p in type.GetProperties()
                   where p.Name == property.Name
                   select p;
        }

        internal static IEnumerable<Type> GetTypeHierarchyAndInterfaces(this Type type)
        {
            var typeHierarchy = type.Iterrate(t => t.BaseType).ToList();

            return (from t in typeHierarchy
                    from i in t.GetInterfaces()
                    select i).Union(typeHierarchy);
        }

        internal static bool IsNavigationProperty(this PropertyInfo property)
        {
            var type = property.GetElementType();
            return type.IsClass && type.Namespace != "System";
        }
    }
}