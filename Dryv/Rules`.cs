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
            if (!(property.Body is MemberExpression memberExpression) ||
                !(memberExpression.Member is PropertyInfo propertyInfo))
            {
                return;
            }

            var members = memberExpression
                .Iterrate(e => e.Expression as MemberExpression)
                .ToList();

            var parameter = (ParameterExpression)members.Last().Expression;

            var modelPath = string.Join(".", members
                .Skip(1)
                .Select(e => e.Member.Name.ToCamelCase())
                .Reverse());

            this.PropertyRules.Add(new DryvRuleDefinition
            {
                PropertyExpression = memberExpression,
                Property = propertyInfo,
                ModelPath = modelPath,
                ModelType = parameter.Type,
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