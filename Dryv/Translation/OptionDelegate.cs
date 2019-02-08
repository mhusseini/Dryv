using System.Linq.Expressions;

namespace Dryv.Translation
{
    public class OptionDelegate
    {
        public LambdaExpression LambdaExpression { get; set; }

        public int Index { get; set; }

        public bool IsRawOutput { get; set; }
    }
}