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
        private const BindingFlags BindingFlagsForProperties = BindingFlags.FlattenHierarchy |
                                                               BindingFlags.Instance |
                                                               BindingFlags.Static |
                                                               BindingFlags.Public |
                                                               BindingFlags.NonPublic;

        private const BindingFlags BindingFlagsForRules = BindingFlags.Static |
                                                          BindingFlags.Public |
                                                          BindingFlags.NonPublic |
                                                          BindingFlags.FlattenHierarchy;

        private static readonly ConcurrentDictionary<string, IList<DryvRuleNode>> PropertyRules = new ConcurrentDictionary<string, IList<DryvRuleNode>>();

        private static readonly ConcurrentDictionary<Type, IList<DryvRule>> TypeRules = new ConcurrentDictionary<Type, IList<DryvRule>>();

        public static IEnumerable<DryvRuleNode> GetRulesForProperty(this Type modelType, PropertyInfo property, string modelPath = "")
        {
            return PropertyRules.GetOrAdd(
                $"{modelType.FullName}|{modelPath}|{property.DeclaringType.FullName}|{property.Name}",
                _ => GetInheritedRules(modelType, property, modelPath).ToList());
        }

        public static IEnumerable<DryvRule> GetRulesOnType(this Type objectType) => TypeRules.GetOrAdd(objectType, type =>
        {
            var fromFields = from p in type.GetFields(BindingFlagsForRules)
                             where typeof(DryvRules).IsAssignableFrom(p.FieldType)
                             select p.GetValue(null) as DryvRules;

            var fromProperties = from p in type.GetProperties(BindingFlagsForRules)
                                 where typeof(DryvRules).IsAssignableFrom(p.PropertyType)
                                 select p.GetValue(null) as DryvRules;

            var fromMethods = from m in type.GetMethods(BindingFlagsForRules)
                              where m.IsStatic
                                    && !m.GetParameters().Any()
                                    && typeof(DryvRules).IsAssignableFrom(m.ReturnType)
                                    && !m.ContainsGenericParameters
                              select m.Invoke(null, null) as DryvRules;

            return (from rules in fromFields.Union(fromProperties).Union(fromMethods)
                    from rule in rules.PropertyRules
                    select rule).ToList();
        });

        internal static IEnumerable<DryvRuleNode> FindRulesForProperty(this Type type, PropertyInfo property, string modelPath)
        {
            var tuples = type.FindRulesForProperty(
                property,
                new List<(DryvRule Rule, List<string> Path)>(),
                new HashSet<Type>(),
                null,
                modelPath,
                new ConcurrentDictionary<Type, List<DryvRule>>());
            return (from tuple in tuples
                    select new DryvRuleNode
                    {
                        Path = string.Join(".", tuple.Path.Split(".").Reverse().Skip(1).Reverse()),
                        Rule = tuple.Rule
                    }).ToList();
        }

        private static List<(DryvRule Rule, List<string> Path)> AdvancePathForCurrentProperty(IList<(DryvRule Rule, List<string> Path)> propertiesToFind, string childPropertyName)
        {
            return propertiesToFind
                .Where(pp => pp.Path.First() == childPropertyName)
                .Select(i => (Rule: i.Rule, Path: i.Path.Skip(1).ToList()))
                .ToList();
        }

        private static IEnumerable<DryvRuleNode> FindRulesForProperty(this Type type, PropertyInfo property, IList<(DryvRule Rule, List<string> Path)> propertiesToFind, ISet<Type> processed, string path, string modelPath, ConcurrentDictionary<Type, List<DryvRule>> deferredRules)
        {
            var pathPrefix = string.IsNullOrWhiteSpace(path) ? string.Empty : $"{path}.";
            processed.Add(type);

            var deferredRulesForType = deferredRules.GetOrAdd(type, _ => new List<DryvRule>());

            foreach (var rule in from rule in deferredRulesForType.Union(type.GetRulesOnType())
                                 where rule.Property == property
                                 select rule)
            {
                var pathToModelInRule = rule.ModelPath.Split(".")
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

                var targetType = rule.ValidationExpression.Parameters.First().Type;
                var currentTypeHierarchy = GetBaseTypesAndInterfaces(type).ToHashSet();

                if (!pathToModelInRule.Any() && !currentTypeHierarchy.Contains(targetType))
                {
                    deferredRules.GetOrAdd(targetType, _ => new List<DryvRule>()).Add(rule);
                }
                else
                {
                    pathToModelInRule.Add(rule.Property.Name.ToCamelCase());
                    propertiesToFind.Add((Rule: rule, Path: pathToModelInRule));
                }
            }

            foreach (var childProperty in type.GetProperties(BindingFlagsForProperties))
            {
                var childPropertyName = childProperty.Name.ToCamelCase();
                var pathToCurrentProperty = $"{pathPrefix}{childPropertyName}";
                var propertiesToFind2 = AdvancePathForCurrentProperty(propertiesToFind, childPropertyName);

                if (pathToCurrentProperty.StartsWith(modelPath))
                {
                    foreach (var rule in GetRulesAtCurrrentPath(propertiesToFind2))
                    {
                        yield return new DryvRuleNode
                        {
                            Path = pathToCurrentProperty,
                            Rule = rule
                        };
                    }
                }

                propertiesToFind2 = RemoveRulesAtCurrentPath(propertiesToFind2);

                if (IsNotANavigationProperty(childProperty) ||
                    pathToCurrentProperty.StartsWith(modelPath) && pathToCurrentProperty != modelPath &&
                    processed.Contains(childProperty.PropertyType) && !propertiesToFind2.Any())
                {
                    continue;
                }

                var childType = GetChildType(childProperty);

                foreach (var p2 in childType.FindRulesForProperty(
                    property,
                    propertiesToFind2,
                    processed,
                    pathToCurrentProperty,
                    modelPath,
                    deferredRules))
                {
                    yield return p2;
                }
            }
        }

        private static IEnumerable<Type> GetBaseTypesAndInterfaces(Type type)
        {
            var currentTypes = type.Iterrate(t => t.BaseType).ToList();
            var currentTypeHierarchy = (from t in currentTypes
                                        from i in t.GetInterfaces()
                                        select i).Union(currentTypes);
            return currentTypeHierarchy;
        }

        private static Type GetChildType(PropertyInfo childProperty)
        {
            return typeof(IEnumerable).IsAssignableFrom(childProperty.PropertyType) &&
                   childProperty.PropertyType.IsGenericType
                ? childProperty.PropertyType.GetGenericArguments().First()
                : childProperty.PropertyType;
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

        private static IEnumerable<DryvRuleNode> GetInheritedRules(Type modelType, MemberInfo property, string modelPath)
        {
            return (from prop in GetInheritedProperties(property)
                    from rule in modelType.FindRulesForProperty(prop, modelPath)
                    select rule)
                .Distinct()
                .ToList();
        }

        private static IEnumerable<DryvRule> GetRulesAtCurrrentPath(List<(DryvRule Rule, List<string> Path)> propertiesToFind2)
        {
            return from kvp in propertiesToFind2
                   where !kvp.Path.Any()
                   select kvp.Rule;
        }

        private static bool IsNotANavigationProperty(PropertyInfo childProperty)
        {
            return !childProperty.PropertyType.IsClass || childProperty.PropertyType.Namespace == "System";
        }

        private static List<(DryvRule Rule, List<string> Path)> RemoveRulesAtCurrentPath(List<(DryvRule Rule, List<string> Path)> propertiesToFind2)
        {
            return propertiesToFind2
                .Where(i => i.Path.Any())
                .ToList();
        }
    }
}