using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dryv.Extensions;
using Dryv.Reflection;
using Dryv.Rules;

namespace Dryv.RuleDetection
{
    public class DryvRulesFinder
    {
        private const BindingFlags BindingFlagsForProperties = BindingFlags.FlattenHierarchy |
                                                               BindingFlags.Instance |
                                                               BindingFlags.Public |
                                                               BindingFlags.NonPublic;

        private const BindingFlags BindingFlagsForRules = BindingFlags.Static |
                                                          BindingFlags.Public |
                                                          BindingFlags.NonPublic |
                                                          BindingFlags.FlattenHierarchy;

        private static readonly ConcurrentDictionary<string, IEnumerable<DryvCompiledRule>> CompiledRuleCache = new ConcurrentDictionary<string, IEnumerable<DryvCompiledRule>>();

        private static readonly ConcurrentDictionary<string, IDictionary<PropertyInfo, IEnumerable<DryvRuleTreeNode>>> RuleTreeNodeCache = new ConcurrentDictionary<string, IDictionary<PropertyInfo, IEnumerable<DryvRuleTreeNode>>>();

        public IDictionary<PropertyInfo, IEnumerable<DryvRuleTreeNode>> FindValidationRules<T>(string modelPath = null, RuleType ruleType = RuleType.Default)
        {
            return this.GetRulesDeclaredIn(typeof(T), modelPath, ruleType);
        }

        public IDictionary<PropertyInfo, IEnumerable<DryvRuleTreeNode>> GetRulesDeclaredIn(Type rootModelType, string modelPath = null, RuleType ruleType = RuleType.Default)
        {
            return RuleTreeNodeCache.GetOrAdd($"{rootModelType.FullName}|{ruleType}|{modelPath ?? string.Empty}", _ =>
              {
                  var type = rootModelType.GetElementType() ?? rootModelType;
                  var types = new List<Type>();
                  var pathStack = new Queue<string>();
                  if (!string.IsNullOrWhiteSpace(modelPath))
                  {
                      pathStack.Enqueue(modelPath);
                  }

                  var properties = this.TraverseTypeTreeForProperties(type, types, pathStack, ruleType).ToList();
                  types.AddRange((from t in properties.Select(p => p.Key.DeclaringType).Distinct()
                                  from a in t.GetTypeInfo().GetCustomAttributes<DryvValidationAttribute>()
                                  where a.RuleContainerType != null
                                  select a.RuleContainerType).Distinct());
                  var validationRules = FindValidationRulesOnTypes(types.Distinct(), ruleType);

                  var g = from item in properties
                          let property = item.Key
                          let path = item.Value
                          let propertyRules = FindValidationRulesForProperty(property, validationRules, ruleType)
                          from inheritedProperty in property.GetInheritedProperties()
                          from rule in GetElementsFromDictionary(validationRules, inheritedProperty)
                          where rule != null
                          let node = new DryvRuleTreeNode(path, rule)
                          group node by property;

                  return g.ToDictionary(i => i.Key, i => i.AsEnumerable(), PropertyComparer.Default);
              });
        }

        public IEnumerable<DryvRuleTreeNode> GetRulesForProperty(Type rootModelType, PropertyInfo property, RuleType ruleType, string modelPath = null)
        {
            return !this
                .GetRulesDeclaredIn(rootModelType, modelPath, ruleType)
                .TryGetValue(property, out var nodes)
                ? new DryvRuleTreeNode[0]
                : nodes;
        }

        private static IDictionary<PropertyInfo, List<DryvCompiledRule>> FindValidationRulesForProperty(MemberInfo property, IDictionary<PropertyInfo, List<DryvCompiledRule>> commonValidationRules, RuleType ruleType)
        {
            var attributes = (from a in property.GetCustomAttributes<DryvValidationAttribute>()
                              where a.RuleContainerType != null
                              select a).ToList();

            if (!attributes.Any())
            {
                return commonValidationRules;
            }

            var types = attributes.Select(a => a.RuleContainerType);
            var result = FindValidationRulesOnTypes(types.Distinct(), ruleType);
            result.AddRange(commonValidationRules);

            return result;
        }

        private static IEnumerable<DryvCompiledRule> FindValidationRulesOnType(Type type, RuleType ruleType)
        {
            return CompiledRuleCache.GetOrAdd($"{type.FullName}|{ruleType}", _ =>
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
                        from rule in GetRulesOfType(rules, ruleType)
                        select rule).ToList();
            });
        }

        private static IDictionary<PropertyInfo, List<DryvCompiledRule>> FindValidationRulesOnTypes(IEnumerable<Type> types, RuleType ruleType)
        {
            var typesWithBaseTypes = (from type in types
                                      from t2 in type.Iterate(t => t.GetBaseType())
                                      select t2).Distinct();

            return (from type in typesWithBaseTypes
                    from rule in FindValidationRulesOnType(type, ruleType)
                    group rule by rule.Property)
                .ToDictionary(r => r.Key, r => r.Distinct().ToList(), PropertyComparer.Default);
        }

        private static IEnumerable<TValue> GetElementsFromDictionary<TKey, TValue>(IDictionary<TKey, List<TValue>> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var value) ? (IEnumerable<TValue>)value : new TValue[0];
        }

        private static IEnumerable<DryvCompiledRule> GetRulesOfType(DryvRules rules, RuleType ruleType)
        {
            return ruleType switch
            {
                RuleType.Disabling => rules.DisablingRules,
                _ => rules.PropertyRules
            };
        }

        private IEnumerable<KeyValuePair<PropertyInfo, string>> TraverseTypeTreeForProperties(Type type, ICollection<Type> foundTypes, Queue<string> pathStack = null, RuleType ruleType = RuleType.Default)
        {
            if (foundTypes.Contains(type))
            {
                yield break;
            }

            foundTypes.Add(type);

            var isDryvEnabled = type.GetTypeInfo().GetCustomAttribute<DryvValidationAttribute>() != null;

            foreach (var property in type.GetProperties(BindingFlagsForProperties))
            {
                if (property.IsNavigationProperty())
                {
                    pathStack.Enqueue(property.Name.ToCamelCase());

                    if (ruleType == RuleType.Disabling && property.GetCustomAttribute<DryvValidationAttribute>() != null)
                    {
                        yield return new KeyValuePair<PropertyInfo, string>(property, string.Join(".", pathStack));
                    }

                    var childType = property.PropertyType.GetElementType() ?? property.PropertyType;

                    foreach (var item in this.TraverseTypeTreeForProperties(childType, foundTypes, pathStack, ruleType))
                    {
                        yield return item;
                    }

                    pathStack.Dequeue();
                }
                else if (isDryvEnabled || property.GetCustomAttribute<DryvValidationAttribute>() != null)
                {
                    yield return new KeyValuePair<PropertyInfo, string>(property, string.Join(".", pathStack));
                }
            }
        }
    }
}