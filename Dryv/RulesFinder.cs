using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dryv.Reflection;
using Dryv.Utils;

namespace Dryv
{
    internal static class RulesFinder
    {
        private const BindingFlags BindingFlagsForProperties = BindingFlags.FlattenHierarchy |
                                                               BindingFlags.Instance |
                                                               BindingFlags.Public |
                                                               BindingFlags.NonPublic;

        private const BindingFlags BindingFlagsForRules = BindingFlags.Static |
                                                          BindingFlags.Public |
                                                          BindingFlags.NonPublic |
                                                          BindingFlags.FlattenHierarchy;

        private static readonly ConcurrentDictionary<string, IList<DryvRuleNode>> PropertyRules = new ConcurrentDictionary<string, IList<DryvRuleNode>>();

        private static readonly ConcurrentDictionary<Type, IList<DryvRuleDefinition>> TypeRules = new ConcurrentDictionary<Type, IList<DryvRuleDefinition>>();

        public static IEnumerable<DryvRuleNode> GetRulesForProperty(Type modelType, PropertyInfo property, string modelPath = "")
        {
            return PropertyRules.GetOrAdd(
                $"{modelType.FullName}|{modelPath}|{property.DeclaringType.FullName}|{property.Name}",
                _ => GetInheritedRules(modelType, property, modelPath));
        }

        public static IEnumerable<DryvRuleDefinition> GetRulesOnType(Type objectType) => TypeRules.GetOrAdd(objectType, type =>
        {
            var typeInfo = type.GetTypeInfo();

            var fromFields = from p in typeInfo.GetFields(BindingFlagsForRules)
                             where typeof(DryvRules).IsAssignableFrom(p.FieldType)
                             select p.GetValue(null) as DryvRules;

            var fromProperties = from p in typeInfo.GetProperties(BindingFlagsForRules)
                                 where typeof(DryvRules).IsAssignableFrom(p.PropertyType)
                                 select p.GetValue(null) as DryvRules;

            var fromMethods = from m in typeInfo.GetMethods(BindingFlagsForRules)
                              where m.IsStatic
                                    && !m.GetParameters().Any()
                                    && typeof(DryvRules).IsAssignableFrom(m.ReturnType)
                                    && !m.ContainsGenericParameters
                              select m.Invoke(null, null) as DryvRules;

            return (from rules in fromFields.Union(fromProperties).Union(fromMethods)
                    from rule in rules.PropertyRules
                    select rule).ToList();
        });

        private static IEnumerable<DryvRuleNode> FindRulesForProperty(Type type, PropertyInfo property, string modelPath)
        {
            var tuples = FindRulesForProperty(type,
                property,
                new List<RulePaths>(),
                new HashSet<Type>(),
                null,
                modelPath,
                new ConcurrentDictionary<Type, List<DryvRuleDefinition>>());

            return (from tuple in tuples
                    select new DryvRuleNode
                    {
                        Path = string.Join(".", tuple.Path.Split('.').Reverse().Skip(1).Reverse()),
                        Rule = tuple.Rule
                    }).ToList();
        }

        private static List<RulePaths> AdvancePathForCurrentProperty(IList<RulePaths> propertiesToFind, string childPropertyName)
        {
            return propertiesToFind
                .Where(pp => pp.Path.First() == childPropertyName)
                .Select(i => new RulePaths(i.Rule, i.Path.Skip(1).ToList()))
                .ToList();
        }

        private static void DeterminePropertyPathOrDefer(Type type, ICollection<RulePaths> propertiesToFind, ConcurrentDictionary<Type, List<DryvRuleDefinition>> deferredRules, DryvRuleDefinition rule)
        {
            var targetType = rule.ValidationExpression.Parameters.First().Type;
            var currentTypeHierarchy = type.GetTypeHierarchyAndInterfaces().ToHashSet();
            var pathToPropertyInRule = rule.ModelPath.Split('.')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            if (!pathToPropertyInRule.Any() && !currentTypeHierarchy.Contains(targetType))
            {
                // The rule is defined on the current type, but it doesnt affect the current
                // type at all --> deferr rule until we're on the type matching the rule.
                deferredRules.GetOrAdd(targetType, _ => new List<DryvRuleDefinition>()).Add(rule);
            }
            else
            {
                // From the current node in the model tree, calculate
                // the path to the property that for which the rule is defined.
                pathToPropertyInRule.Add(rule.Property.Name.ToCamelCase());
                propertiesToFind.Add(new RulePaths(rule, pathToPropertyInRule));
            }
        }

        private static IEnumerable<DryvRuleNode> FindRulesForProperty(Type type, PropertyInfo property, IList<RulePaths> propertiesToFind, ISet<Type> processed, string path, string modelPath, ConcurrentDictionary<Type, List<DryvRuleDefinition>> deferredRules)
        {
            processed.Add(type);

            var pathPrefix = string.IsNullOrWhiteSpace(path) ? string.Empty : $"{path}.";
            var deferredRulesForType = deferredRules.GetOrAdd(type, _ => new List<DryvRuleDefinition>());

            foreach (var rule in from rule in deferredRulesForType.Union(GetRulesOnType(type))
                                 where Equals(rule.Property, property)
                                 select rule)
            {
                DeterminePropertyPathOrDefer(type, propertiesToFind, deferredRules, rule);
            }

            foreach (var ruleNode in from childProperty in type.GetProperties(BindingFlagsForProperties)
                                     from ruleNode in FindRulesForPropertyInChild(
                                         property,
                                         propertiesToFind,
                                         processed,
                                         modelPath,
                                         deferredRules,
                                         childProperty,
                                         pathPrefix)
                                     select ruleNode)
            {
                yield return ruleNode;
            }
        }

        /// <summary>
        /// Finds rules for the specified property on an object contained in the specified "child property".
        /// </summary>
        /// <param name="property">The property to find the rules for.</param>
        /// <param name="propertiesToFind">Paths to the specified property in relation to already found rules.</param>
        /// <param name="processed">Types that are already processed and should be ignores.</param>
        /// <param name="modelPath">The base path to the model.</param>
        /// <param name="deferredRules">Rules that were found on non-matching types.</param>
        /// <param name="childProperty">A property that might contain a child object that has the property for which the rules should be found.</param>
        /// <param name="pathPrefix">The current property model path.</param>
        /// <returns></returns>
        private static IEnumerable<DryvRuleNode> FindRulesForPropertyInChild(PropertyInfo property, IList<RulePaths> propertiesToFind, ISet<Type> processed, string modelPath, ConcurrentDictionary<Type, List<DryvRuleDefinition>> deferredRules, PropertyInfo childProperty, string pathPrefix)
        {
            var childType = childProperty.GetElementType();
            var childPropertyName = childProperty.Name.ToCamelCase();
            var pathToChildProperty = $"{pathPrefix}{childPropertyName}";

            propertiesToFind = AdvancePathForCurrentProperty(propertiesToFind, childPropertyName);

            foreach (var node in GetRulesAtCurrentPath(propertiesToFind, modelPath, pathToChildProperty))
            {
                yield return node;
            }

            propertiesToFind = RemoveRulesAtCurrentPath(propertiesToFind);

            if (!childProperty.IsNavigationProperty() ||
                pathToChildProperty.StartsWith(modelPath) && pathToChildProperty != modelPath &&
                processed.Contains(childProperty.PropertyType) && !propertiesToFind.Any())
            {
                yield break;
            }

            foreach (var node in FindRulesForProperty(
                childType,
                property,
                propertiesToFind,
                processed,
                pathToChildProperty,
                modelPath,
                deferredRules.Clone()))
            {
                yield return node;
            }
        }

        private static IList<DryvRuleNode> GetInheritedRules(Type modelType, PropertyInfo property, string modelPath)
        {
            return property.GetCustomAttribute<DryvRulesAttribute>() == null
                ? (IList<DryvRuleNode>)new DryvRuleNode[0]
                : (from prop in property.GetInheritedProperties()
                   from rule in FindRulesForProperty(modelType, prop, modelPath)
                   select rule)
                .Distinct()
                .ToList();
        }

        private static IEnumerable<DryvRuleNode> GetRulesAtCurrentPath(
            IEnumerable<RulePaths> propertiesToFind,
            string modelPath,
            string pathToChildProperty)
        {
            if (!pathToChildProperty.StartsWith(modelPath))
            {
                yield break;
            }

            foreach (var rule in from kvp in propertiesToFind
                                 where !kvp.Path.Any()
                                 select kvp.Rule)
            {
                yield return new DryvRuleNode
                {
                    Path = pathToChildProperty,
                    Rule = rule
                };
            }
        }

        private static List<RulePaths> RemoveRulesAtCurrentPath(IEnumerable<RulePaths> propertiesToFind2)
        {
            return propertiesToFind2
                .Where(i => i.Path.Any())
                .ToList();
        }

        private struct RulePaths
        {
            public RulePaths(DryvRuleDefinition rule, List<string> path)
            {
                this.Rule = rule;
                this.Path = path;
            }

            public List<string> Path { get; }
            public DryvRuleDefinition Rule { get; }
        }
    }
}