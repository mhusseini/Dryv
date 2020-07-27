using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Extensions;

namespace Dryv.Rules
{
    [DebuggerDisplay("{" + nameof(ValidationExpression) + "}")]
    public sealed class DryvCompiledRule
    {
        public string CodeTemplate { get; internal set; }
        public Func<object[], bool> CompiledEnablingExpression { get; internal set; }
        public Func<object, object[], object> CompiledValidationExpression { get; internal set; }
        public DryvRuleLocation EvaluationLocation { get; internal set; }
        public string GroupName { get; internal set; }
        public bool IsAsync { get; internal set; }
        public RuleType RuleType { get; internal set; } = RuleType.Validation;
        public string ModelPath { get; internal set; }
        public Type ModelType { get; internal set; }
        public string Name { get; internal set; }
        public Type[] PreevaluationOptionTypes { get; internal set; }
        public PropertyInfo Property { get; internal set; }
        public Func<Func<Type, object>, object[], string> TranslatedValidationExpression { get; internal set; }
        public Exception TranslationError { get; internal set; }
        internal LambdaExpression EnablingExpression { get; set; }
        internal MemberExpression PropertyExpression { get; set; }
        internal string UniquePath { get; set; }
        internal LambdaExpression ValidationExpression { get; set; }
        internal List<DryvCompiledRule> Parameters { get; set; }

        public static DryvCompiledRule CreateParameter(string name, Expression<Func<object, object[], object>> lambda, Type[] services)
        {
            return new DryvCompiledRule
            {
                RuleType = RuleType.Parameter,
                Name = name,
                PreevaluationOptionTypes = services,
                CompiledValidationExpression = lambda.Compile()
            };
        }
        public static DryvCompiledRule Create<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, LambdaExpression validationExpression, LambdaExpression enablingExpression, DryvRuleLocation ruleLocation, string groupName)
        {
            var body = propertyExpression.Body is UnaryExpression unaryExpression
                ? unaryExpression.Operand
                : propertyExpression.Body;

            if (!(body is MemberExpression memberExpression) ||
                !(memberExpression.Member is PropertyInfo propertyInfo))
            {
                return null;
            }

            var members = memberExpression.Iterate(e => e.Expression as MemberExpression)
                .ToList();

            var parameter = (ParameterExpression)members.Last().Expression;

            var modelPath = string.Join(".", members
                .Skip(1)
                .Select(e => e.Member.Name.ToCamelCase())
                .Reverse());

            var propertyPath = string.Join(".", members
                .Skip(1)
                .Select(e => e.Member.Name + ":" + GetMemberType(e).FullName)
                .Reverse());

            var uniquePath = ":" + parameter.Type.FullName;
            if (!string.IsNullOrWhiteSpace(propertyPath))
            {
                uniquePath += "." + propertyPath;
            }

            return new DryvCompiledRule
            {
                EnablingExpression = enablingExpression,
                EvaluationLocation = ruleLocation,
                GroupName = groupName,
                UniquePath = uniquePath,
                ModelPath = modelPath,
                ModelType = parameter.Type,
                Property = propertyInfo,
                PropertyExpression = memberExpression,
                ValidationExpression = validationExpression,
            };
        }

        private static Type GetMemberType(MemberExpression e)
        {
            return e.Member switch
            {
                PropertyInfo p => p.PropertyType,
                FieldInfo f => f.FieldType,
                _ => throw new NotSupportedException($"Member expressions returning {e.Member.GetType().FullName} are not supported.")
            };
        }
    }
}