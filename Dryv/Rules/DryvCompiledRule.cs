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
        public IDictionary<string, object> Annotations { get; internal set; } = new Dictionary<string, object>();
        public string CodeTemplate { get; internal set; }
        public Func<object[], bool> CompiledEnablingExpression { get; internal set; }
        public Func<object, object[], object> CompiledValidationExpression { get; internal set; }
        public LambdaExpression EnablingExpression { get; internal set; }
        public DryvEvaluationLocation EvaluationLocation { get; internal set; }
        public string Group { get; internal set; }
        public bool IsAsync { get; internal set; }
        public string ModelPath { get; internal set; }
        public Type ModelType { get; internal set; }
        public string Name { get; internal set; }
        public Type[] PreevaluationOptionTypes { get; internal set; }
        public PropertyInfo Property { get; internal set; }
        public MemberExpression PropertyExpression { get; internal set; }
        public RuleType RuleType { get; internal set; } = RuleType.Validation;
        public Func<Func<Type, object>, object[], string> TranslatedValidationExpression { get; internal set; }
        public Exception TranslationError { get; internal set; }
        public LambdaExpression ValidationExpression { get; internal set; }
        internal IReadOnlyList<DryvCompiledRule> Parameters { get; set; }
        internal string UniquePath { get; set; }
        internal IDictionary<PropertyInfo, string> RelatedProperties { get; set; }

        internal static DryvCompiledRule Create<TModel, TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, LambdaExpression validationExpression, LambdaExpression enablingExpression, DryvEvaluationLocation ruleLocation, string group)
        {
            var memberExpression = propertyExpression.GetMemberExpression();
            if (memberExpression == null)
            {
                return null;
            }

            var modelPath = memberExpression.GetModelPath(true, out var members);
            var propertyPath = string.Join(".", members
                .Skip(1)
                .Select(e => e.Member.Name + ":" + GetMemberType(e).FullName)
                .Reverse());

            var parameter = (ParameterExpression)members.Last().Expression;
            var uniquePath = ":" + parameter.Type.FullName;
            if (!string.IsNullOrWhiteSpace(propertyPath))
            {
                uniquePath += "." + propertyPath;
            }

            return new DryvCompiledRule
            {
                EnablingExpression = enablingExpression,
                EvaluationLocation = ruleLocation,
                Group = group,
                UniquePath = uniquePath,
                ModelPath = modelPath,
                ModelType = parameter.Type,
                Property = (PropertyInfo)memberExpression.Member,
                PropertyExpression = memberExpression,
                ValidationExpression = validationExpression,
            };
        }

        internal static DryvCompiledRule CreateParameter(string name, Expression<Func<object, object[], object>> lambda, Type[] services)
        {
            return new DryvCompiledRule
            {
                RuleType = RuleType.Parameter,
                Name = name,
                PreevaluationOptionTypes = services,
                CompiledValidationExpression = lambda.Compile()
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