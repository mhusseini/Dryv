using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dryv.Translation
{
    public class ExpressionNodeFinder<TExpression> : ExpressionVisitor
        where TExpression : Expression
    {
        public List<TExpression> FoundChildren { get; } = new List<TExpression>();

        public override Expression Visit(Expression node)
        {
            if (node is TExpression expression && !this.FoundChildren.Contains(expression))
            {
                this.FoundChildren.Add(expression);
            }

            return base.Visit(node);
        }
    }
}