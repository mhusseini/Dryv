using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Reflection;

namespace Dryv.Translation
{
    internal class EnumComparisionModifier : ExpressionVisitor
    {
        //protected override Expression VisitUnary(UnaryExpression node)
        //{
        //    if (node.NodeType != ExpressionType.Convert)
        //    {
        //        return base.VisitUnary(node);
        //    }

        //    var typeInfo = node.Operand.Type.GetTypeInfo();

        //    if (!typeInfo.IsEnum)
        //    {
        //        return base.VisitUnary(node);
        //    }

        //    return this.Visit(node.Operand) ?? throw new InvalidOperationException();
        //}

        public static bool Equals(object a, object b) => object.Equals(a, b);

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType != ExpressionType.Equal && node.NodeType != ExpressionType.NotEqual)
            {
                return base.VisitBinary(node);
            }

            var left = this.Visit(node.Left) ?? throw new InvalidOperationException();
            var right = this.Visit(node.Right) ?? throw new InvalidOperationException();

            if (left.Type != right.Type)
            {
                if (right.NodeType == ExpressionType.Constant)
                {
                    right = Expression.Convert(right, left.Type);
                }
                else if (left.NodeType == ExpressionType.Constant)
                {
                    left = Expression.Convert(left, right.Type);
                }
            }
            //left = GetInnerNode(left);
            //right = GetInnerNode(right);

            var x = ChangeIntegerToEnum(node, left, right)
                    ?? ChangeIntegerToEnum(node, right, left);
            if (x != null) return x;

            return Expression.MakeBinary(node.NodeType, left, right, node.IsLiftedToNull, node.Method);
        }

        //private static Expression GetInnerNode(Expression node)
        //{
        //    if (!(node is UnaryExpression unaryExpression) || node.NodeType != ExpressionType.Convert)
        //    {
        //        return node;
        //    }

        //    var innerType = GetTypeOrNullable(unaryExpression.Operand.Type);
        //    var outerType = GetTypeOrNullable(unaryExpression.Type);

        //    if (outerType == typeof(int) && innerType.GetTypeInfo().IsEnum)
        //    {
        //        return Expression.Convert(unaryExpression.Operand, unaryExpression.Operand.Type);
        //    }

        //    return node;
        //}

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType != ExpressionType.Convert)
            {
                return base.VisitUnary(node);
            }

            var innerType = GetTypeOrNullable(node.Operand.Type);
            var outerType = GetTypeOrNullable(node.Type);

            if (outerType != typeof(int) || !innerType.GetTypeInfo().IsEnum)
            {
                return base.VisitUnary(node);
            }

            return node.Operand;
        }

        private static Expression ChangeIntegerToEnum(BinaryExpression node, Expression left, Expression right)
        {
            if (!(right is ConstantExpression constant) ||
                GetTypeOrNullable(right.Type) != typeof(int) ||
                left.NodeType != ExpressionType.Convert ||
                !(((UnaryExpression)left).Operand is MemberExpression memberExpression))
            {
                return null;
            }

            var memberType = memberExpression.Member switch
            {
                PropertyInfo p => p.PropertyType,
                FieldInfo f => f.FieldType,
            };

            if (IsNullable(memberType))
            {
                memberType = memberType.GetGenericArguments().FirstOrDefault();
                if (memberType == null)
                {
                    return null;
                }
            }

            var typeInfo = memberType.GetTypeInfo();
            if (!typeInfo.IsEnum)
            {
                return null;
            }

            var number = constant.Value switch
            {
                int i => i,
                _ => 0
            };

            var newValue = Enum.ToObject(memberType, number);

            return Expression.MakeBinary(node.NodeType, Expression.Constant(newValue), memberExpression, node.IsLiftedToNull, node.Method);
        }

        private static Type GetTypeOrNullable(Type type)
        {
            return IsNullable(type) ? type.GetGenericArguments().First() : type;
        }

        private static bool IsNullable(Type type)
        {
            return type.GenericTypeArguments.Any() && typeof(Nullable<>) == type.GetGenericTypeDefinition();
        }
    }
}