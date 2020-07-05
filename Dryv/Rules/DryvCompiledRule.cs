using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Extensions;

namespace Dryv.Rules
{
    public sealed class DryvCompiledRule
    {
        public bool IsDisablingRule { get; internal set; }
        public string CodeTemplate { get; internal set; }
        public Func<object[], bool> CompiledEnablingExpression { get; internal set; }
        public Func<object, object[], object> CompiledValidationExpression { get; internal set; }
        public DryvRuleLocation EvaluationLocation { get; internal set; }
        public string GroupName { get; set; }
        public string ModelPath { get; internal set; }
        public Type ModelType { get; internal set; }
        public Type[] PreevaluationOptionTypes { get; internal set; }
        public PropertyInfo Property { get; internal set; }
        public Func<Func<Type, object>, object[], string> TranslatedValidationExpression { get; internal set; }
        public Exception TranslationError { get; internal set; }
        internal LambdaExpression EnablingExpression { get; set; }
        internal MemberExpression PropertyExpression { get; set; }
        internal LambdaExpression ValidationExpression { get; set; }

        public static DryvCompiledRule Create<TModel, TProperty>(Expression<Func<TModel, TProperty>> property, LambdaExpression rule, LambdaExpression enabled, DryvRuleLocation ruleLocation, string groupName)
        {
            var body = property.Body is UnaryExpression unaryExpression
                ? unaryExpression.Operand
                : property.Body;

            if (!(body is MemberExpression memberExpression) ||
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

            return new DryvCompiledRule
            {
                EnablingExpression = enabled,
                EvaluationLocation = ruleLocation,
                GroupName = groupName,
                ModelPath = modelPath,
                ModelType = parameter.Type,
                Property = propertyInfo,
                PropertyExpression = memberExpression,
                ValidationExpression = rule,
            };
        }
    }
}