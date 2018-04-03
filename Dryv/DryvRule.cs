using System;
using System.Linq.Expressions;

namespace Dryv
{
    public class DryvRule
    {
        internal Func<object[], bool> CompiledEnablingExpression { get; set; }
        internal Func<object, object[], DryvResult> CompiledValidationExpression { get; set; }
        internal LambdaExpression EnablingExpression { get; set; }
        internal Type[] PreevaluationOptionTypes { get; set; }
        public RuleEvaluationLocation EvaluationLocation { get; set; }
        internal Func<object[], string> TranslatedValidationExpression { get; set; }
        internal Exception TranslationError { get; set; }
        public LambdaExpression ValidationExpression { get; set; }
    }
}