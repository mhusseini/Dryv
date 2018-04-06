using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dryv.Utils;

namespace Dryv
{
    internal static class RulesFinder
    {
        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Static |
                                          System.Reflection.BindingFlags.Public |
                                          System.Reflection.BindingFlags.NonPublic |
                                          System.Reflection.BindingFlags.FlattenHierarchy;

        private static readonly ConcurrentDictionary<string, IList<(string Path, DryvRule Rule)>> PropertyRules = new ConcurrentDictionary<string, IList<(string Path, DryvRule Rule)>>();

        private static readonly ConcurrentDictionary<Type, IList<DryvRules>> TypeRules = new ConcurrentDictionary<Type, IList<DryvRules>>();

        public static IEnumerable<(string Path, DryvRule Rule)> GetRulesForProperty(this Type modelType, PropertyInfo property, string modelPath = "")
        {
            var key = $"{modelType.FullName}|{modelPath}|{property.DeclaringType.FullName}|{property.Name}";
            return PropertyRules.GetOrAdd(
                    key,
                    _ => GetInheritedRules(modelType, property, modelPath).ToList());
        }

        public static IEnumerable<DryvRules> GetRulesOnType(this Type objectType) => TypeRules.GetOrAdd(objectType, type =>
        {
            var fromFields = from p in type.GetFields(BindingFlags)
                             where typeof(DryvRules).IsAssignableFrom(p.FieldType)
                             select p.GetValue(null) as DryvRules;

            var fromProperties = from p in type.GetProperties(BindingFlags)
                                 where typeof(DryvRules).IsAssignableFrom(p.PropertyType)
                                 select p.GetValue(null) as DryvRules;

            var fromMethods = from m in type.GetMethods(BindingFlags)
                              where m.IsStatic
                                    && !m.GetParameters().Any()
                                    && typeof(DryvRules).IsAssignableFrom(m.ReturnType)
                              select m.Invoke(null, null) as DryvRules;

            return fromFields.Union(fromProperties).Union(fromMethods).ToList();
        });

        internal static IEnumerable<(string Path, DryvRule Rule)> FindRulesForProperty(this Type type, PropertyInfo property, string modelPath)
        {
            var result = (from tuple in type.FindRulesForProperty(property, new Dictionary<DryvRule, IList<string>>(), new HashSet<Type>(), null, modelPath)
                          select (
                              Path: string.Join(".", tuple.Path.Split(".").Reverse().Skip(1).Reverse()),
                              Rule: tuple.Rule)
                )
                .ToList();

            return result;
        }

        private static IEnumerable<(string Path, DryvRule Rule)> FindRulesForProperty(this Type type, PropertyInfo property, Dictionary<DryvRule, IList<string>> propertiesToFind, ISet<Type> processed, string path, string modelPath)
        {
            var pathPrefix = path == null ? string.Empty : $"{path}.";
            const BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance |
                                              BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            processed.Add(type);

            foreach (var rule in from ruleSet in type.GetRulesOnType()
                                 from kvp in ruleSet.PropertyRules
                                 where kvp.Key == property
                                 from rule in kvp.Value
                                 select rule)
            {
                var l = rule.ModelName.Split(".")
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();
                l.Add(rule.PropertyName.ToCamelCase());
                propertiesToFind[rule] = l;
            }

            foreach (var childProperty in from prop in type.GetProperties(bindingFlags)
                                          where prop.PropertyType.IsClass
                                          select prop)
            {
                var childPropertyName = childProperty.Name.ToCamelCase();
                var propertyPath = $"{pathPrefix}{childPropertyName}";
                var propertiesToFind2 = propertiesToFind
                    .Where(pp => pp.Value.First() == childPropertyName)
                    .ToDictionary(
                        i => i.Key,
                        i => (IList<string>)i.Value.Skip(1).ToList());

                if (propertyPath.StartsWith(modelPath))
                {
                    foreach (var rule in from kvp in propertiesToFind2
                                         where !kvp.Value.Any()
                                         select kvp.Key)
                    {
                        yield return (Path: propertyPath, Rule: rule);
                    }
                }

                propertiesToFind2 = propertiesToFind2
                    .Where(i => i.Value.Any())
                    .ToDictionary(
                        i => i.Key,
                        i => i.Value);

                if (propertyPath.StartsWith(modelPath) && propertyPath != modelPath &&
                    processed.Contains(childProperty.PropertyType) && !propertiesToFind2.Any())
                {
                    continue;
                }

                var childType = typeof(IEnumerable).IsAssignableFrom(childProperty.PropertyType) &&
                                childProperty.PropertyType.IsGenericType
                    ? childProperty.PropertyType.GetGenericArguments().First()
                    : childProperty.PropertyType;

                foreach (var p2 in childType.FindRulesForProperty(property, propertiesToFind2, processed, propertyPath, modelPath))
                {
                    yield return p2;
                }
            }
        }

        private static IEnumerable<PropertyInfo> GetInheritedProperties(MemberInfo property)
        {
            foreach (var prop in from iface in property.DeclaringType.Iterrate(t => t.BaseType)
                                 from p in iface.GetProperties()
                                 where p.Name == property.Name
                                 select p)
            {
                yield return prop;
            }

            foreach (var prop in from iface in property.DeclaringType.GetInterfaces()
                                 from p in iface.GetProperties()
                                 where p.Name == property.Name
                                 select p)
            {
                yield return prop;
            }
        }

        private static IEnumerable<(string Path, DryvRule Rule)> GetInheritedRules(Type modelType, MemberInfo property, string modelPath)
        {
            return (from prop in GetInheritedProperties(property)
                    from rule in modelType.FindRulesForProperty(prop, modelPath)
                    select rule)
                .Distinct()
                .ToList();
        }
    }
}