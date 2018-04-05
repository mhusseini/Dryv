using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dryv.Utils;

namespace Dryv
{
    internal class RulesFinder
    {
        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Static |
                                          System.Reflection.BindingFlags.Public |
                                          System.Reflection.BindingFlags.NonPublic |
                                          System.Reflection.BindingFlags.FlattenHierarchy;

        private static readonly ConcurrentDictionary<string, IList<DryvRule>> PropertyRules = new ConcurrentDictionary<string, IList<DryvRule>>();
        private static readonly ConcurrentDictionary<Type, IList<DryvRules>> TypeRules = new ConcurrentDictionary<Type, IList<DryvRules>>();

        public static IEnumerable<DryvRule> GetRulesForProperty(Type modelType, PropertyInfo property)
        {
            var key = $"{modelType.FullName}|{property.DeclaringType.FullName}|{property.Name}";
            return PropertyRules.GetOrAdd(
                    key,
                    _ => GetInheritedRules(modelType, property).ToList());
        }

        private static IEnumerable<PropertyInfo> GetInheritedProperties(PropertyInfo property)
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

        private static IEnumerable<DryvRule> GetInheritedRules(Type modelType, PropertyInfo prop)
        {
            var typeRules = GetRulesOnType(modelType);
            var properties = GetInheritedProperties(prop);

            return from p in properties
                   from rule in typeRules
                   where rule.PropertyRules.ContainsKey(p)
                   from expression in rule.PropertyRules[p]
                   select expression;
        }

        private static IEnumerable<DryvRules> GetRulesOnType(Type objectType) => TypeRules.GetOrAdd(objectType, type =>
        {
            var typeHierarchy = type.GetObjecTypeHierarchy().ToList();

            var fromFields = from t in typeHierarchy
                             from p in t.GetFields(BindingFlags)
                             where typeof(DryvRules).IsAssignableFrom(p.FieldType)
                             select p.GetValue(null) as DryvRules;

            var fromProperties = from t in typeHierarchy
                                 from p in objectType.GetProperties(BindingFlags)
                                 where typeof(DryvRules).IsAssignableFrom(p.PropertyType)
                                 select p.GetValue(null) as DryvRules;

            var fromMethods = from t in typeHierarchy
                              from m in objectType.GetMethods(BindingFlags)
                              where m.IsStatic
                                    && !m.GetParameters().Any()
                                    && typeof(DryvRules).IsAssignableFrom(m.ReturnType)
                              select m.Invoke(null, null) as DryvRules;

            return fromFields.Union(fromProperties).Union(fromMethods).ToList();
        });
    }
}