using System.Linq.Expressions;

namespace Dryv.Translation.Visitors
{
    internal class NodeReplacer : ExpressionVisitor
    {
        private Expression node;
        private Expression replacement;

        public Expression Replace(Expression root, Expression node, Expression replacement)
        {
            this.node = node;
            this.replacement = replacement;

            return this.Visit(root);
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node == this.node ? this.replacement : node);
        }
    }
}