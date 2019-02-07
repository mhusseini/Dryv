using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Dryv
{
    public sealed class DryvRuleDefinition
    {
        public RuleEvaluationLocation EvaluationLocation { get; internal set; }
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
    }
}