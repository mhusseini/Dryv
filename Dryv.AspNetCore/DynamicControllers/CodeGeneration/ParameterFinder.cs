using System.Collections.Generic;
using System.Linq.Expressions;
using Dryv.Translation.Visitors;

namespace Dryv.AspNetCore.DynamicControllers.CodeGeneration
{
    internal class ParameterFinder : ExpressionNodeFinder<ParameterExpression>
    {
        private Expression entryNode;

        public List<ParameterExpression> Find(Expression node)
        {
            this.entryNode = node;
            base.Visit(node);

            return this.FoundChildren;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (this.entryNode != node)
            {
                this.BlackList.AddRange(node.Parameters);
            }

            return base.VisitLambda<T>(node);
        }
    }
}