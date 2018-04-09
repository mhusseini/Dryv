using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dryv.Utils
{
    internal static class TypeExtensions
    {
        public static IEnumerable<Type> GetTypeHierarchyAndInterfaces(this Type type)
        {
            var typeHierarchy = type.Iterrate(t => t.BaseType).ToList();

            return (from t in typeHierarchy
                    from i in t.GetInterfaces()
                    select i).Union(typeHierarchy);
        }

        public static IEnumerable<PropertyInfo> GetInheritedProperties(this PropertyInfo property)
        {
            return from type in property.DeclaringType.GetTypeHierarchyAndInterfaces()
                   from p in type.GetProperties()
                   where p.Name == property.Name
                   select p;
        }

        public static Type GetElementType(this PropertyInfo property)
        {
            var type = property.PropertyType;
            while (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
            {
                type = type.GetGenericArguments().First();
            }

            return type;
        }

        public static bool IsNavigationProperty(this PropertyInfo property)
        {
            var type = property.GetElementType();
            return type.IsClass && type.Namespace != "System";
        }
    }
}