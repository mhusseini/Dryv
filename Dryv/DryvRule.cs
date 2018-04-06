using System;
using System.Linq.Expressions;

namespace Dryv
{
    public class DryvRule
    {
        public RuleEvaluationLocation EvaluationLocation { get; internal set; }
        public string PropertyName { get; internal set; }
        public LambdaExpression ValidationExpression { get; internal set; }
        internal Func<object[], bool> CompiledEnablingExpression { get; set; }
        internal Func<object, object[], DryvResult> CompiledValidationExpression { get; set; }
        internal LambdaExpression EnablingExpression { get; set; }
        internal string ModelName { get; set; }
        internal Type[] PreevaluationOptionTypes { get; set; }
        internal MemberExpression PropertyExpression { get; set; }
        internal Func<object[], string> TranslatedValidationExpression { get; set; }
        internal Exception TranslationError { get; set; }
    }
}