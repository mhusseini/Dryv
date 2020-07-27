using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dryv.Reflection;
using Dryv.Translation.Visitors;

namespace Dryv.Translation
{
    public class MethodCallExpressionHelper
    {
        public static bool CanInjectMethodCall(MethodCallExpression expression, TranslationContext context)
        {
            return CanInjectMethodCall(expression, context, out _);
        }

        public static bool CanInjectMethodCall(MethodCallExpression expression, TranslationContext context, out IList<ParameterExpression> parameters)
        {
            parameters = null;

            if (!expression.Type.IsSystemType())
            {
                return false;
            }

            parameters = ExpressionNodeFinder<ParameterExpression>.FindChildrenStatic(expression.Object);
            if (parameters.Any(p => !context.OptionsTypes.Contains(p.Type)))
            {
                return false;
            }

            if (!parameters.Any() && !expression.Method.IsStatic)
            {
                return false;
            }

            var finder = new ExpressionNodeFinder<ParameterExpression>();
            var parameterExpressions = (from a in expression.Arguments
                from p in finder.FindChildren(a)
                where p != null
                select p).ToList();

            if (parameterExpressions.Any(p => p.Type == context.ModelType))
            {
                return false;
            }

            if (!parameters.Any() && parameterExpressions.Any())
            {
                parameters = parameterExpressions.Where(p => context.OptionsTypes.Contains(p.Type)).ToList();
            }

            return parameters.Any();
        }
    }
}