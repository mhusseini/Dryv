using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dryv.Cache;
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

        private readonly ICache cache;

        public DryvRulesFinder(ICache cache)
        {
            this.cache = cache;
        }

        public IDictionary<PropertyInfo, IEnumerable<DryvRuleTreeNode>> FindValidationRules<T>(string modelPath = null)
        {
            return this.GetRulesDeclaredIn(typeof(T), modelPath);
        }

        public IDictionary<PropertyInfo, IEnumerable<DryvRuleTreeNode>> GetRulesDeclaredIn(Type rootModelType, string modelPath = null)
        {
            return this.cache.GetOrAdd($"{rootModelType.FullName}:{modelPath ?? string.Empty}", () =>
              {
                  var type = rootModelType.GetElementType() ?? rootModelType;
                  var types = new List<Type>();
                  var pathStack = new Queue<string>();
                  if (!string.IsNullOrWhiteSpace(modelPath))
                  {
                      pathStack.Enqueue(modelPath);
                  }
                  var properties = this.TraverseTypeTreeForProperties(type, types, pathStack).ToDictionary(i => i.Key, i => i.Value);
                  var validationRules = this.FindValidationRulesOnTypes(types);

                  var g = from item in properties
                          let property = item.Key
                          let path = item.Value
                          from inheritedProperty in property.GetInheritedProperties()
                          from rule in GetElementsFromDictionary(validationRules, inheritedProperty)
                          where rule != null
                          let node = new DryvRuleTreeNode(path, rule)
                          group node by property;

                  return g.ToDictionary(i => i.Key, i => i.AsEnumerable(), PropertyComparer.Default);
              });
        }

        public IEnumerable<DryvRuleTreeNode> GetRulesForProperty(Type rootModelType, PropertyInfo property, string modelPath = null)
        {
            return !this
                .GetRulesDeclaredIn(rootModelType, modelPath)
                .TryGetValue(property, out var nodes)
                ? new DryvRuleTreeNode[0]
                : nodes;
        }

        private static IEnumerable<TValue> GetElementsFromDictionary<TKey, TValue>(IDictionary<TKey, List<TValue>> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var value) ? (IEnumerable<TValue>)value : new TValue[0];
        }

        private IEnumerable<DryvCompiledRule> FindValidationRulesOnType(Type type)
        {
            return this.cache.GetOrAdd(type.FullName, () =>
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
        }

        private IDictionary<PropertyInfo, List<DryvCompiledRule>> FindValidationRulesOnTypes(IEnumerable<Type> types)
        {
            return (from type in types
                    from rule in this.FindValidationRulesOnType(type)
                    group rule by rule.Property)
                .ToDictionary(r => r.Key, r => r.Distinct().ToList(), PropertyComparer.Default);
        }

        private IEnumerable<KeyValuePair<PropertyInfo, string>> TraverseTypeTreeForProperties(Type type, ICollection<Type> foundTypes, Queue<string> pathStack = null)
        {
            if (foundTypes.Contains(type))
            {
                yield break;
            }

            foundTypes.Add(type);

            foreach (var property in type.GetProperties(BindingFlagsForProperties))
            {
                if (property.IsNavigationProperty())
                {
                    pathStack.Enqueue(property.Name.ToCamelCase());

                    var childType = property.PropertyType.GetElementType() ?? property.PropertyType;

                    foreach (var item in this.TraverseTypeTreeForProperties(childType, foundTypes, pathStack))
                    {
                        yield return item;
                    }

                    pathStack.Dequeue();
                }
                else if (property.GetCustomAttribute<DryvRulesAttribute>() != null)
                {
                    yield return new KeyValuePair<PropertyInfo, string>(property, string.Join(".", pathStack));
                }
            }
        }
    }
}