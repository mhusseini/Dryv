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

        private static LambdaExpression DelegateToLambda(Delegate rule, Type[] services, bool ignoreModel = false)
        {
            if (rule == null)
            {
                return null;
            }

            var m = rule.GetMethodInfo();
            var o = rule.Target;
            var parameters = new List<ParameterExpression>();
            if (!ignoreModel)
            {
                parameters.Add(Expression.Parameter(typeof(TModel), "m"));
            }

            parameters.AddRange(services.Select((type, i) => Expression.Parameter(type, "p" + i)));

            var lambda = o == null
                ? Expression.Lambda(Expression.Call(m, parameters), parameters)
                : Expression.Lambda(Expression.Call(Expression.Constant(o), m, parameters), parameters);
            return lambda;
        }

        private void Add<TProperty>(
            Expression<Func<TModel, TProperty>> property,
            LambdaExpression rule,
            LambdaExpression enabled,
            DryvRuleSettings settings,
            DryvRuleLocation ruleLocation)
        {
            var ruleDefinition = DryvCompiledRule.Create(property, rule, enabled, ruleLocation, null);
            ruleDefinition.RuleType = RuleType.Validation;
            ruleDefinition.Parameters = this.Parameters;

            if (settings != null)
            {
                ruleDefinition.Annotations = settings;
                ruleDefinition.Name = settings.Name;
                ruleDefinition.GroupName = settings.GroupName;
            }

            this.ValidationRules.Add(ruleDefinition);
        }

        private void Add<TProperty>(
            LambdaExpression rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            Delegate ruleSwitch,
            DryvRuleSettings settings,
            params Type[] services)
        {
            var switchLambda = DelegateToLambda(ruleSwitch, services, true);

            foreach (var property in properties)
            {
                this.Add(property, rule, switchLambda, settings, DryvRuleLocation.Server | DryvRuleLocation.Client);
            }
        }

        private void AddClient<TProperty>(
            LambdaExpression rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            Delegate ruleSwitch,
            DryvRuleSettings settings,
            params Type[] services)
        {
            var switchLambda = DelegateToLambda(ruleSwitch, services, true);

            foreach (var property in properties)
            {
                this.Add(property, rule, switchLambda, settings, DryvRuleLocation.Client);
            }
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

        private void AddServer<TProperty>(
            Delegate rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            Delegate ruleSwitch,
            DryvRuleSettings settings,
            params Type[] services)
        {
            var lambda = DelegateToLambda(rule, services);
            var switchLambda = DelegateToLambda(ruleSwitch, services, true);

            foreach (var property in properties)
            {
                this.Add(property, lambda, switchLambda, settings, DryvRuleLocation.Server);
            }
        }

        private void Disable<TProperty>(
            Expression<Func<TModel, TProperty>> property,
            LambdaExpression rule,
            Delegate ruleSwitch,
            DryvRuleLocation ruleLocation,
            params Type[] services)
        {
            var switchLambda = DelegateToLambda(ruleSwitch, services, true);
            var ruleDefinition = DryvCompiledRule.Create(property, rule, switchLambda, ruleLocation, null);
            ruleDefinition.RuleType = RuleType.Disabling;
            ruleDefinition.Parameters = this.Parameters;

            this.DisablingRules.Add(ruleDefinition);
        }

        private void Disable<TProperty>(
            LambdaExpression rule,
            IEnumerable<Expression<Func<TModel, TProperty>>> properties,
            Delegate ruleSwitch,
            params Type[] services)
        {
            foreach (var property in properties)
            {
                this.Disable(property, rule, ruleSwitch, DryvRuleLocation.Server | DryvRuleLocation.Client, services);
            }
        }
    }
}