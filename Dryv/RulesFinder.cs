using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dryv
{
    internal class RulesFinder
    {
        private static readonly ConcurrentDictionary<Type, IList<DryvRules>> TypeRules = new ConcurrentDictionary<Type, IList<DryvRules>>();

        public static IEnumerable<DryvRule> GetRulesForProperty(PropertyInfo property)
        {
            var typeRules = GetRulesForType(property.DeclaringType);

            return from rules in typeRules
                   where rules.PropertyRules.ContainsKey(property)
                   from expression in rules.PropertyRules[property]
                   select expression;
        }

        private static IEnumerable<DryvRules> GetRulesForType(Type objectType) => TypeRules.GetOrAdd(objectType, t =>
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