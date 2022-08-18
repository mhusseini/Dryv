using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dryv.Rules.Sources
{
    public class ReflectionDryvRuleSource : IDryvRuleSource
    {
        public IReadOnlyCollection<DryvRule> GetRules(Type root)
        {
            return this.GetRules(root, null);
        }

        public IReadOnlyCollection<DryvRule> GetRules(Type root, string ruleSetName)
        {
            var fields = root
                .GetFields(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => IsDryvRule(field.FieldType))
                .Select(field => field.GetValue(null));

            var properties = root
                .GetProperties(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(prop => IsDryvRule(prop.PropertyType))
                .Select(prop => prop.GetValue(null));

            var methods = root
                .GetMethods(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(method => IsDryvRule(method.ReturnType) && method.GetParameters().Length == 0)
                .Select(method => method.Invoke(null, new object[0]));

            var rules = fields
                .Union(properties)
                .Union(methods)
                .SelectMany(value => value switch
                {
                    IEnumerable<DryvRules> list => list,
                    DryvRules rule => new[] { rule },
                    _ => new DryvRules[0]
                })
                .SelectMany(r=>r.Rules)
                .Where(rule => rule != null && (rule.RuleSetName == null || rule.RuleSetName.Equals(ruleSetName, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (var rule in rules)
            {
                rule.DeclaringType = root;
            }

            return rules;
        }

        private static bool IsDryvRule(Type type)
        {
            return type == typeof(DryvRules) || IsDryvRuleList(type);
        }

        private static bool IsDryvRuleList(Type type)
        {
            return typeof(IEnumerable<DryvRules>).IsAssignableFrom(type);
        }
    }
}