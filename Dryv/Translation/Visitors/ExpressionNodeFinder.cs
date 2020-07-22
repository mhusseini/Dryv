using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dryv.Translation.Visitors
{
    public class ExpressionNodeFinder<TExpression> : ExpressionVisitor
        where TExpression : Expression
    {
        private Stack<Expression> stack = new Stack<Expression>();

        public List<TExpression> BlackList { get; } = new List<TExpression>();

        public List<TExpression> FoundChildren { get; } = new List<TExpression>();

        public Dictionary<TExpression, ICollection<Expression>> FoundChildrenWithStack { get; } = new Dictionary<TExpression, ICollection<Expression>>();

        public static IList<TExpression> FindChildrenStatic(Expression expression)
        {
            return new ExpressionNodeFinder<TExpression>().FindChildren(expression);
        }

        public List<TExpression> FindChildren(Expression node)
        {
            this.Visit(node);
            return this.FoundChildren;
        }

        public override Expression Visit(Expression node)
        {
            this.stack.Push(node);

            if (node is TExpression expression && !this.FoundChildren.Contains(expression) && !this.BlackList.Contains(expression))
            {
                this.FoundChildren.Add(expression);
                this.FoundChildrenWithStack[expression] = this.stack.ToList();
            }

            var result = base.Visit(node);
            this.stack.Pop();

            return result;
        }
    }
}