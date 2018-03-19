using System;
using System.Linq.Expressions;

namespace Dryv
{
    internal class DryvRule
    {
        public Func<object[], bool> CompiledEnablingExpression { get; set; }
        public Func<object, object[], DryvResult> CompiledValidationExpression { get; set; }
        public LambdaExpression EnablingExpression { get; set; }
        public Type[] PreevaluationOptionTypes { get; set; }
        public Func<object[], string> TranslatedValidationExpression { get; set; }
        public LambdaExpression ValidationExpression { get; set; }
        public Exception TranslationError { get; set; }
    }
}