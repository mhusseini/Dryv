using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dryv.Translation.Visitors
{
    internal class AsyncBinarySwitcher : ExpressionVisitor
    {
        private List<Expression> asyncExpressions;

        public static Expression Modify(Expression expression, List<Expression> asyncExpressions)
        {
            var switcher = new AsyncBinarySwitcher { asyncExpressions = asyncExpressions };

            return switcher.Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var expression = node.NodeType != ExpressionType.AndAlso && node.NodeType != ExpressionType.OrElse
                ? node
                : this.ConvertNode(node);

            return base.VisitBinary(expression);
        }

        private BinaryExpression ConvertNode(BinaryExpression node)
        {
            var isLeftAsync = this.asyncExpressions.Contains(node.Left);
            var isRightAsync = this.asyncExpressions.Contains(node.Right);
            var switchOperands = isLeftAsync && !isRightAsync;

            return switchOperands
                ? Expression.MakeBinary(node.NodeType, node.Right, node.Left, node.IsLiftedToNull, node.Method)
                : node;
        }
    }
}