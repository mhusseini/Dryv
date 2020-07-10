using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dryv.Translation.Visitors
{
    public class ExpressionNodeFinder<TExpression> : ExpressionVisitor
        where TExpression : Expression
    {
        public List<TExpression> FoundChildren { get; } = new List<TExpression>();

        public List<TExpression> BlackList { get; } = new List<TExpression>();

        public List<TExpression> FindChildren(Expression node)
        {
            this.Visit(node);
            return this.FoundChildren;
        }

        public override Expression Visit(Expression node)
        {
            if (node is TExpression expression && !this.FoundChildren.Contains(expression) && !this.BlackList.Contains(expression))
            {
                this.FoundChildren.Add(expression);
            }

            return base.Visit(node);
        }
    }
}