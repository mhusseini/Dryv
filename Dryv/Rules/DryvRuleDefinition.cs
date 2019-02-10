using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Extensions;

namespace Dryv.Rules
{
    public sealed class DryvRuleDefinition
    {
        public DryvRuleLocation EvaluationLocation { get; internal set; }
        public Type ModelType { get; internal set; }
        public PropertyInfo Property { get; internal set; }
        public LambdaExpression ValidationExpression { get; internal set; }
        internal Func<object[], bool> CompiledEnablingExpression { get; set; }
        internal Func<object, object[], object> CompiledValidationExpression { get; set; }
        internal LambdaExpression EnablingExpression { get; set; }
        internal string ModelPath { get; set; }
        internal Type[] PreevaluationOptionTypes { get; set; }
        internal MemberExpression PropertyExpression { get; set; }
        internal Func<object[], string> TranslatedValidationExpression { get; set; }
        internal Exception TranslationError { get; set; }
        public string CodeTemplate { get; internal set; }

        public static DryvRuleDefinition Create<TModel, TProperty>(Expression<Func<TModel, TProperty>> property, LambdaExpression rule, LambdaExpression enabled, DryvRuleLocation ruleLocation)
        {
            if (!(property.Body is MemberExpression memberExpression) ||
                !(memberExpression.Member is PropertyInfo propertyInfo))
            {
                return null;
            }

            var members = memberExpression
                .Iterrate(e => e.Expression as MemberExpression)
                .ToList();

            var parameter = (ParameterExpression)members.Last().Expression;

            var modelPath = string.Join(".", members
                .Skip(1)
                .Select(e => e.Member.Name.ToCamelCase())
                .Reverse());

            return new DryvRuleDefinition
            {
                PropertyExpression = memberExpression,
                Property = propertyInfo,
                ModelPath = modelPath,
                ModelType = parameter.Type,
                ValidationExpression = rule,
                EnablingExpression = enabled,
                EvaluationLocation = ruleLocation
            };
        }
    }
}