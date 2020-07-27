using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dryv.Rules
{
    public partial class DryvRules<TModel> : DryvRules
    {
        internal DryvRules()
        {
        }

        private void AddParameter(string name, Delegate parameter, params Type[] services)
        {
            var m = parameter.GetMethodInfo();
            var o = parameter.Target;
            var u = Expression.Parameter(typeof(object), "unused");
            var p = Expression.Parameter(typeof(object[]), "services");
            var args = services.Select((type, i) => (Expression)Expression.Convert(Expression.ArrayAccess(p, Expression.Constant(i)), type)).ToArray();

            var lambda = o == null
                ? Expression.Lambda<Func<object, object[], object>>(Expression.Convert(Expression.Call(m, args), typeof(object)), u, p)
                : Expression.Lambda<Func<object, object[], object>>(Expression.Convert(Expression.Call(Expression.Constant(o), m, args), typeof(object)), u, p);

            var ruleDefinition = DryvCompiledRule.CreateParameter(name, lambda, services);
            this.Parameters.Add(ruleDefinition);
        }

        private void Add<TProperty>(
            string groupName,
            Expression<Func<TModel, TProperty>> property,
            LambdaExpression rule,
            LambdaExpression enabled,
            string ruleName,
            DryvRuleLocation ruleLocation)
        {
            var ruleDefinition = DryvCompiledRule.Create(property, rule, enabled, ruleLocation, groupName);
            ruleDefinition.RuleType = RuleType.Validation;
            ruleDefinition.Name = ruleName;
            ruleDefinition.Parameters = this.Parameters;

            this.ValidationRules.Add(ruleDefinition);
        }

        private void Disable<TProperty>(
            Expression<Func<TModel, TProperty>> property,
            LambdaExpression rule,
            LambdaExpression enabled,
            DryvRuleLocation ruleLocation)
        {
            var ruleDefinition = DryvCompiledRule.Create(property, rule, enabled, ruleLocation, null);
            ruleDefinition.RuleType = RuleType.Disabling;
            ruleDefinition.Parameters = this.Parameters;

            this.DisablingRules.Add(ruleDefinition);
        }

        private void Disable<TProperty>(
            LambdaExpression rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            LambdaExpression ruleSwitch)
        {
            foreach (var property in properties)
            {
                this.Disable(property, rule, ruleSwitch, DryvRuleLocation.Server | DryvRuleLocation.Client);
            }
        }

        private void Add<TProperty>(
            string groupName,
            LambdaExpression rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            LambdaExpression ruleSwitch,
            string ruleName)
        {
            foreach (var property in properties)
            {
                this.Add(groupName, property, rule, ruleSwitch, ruleName, DryvRuleLocation.Server | DryvRuleLocation.Client);
            }
        }

        private void AddServer<TProperty>(
            string groupName,
            LambdaExpression rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            LambdaExpression ruleSwitch,
            string ruleName)
        {
            foreach (var property in properties)
            {
                this.Add(groupName, property, rule, ruleSwitch, ruleName, DryvRuleLocation.Server);
            }
        }

        private void AddClient<TProperty>(
            string groupName,
            LambdaExpression rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            LambdaExpression ruleSwitch,
        string ruleName)
        {
            foreach (var property in properties)
            {
                this.Add(groupName, property, rule, ruleSwitch, ruleName, DryvRuleLocation.Client);
            }
        }
    }
}