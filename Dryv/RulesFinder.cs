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
        private const BindingFlags MemberBindingFlags = BindingFlags.Static |
                                                        BindingFlags.Public |
                                                        BindingFlags.NonPublic |
                                                        BindingFlags.FlattenHierarchy;

        private static readonly ConcurrentDictionary<string, IList<DryvRuleNode>> PropertyRules = new ConcurrentDictionary<string, IList<DryvRuleNode>>();

        private static readonly ConcurrentDictionary<Type, IList<DryvRule>> TypeRules = new ConcurrentDictionary<Type, IList<DryvRule>>();

        public static IEnumerable<DryvRuleNode> GetRulesForProperty(this Type modelType, PropertyInfo property, string modelPath = "")
        {
            var key = $"{modelType.FullName}|{modelPath}|{property.DeclaringType.FullName}|{property.Name}";
            return PropertyRules.GetOrAdd(
                    key,
                    _ => GetInheritedRules(modelType, property, modelPath).ToList());
        }

        public static IEnumerable<DryvRule> GetRulesOnType(this Type objectType) => TypeRules.GetOrAdd(objectType, type =>
        {
            var fromFields = from p in type.GetFields(MemberBindingFlags)
                             where typeof(DryvRules).IsAssignableFrom(p.FieldType)
                             select p.GetValue(null) as DryvRules;

            var fromProperties = from p in type.GetProperties(MemberBindingFlags)
                                 where typeof(DryvRules).IsAssignableFrom(p.PropertyType)
                                 select p.GetValue(null) as DryvRules;

            var fromMethods = from m in type.GetMethods(MemberBindingFlags)
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
                new Dictionary<DryvRule, IList<string>>(),
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

        private static IEnumerable<DryvRuleNode> FindRulesForProperty(this Type type, PropertyInfo property, IDictionary<DryvRule, IList<string>> propertiesToFind, ISet<Type> processed, string path, string modelPath, ConcurrentDictionary<Type, List<DryvRule>> deferredRules)
        {
            var pathPrefix = path == null ? string.Empty : $"{path}.";
            const BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.Instance |
                                              BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            processed.Add(type);

            var deferredRulesForType = deferredRules.GetOrAdd(type, _ => new List<DryvRule>());

            foreach (var rule in from rule in deferredRulesForType.Union(type.GetRulesOnType())
                                 where rule.Property == property
                                 select rule)
            {
                var l = rule.ModelPath.Split(".")
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

                var targetType = rule.ValidationExpression.Parameters.First().Type;
                if (!l.Any() && targetType != type)
                {
                    deferredRules.GetOrAdd(targetType, _ => new List<DryvRule>()).Add(rule);
                }
                else
                {
                    l.Add(rule.Property.Name.ToCamelCase());
                    propertiesToFind[rule] = l;
                }
            }

            foreach (var childProperty in from prop in type.GetProperties(bindingFlags)
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
                        yield return new DryvRuleNode
                        {
                            Path = propertyPath,
                            Rule = rule
                        };
                    }
                }

                propertiesToFind2 = propertiesToFind2
                    .Where(i => i.Value.Any())
                    .ToDictionary(
                        i => i.Key,
                        i => i.Value);

                if (!childProperty.PropertyType.IsClass || childProperty.PropertyType.Namespace == "System" ||
                    (propertyPath.StartsWith(modelPath) && propertyPath != modelPath &&
                    processed.Contains(childProperty.PropertyType) && !propertiesToFind2.Any()))
                {
                    continue;
                }

                var childType = typeof(IEnumerable).IsAssignableFrom(childProperty.PropertyType) &&
                                childProperty.PropertyType.IsGenericType
                    ? childProperty.PropertyType.GetGenericArguments().First()
                    : childProperty.PropertyType;

                foreach (var p2 in childType.FindRulesForProperty(
                    property,
                    propertiesToFind2,
                    processed,
                    propertyPath,
                    modelPath,
                    deferredRules))
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

        private static IEnumerable<DryvRuleNode> GetInheritedRules(Type modelType, MemberInfo property, string modelPath)
        {
            return (from prop in GetInheritedProperties(property)
                    from rule in modelType.FindRulesForProperty(prop, modelPath)
                    select rule)
                .Distinct()
                .ToList();
        }
    }
}