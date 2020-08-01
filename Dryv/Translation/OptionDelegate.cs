using System.Linq.Expressions;

namespace Dryv.Translation
{
    public class InjectedExpression
    {
        public LambdaExpression LambdaExpression { get; set; }

        public int Index { get; set; }

        public bool IsRawOutput { get; set; }
    }
}