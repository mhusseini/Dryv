using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dryv.Rules
{
    public class DryvRuleBuilder
    {
        public DryvRuleBuilder(DryvEvaluationLocation evaluationLocation)
        {
            this.Rule = new DryvRule
            {
                EvaluationLocation = evaluationLocation
            };
        }

        public DryvRule Rule { get; }
    }


    public class DryvRuleBuilder<TModel, TValidationFunction, TAsyncValidationFunction, TSwitchFunction> : DryvRuleBuilder
    {
        public DryvRuleBuilder(DryvEvaluationLocation evaluationLocation) : base(evaluationLocation)
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
            if (property.Body is not MemberExpression memberExpression)
            {
                throw new ArgumentException("Member access expected.", nameof(property));
            }

            var m = memberExpression.Member;

            if (this.Rule.Properties.Any(p => p.Member.Name == m.Name && p.Member.DeclaringType == m.DeclaringType))
            {
                return this;
            }
            
            this.Rule.Properties.Add(memberExpression);
            
            return this;
        }

        public DryvRuleBuilder<TModel, TValidationFunction, TAsyncValidationFunction, TSwitchFunction> Switch(TSwitchFunction function)
        {
            return this;
        }

        internal DryvRuleBuilder<TModel, TValidationFunction, TAsyncValidationFunction, TSwitchFunction> Validate(TValidationFunction function)
        {
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