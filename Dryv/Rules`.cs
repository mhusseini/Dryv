using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Utils;

namespace Dryv
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
            RuleEvaluationLocation ruleLocation)
        {
            var ruleDefinition = DryvRuleDefinition.Create(property, rule, enabled, ruleLocation);

            this.PropertyRules.Add(ruleDefinition);
        }

        private void Add<TProperty>(
            LambdaExpression rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            LambdaExpression ruleSwitch)
        {
            foreach (var property in properties)
            {
                this.Add(property, rule, ruleSwitch, RuleEvaluationLocation.Server | RuleEvaluationLocation.Client);
            }
        }

        private void AddServer<TProperty>(
            LambdaExpression rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            LambdaExpression ruleSwitch)
        {
            foreach (var property in properties)
            {
                this.Add(property, rule, ruleSwitch, RuleEvaluationLocation.Server);
            }
        }

        private void AddClient<TProperty>(
            LambdaExpression rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            LambdaExpression ruleSwitch)
        {
            foreach (var property in properties)
            {
                this.Add(property, rule, ruleSwitch, RuleEvaluationLocation.Client);
            }
        }
    }
}