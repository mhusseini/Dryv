using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dryv.Extensions;

namespace Dryv.Rules
{
    internal class DryvRuleTreeBuilder : IDryvRuleTreeBuilder
    {
        private readonly ICollection<IDryvRuleSource> defaultSources;

        public DryvRuleTreeBuilder(ICollection<IDryvRuleSource> sources)
        {
            this.defaultSources = sources;
        }

        public IReadOnlyCollection<DryvRuleSet> Build(Type root)
        {
            return this.Build(root, null);
        }

        public IReadOnlyCollection<DryvRuleSet> Build(Type root, string ruleSetName)
        {
            return this.Build(root, null, null);
        }

        public IReadOnlyCollection<DryvRuleSet> Build(Type root, string ruleSetName, IEnumerable<IDryvRuleSource> sources)
        {
            var rules = GetRules(null, root, ruleSetName, sources?.ToList(), new List<DryvRule>(), new HashSet<(Type, Type)>()).ToList();

            throw new NotImplementedException();
        }

        private IEnumerable<DryveRuleTreeNode> GetRules(Type parent, Type type, string ruleSetName, ICollection<IDryvRuleSource> sources, IEnumerable<DryvRule> contextRules, ISet<(Type, Type)> processed)
        {
            if (!type.IsComplexType() || processed.Contains((parent, type)))
            {
                yield break;
            }

            processed.Add((parent, type));

            var rules = (from source in sources ?? this.defaultSources
                         from rule in contextRules.Concat(source.GetRules(type, ruleSetName))
                         select rule).ToList();

            var ruleDictionary = (from rule in rules
                                  from prop in rule.Properties
                                  group rule by prop.Property)
                .Distinct()
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public))
            {
                var node = new DryveRuleTreeNode
                {
                    Property = prop,
                    Rules = ruleDictionary.ContainsKey(prop) ? ruleDictionary[prop] : new List<DryvRule>(),
                    Children = GetRules(type, prop.PropertyType, ruleSetName, sources, rules, processed).ToList()
                };

                if (node.Rules.Count > 0 || node.Children.Count > 0)
                {
                    yield return node;
                }
            }
        }
    }
}