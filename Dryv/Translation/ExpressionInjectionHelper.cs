using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dryv.Extensions;
using Dryv.Reflection;
using Dryv.Translation.Visitors;

namespace Dryv.Translation
{
    public class ExpressionInjectionHelper
    {
        public static bool CanInjectMethodCall(MethodCallExpression expression, TranslationContext context)
        {
            var parameters = ExpressionNodeFinder<ParameterExpression>.FindChildrenStatic(expression.Object);
            return CanInjectMethodCall(expression, context, parameters);
        }

        public static bool CanInjectProperty(Expression expression, TranslationContext context, IList<ParameterExpression> parameters)
        {
            if (!expression.Type.IsSystemType())
            {
                return false;
            }

            if (parameters.Count == 0)
            {
                return expression.IsStaticMemberAccess();
            }

            var expressionParams = ExpressionNodeFinder<ParameterExpression>.FindChildrenStatic(expression);
            if (expressionParams.Any(p => p.Type == context.ModelType))
            {
                return false;
            }

            var parameter = GetInjectionParameters(expression, context)?.FirstOrDefault();

            return parameter != null && context.InjectedServiceTypes.Contains(parameter.Type)
                   || expression.IsStaticMemberAccess();

        }

        public static bool CanInjectMethodCall(MethodCallExpression expression, TranslationContext context, IList<ParameterExpression> parameters)
        {
            if (!expression.Type.IsSystemType())
            {
                return false;
            }

            if (parameters.Any(p => !context.InjectedServiceTypes.Contains(p.Type)))
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

            var notInlinable = from p in parameterExpressions
                               where p.Type == context.ModelType || !context.InjectedServiceTypes.Contains(p.Type)
                               select p;

            return !notInlinable.Any();
        }

        public static IList<ParameterExpression> GetInjectionParameters(Expression expression, TranslationContext context)
        {
            if (expression.Type == typeof(DryvValidationResult))
            {
                return null;
            }

            var memberExpression = expression as MemberExpression;
            while (memberExpression?.Expression != null)
            {
                // TODO: can we delete nect block?
                if (typeof(Task).IsAssignableFrom(memberExpression.Expression.Type))
                {
                    return null;
                }

                memberExpression = memberExpression.Expression as MemberExpression;
            }

            var visitor = new ExpressionNodeFinder<ParameterExpression>();
            visitor.Visit(expression);

            var visitor2 = new ExpressionNodeFinder<MethodCallExpression>();
            visitor2.Visit(expression);

            if (!visitor.FoundChildren.Any() && !visitor2.FoundChildren.Any())
            {
                return null;
            }

            // Parameters that start with '$' are generated dummy parameters and should not be injected.
            if (visitor.FoundChildren.Any(c => c.Name.StartsWith("$")))
            {
                return null;
            }

            return visitor.FoundChildrenWithStack.Any(x => x.Value.Contains(expression) && x.Value.Any(o => o.Type == context.ModelType))
                ? null
                : visitor.FoundChildren;
        }
    }
}