using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Dryv
{
    public partial class Rules<TModel> : DryvRules
    {
        internal Rules()
        {
        }

        internal Rules(Expression<Func<TModel, DryvResult>> rule)
        {
            this.ModelRules.Add(rule);
        }

        public Rules<TModel> Rule(Expression<Func<TModel, DryvResult>> rule)
        {
            this.ModelRules.Add(rule);
            return this;
        }

        private void Add<TProperty>(
            Expression<Func<TModel, TProperty>> property,
            LambdaExpression rule,
            LambdaExpression enabled,
            RuleEvaluationLocation ruleLocation)
        {
            if (!(property.Body is MemberExpression memberExpression) ||
                !(memberExpression.Member is PropertyInfo propertyInfo))
            {
                return;
            }

            var expressions = this.PropertyRules.GetOrAdd(propertyInfo, _ => new List<DryvRule>());
            expressions.Add(new DryvRule
            {
                ValidationExpression = rule,
                EnablingExpression = enabled,
                EvaluationLocation = ruleLocation
            });
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