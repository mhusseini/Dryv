using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dryv
{
    internal class RulesFinder
    {
        private static readonly ConcurrentDictionary<PropertyInfo, IList<DryvRule>> PropertyRules = new ConcurrentDictionary<PropertyInfo, IList<DryvRule>>();
        private static readonly ConcurrentDictionary<Type, IList<DryvRules>> TypeRules = new ConcurrentDictionary<Type, IList<DryvRules>>();

        public static IEnumerable<DryvRule> GetRulesForProperty(PropertyInfo property)
        {
            return PropertyRules.GetOrAdd(
                    property,
                    prop => GetInheritedRules(prop).ToList());
        }

        private static IEnumerable<PropertyInfo> GetInheritedProperties(PropertyInfo property)
        {
            foreach (var prop in from iface in property.DeclaringType.GetInterfaces()
                                 from p in iface.GetProperties()
                                 where p.Name == property.Name
                                 select p)
            {
                yield return prop;
            }

            yield return property;
        }

        private static IEnumerable<DryvRule> GetInheritedRules(PropertyInfo prop)
        {
            var typeRules = GetRulesOnType(prop.DeclaringType);
            var properties = GetInheritedProperties(prop);

            return from p in properties
                   from rule in typeRules
                   where rule.PropertyRules.ContainsKey(p)
                   from expression in rule.PropertyRules[p]
                   select expression;
        }

        private static IEnumerable<DryvRules> GetRulesOnType(Type objectType) => TypeRules.GetOrAdd(objectType, t =>
        {
            var fromFields = from p in t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                             where typeof(DryvRules).IsAssignableFrom(p.FieldType)
                             select p.GetValue(null) as DryvRules;

            var fromProperties = from p in objectType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                 where typeof(DryvRules).IsAssignableFrom(p.PropertyType)
                                 select p.GetValue(null) as DryvRules;

            return fromFields.Union(fromProperties).ToList();
        });
    }
}