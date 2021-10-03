using System.Linq.Expressions;

namespace Dryv.Translation.Visitors
{
    internal class ConditionConversionModifier : ExpressionVisitor
    {
        protected override Expression VisitUnary(UnaryExpression conversion)
        {
            return conversion.NodeType == ExpressionType.Convert && conversion.Operand is ConditionalExpression conditional
                ? base.VisitConditional(Expression.Condition(conditional.Test,
                    Expression.Convert(conditional.IfTrue, conversion.Type),
                    Expression.Convert(conditional.IfFalse, conversion.Type)))
                : base.VisitUnary(conversion);
        }
    }
}