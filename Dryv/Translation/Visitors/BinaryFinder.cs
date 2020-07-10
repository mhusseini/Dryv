using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dryv.Translation.Visitors
{
    internal class BinaryFinder : ExpressionVisitor
    {
        private readonly AsyncMethodCallFinder asyncFinder;

        public BinaryFinder(JavaScriptTranslator translator, TranslationContext context)
        {
            this.asyncFinder = new AsyncMethodCallFinder(translator, context);
        }

        public List<BinaryExpression> Binaries { get; } = new List<BinaryExpression>();
        public HashSet<Expression> AsyncExpressions { get; } = new HashSet<Expression>();

        public Expression Modify(Expression expression)
        {
            this.Binaries.Clear();
            this.AsyncExpressions.Clear();

            return this.Visit(expression);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    var expression = this.ConvertNode(node);
                    return base.VisitBinary(expression);
            }

            return base.VisitBinary(node);
        }

        private BinaryExpression ConvertNode(BinaryExpression node)
        {
            var isLeftAsync = this.asyncFinder.IsAsync(node.Left);
            var isRightAsync = this.asyncFinder.IsAsync(node.Right);
            var switchOperands = isLeftAsync && !isRightAsync;

            if (isLeftAsync) this.AsyncExpressions.Add(node.Left);
            if (isRightAsync) this.AsyncExpressions.Add(node.Right);

            this.Binaries.Add(node);

            return switchOperands
                ? Expression.MakeBinary(node.NodeType, node.Right, node.Left, node.IsLiftedToNull, node.Method)
                : node;
        }
    }
}