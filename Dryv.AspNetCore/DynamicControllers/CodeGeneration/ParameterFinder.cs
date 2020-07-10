using System.Linq.Expressions;
using Dryv.Translation;
using Dryv.Translation.Visitors;

namespace Dryv.AspNetCore.DynamicControllers.CodeGeneration
{
    internal class ParameterFinder : ExpressionNodeFinder<ParameterExpression>
    {
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            this.BlackList.AddRange(node.Parameters);

            return base.VisitLambda<T>(node);
        }
    }
}