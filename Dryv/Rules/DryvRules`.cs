using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dryv.Rules
{
    public partial class DryvRules<TModel> : DryvRules
    {
        internal DryvRules()
        {
        }

        private void Add<TProperty>(
            Expression<Func<TModel, TProperty>> property,
            LambdaExpression rule,
            LambdaExpression enabled,
            DryvRuleLocation ruleLocation)
        {
            var ruleDefinition = DryvCompiledRule.Create(property, rule, enabled, ruleLocation);

            this.PropertyRules.Add(ruleDefinition);
        }

        private void Add<TProperty>(
            LambdaExpression rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            LambdaExpression ruleSwitch)
        {
            foreach (var property in properties)
            {
                this.Add(property, rule, ruleSwitch, DryvRuleLocation.Server | DryvRuleLocation.Client);
            }
        }

        private void AddServer<TProperty>(
            LambdaExpression rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            LambdaExpression ruleSwitch)
        {
            foreach (var property in properties)
            {
                this.Add(property, rule, ruleSwitch, DryvRuleLocation.Server);
            }
        }

        private void AddClient<TProperty>(
            LambdaExpression rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            LambdaExpression ruleSwitch)
        {
            foreach (var property in properties)
            {
                this.Add(property, rule, ruleSwitch, DryvRuleLocation.Client);
            }
        }
    }
}