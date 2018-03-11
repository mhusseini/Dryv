using System;
using System.Linq.Expressions;
using System.Text;

namespace Dryv.Translation
{
    internal abstract class Translator
    {
        public virtual string Translate(Expression expression)
        {
            var sb = new StringBuilder();

            using (var writer = new IndentingStringWriter(sb))
            {
                this.Translate(expression, writer);
            }

            return sb.ToString();
        }

        public virtual void Translate(Expression expression, IndentingStringWriter writer, bool negated = false)
        {
            this.Visit((dynamic)expression, writer);
        }

        public virtual object Visit(BinaryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(BlockExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            foreach (dynamic child in expression.Expressions)
            {
                this.Visit(child, writer);
            }
            return null;
        }

        public virtual object Visit(ConditionalExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(ConstantExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(DebugInfoExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(DefaultExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(DynamicExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(GotoExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(IDynamicExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(IndexExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(InvocationExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(LabelExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(LambdaExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(ListInitExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(LoopExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(MemberExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(MemberInitExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(MethodCallExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(NewArrayExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(NewExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(ParameterExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(RuntimeVariablesExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(SwitchExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(TryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(TypeBinaryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        public virtual object Visit(UnaryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            return null;
        }

        protected object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}