using System;
using System.Linq.Expressions;
using System.Text;

namespace Dryv.Translation
{
    public abstract class Translator : ITranslator
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

        public virtual void Translate(Expression expression, IndentingStringWriter writer)
        {
            this.Visit(expression, writer);
        }

        public virtual void Visit(Expression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(BinaryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(BlockExpression expression, IndentingStringWriter writer, bool negated = false)
        {
            foreach (dynamic child in expression.Expressions)
            {
                this.Visit(child, writer);
            }
        }

        public virtual void Visit(ConditionalExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(ConstantExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(DebugInfoExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(DefaultExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(DynamicExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(GotoExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(IDynamicExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(IndexExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(InvocationExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(LabelExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(LambdaExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(ListInitExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(LoopExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(MemberExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(MemberInitExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(MethodCallExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(NewArrayExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(NewExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(ParameterExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(RuntimeVariablesExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(SwitchExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(TryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(TypeBinaryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void Visit(UnaryExpression expression, IndentingStringWriter writer, bool negated = false)
        {
        }

        public virtual void VisitWithBrackets(Expression expression, IndentingStringWriter writer)
        {
        }

        protected object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}