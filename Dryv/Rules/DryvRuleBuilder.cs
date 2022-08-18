using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Extensions;

namespace Dryv.Rules
{
    public class DryvRuleBuilder
    {
        protected DryvRuleBuilder(DryvEvaluationLocation evaluationLocation, Type modeltype)
        {
            this.Rule = new DryvRule
            {
                EvaluationLocation = evaluationLocation,
                ModelType = modeltype
            };
        }

        public DryvRule Rule { get; }
    }


    public class DryvRuleBuilder<TModel, TValidationFunction, TAsyncValidationFunction, TSwitchFunction> : DryvRuleBuilder
    {
        public DryvRuleBuilder(DryvEvaluationLocation evaluationLocation) : base(evaluationLocation, typeof(TModel))
        {
        }

        public DryvRuleBuilder<TModel, TValidationFunction, TAsyncValidationFunction, TSwitchFunction> Name(string name)
        {
            this.Rule.Name = name;
            return this;
        }

        public DryvRuleBuilder<TModel, TValidationFunction, TAsyncValidationFunction, TSwitchFunction> Group(string @group)
        {
            this.Rule.Group = @group;
            return this;
        }

        public DryvRuleBuilder<TModel, TValidationFunction, TAsyncValidationFunction, TSwitchFunction> Annotation(string key, object value)
        {
            this.Rule.Annotations.Add(key, value);
            return this;
        }

        public DryvRuleBuilder<TModel, TValidationFunction, TAsyncValidationFunction, TSwitchFunction> Property(Expression<Func<TModel, object>> property)
        {
            if (property.Body is not MemberExpression memberExpression ||
                memberExpression.Member is not PropertyInfo propertyInfo)
            {
                throw new ArgumentException("Property access expected.", nameof(property));
            }

            if (this.Rule.Properties.Any(p => p.Property.Name == propertyInfo.Name
                                              && p.Property.DeclaringType == propertyInfo.DeclaringType))
            {
                return this;
            }

            var experssionChain = memberExpression.Iterate(e => e.Expression as MemberExpression).Reverse().ToList();
            var methodAccess = experssionChain.FirstOrDefault(e => e.Member is MethodInfo);
            if (methodAccess != null)
            {
                throw new ArgumentException($"Method call not supported: {methodAccess}.", nameof(property));
            }

            var path = string.Join(".", experssionChain.Select(e => e.Member.Name));
            var propertyHierrarchy = experssionChain.Select(e => e.Member).ToList();

            this.Rule.Properties.Add(new DryvRuleProperty
            {
                Property = propertyInfo,
                PropertyPath = path,
                PropertyHierarchy = propertyHierrarchy,
                IsGlobal = propertyInfo.DeclaringType == typeof(TModel)
            });

            return this;
        }

        public DryvRuleBuilder<TModel, TValidationFunction, TAsyncValidationFunction, TSwitchFunction> Switch(TSwitchFunction function)
        {
            return this;
        }

        internal DryvRuleBuilder<TModel, TValidationFunction, TAsyncValidationFunction, TSwitchFunction> Validate(TValidationFunction function)
        {
            this.Rule.RuleType = DryvRuleType.Validation;

            switch (function)
            {
                case Delegate @delegate:
                    this.Rule.ValidationFunction = @delegate;
                    break;

                case Expression expression:
                    this.Rule.ValidationFunctionExpression = expression;
                    break;
            }

            return this;
        }

        internal DryvRuleBuilder<TModel, TValidationFunction, TAsyncValidationFunction, TSwitchFunction> ValidateAsync(TAsyncValidationFunction function)
        {
            this.Rule.RuleType = DryvRuleType.Validation;

            switch (function)
            {
                case Delegate @delegate:
                    this.Rule.AsyncValidationFunction = @delegate;
                    break;

                case Expression expression:
                    this.Rule.AsyncValidationFunctionExression = expression;
                    break;
            }

            return this;
        }

        internal DryvRuleBuilder<TModel, TValidationFunction, TAsyncValidationFunction, TSwitchFunction> Disable(TValidationFunction function)
        {
            this.Rule.RuleType = DryvRuleType.Disabling;

            switch (function)
            {
                case Delegate @delegate:
                    this.Rule.ValidationFunction = @delegate;
                    break;

                case Expression expression:
                    this.Rule.ValidationFunctionExpression = expression;
                    break;
            }

            return this;
        }

        internal DryvRuleBuilder<TModel, TValidationFunction, TAsyncValidationFunction, TSwitchFunction> DisableAsync(TAsyncValidationFunction function)
        {
            this.Rule.RuleType = DryvRuleType.Disabling;

            switch (function)
            {
                case Delegate @delegate:
                    this.Rule.AsyncValidationFunction = @delegate;
                    break;

                case Expression expression:
                    this.Rule.AsyncValidationFunctionExression = expression;
                    break;
            }

            return this;
        }
    }
}