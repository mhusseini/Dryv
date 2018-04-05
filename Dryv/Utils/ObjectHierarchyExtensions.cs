using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dryv.Utils
{
    internal static class ObjectHierarchyExtensions
    {
        public static IEnumerable<Type> GetObjecTypeHierarchy(this Type type)
        {
            var found = new HashSet<Type> { type };
            type.GetObjecTypeHierarchy(found);

            return found;
        }

        private static void GetObjecTypeHierarchy(this IReflect type, ICollection<Type> found)
        {
            const BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance |
                                              BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            var childTypes = type.GetProperties(bindingFlags).Select(p => p.PropertyType)
                .Union(type.GetFields(bindingFlags).Select(f => f.FieldType))
                .Distinct()
                .Where(t => !found.Contains(t))
                .ToList();

            found.AddRange(childTypes);

            foreach (var childType in childTypes)
            {
                childType.GetObjecTypeHierarchy(found);
            }
        }
    }
}