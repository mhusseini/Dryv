using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dryv.Translation.Visitors
{
    internal class AsyncBinaryFinder : ExpressionVisitor<object>
    {
        private readonly Dictionary<Expression, Expression> parents = new Dictionary<Expression, Expression>();
        private readonly IReadOnlyCollection<IDryvMethodCallTranslator> methodCallTranslators;
        private readonly TranslationContext translationContext;

        public AsyncBinaryFinder(IReadOnlyCollection<IDryvMethodCallTranslator> methodCallTranslators, TranslationContext translationContext)
        {
            this.methodCallTranslators = methodCallTranslators;
            this.translationContext = translationContext;
        }

        public List<Expression> AsyncBinaryExpressions { get; } = new List<Expression>();
        public List<Expression> AsyncPath { get; } = new List<Expression>();

        public void FindAsyncBinaryExpressions(Expression node)
        {
            this.parents.Clear();
            this.AsyncBinaryExpressions.Clear();
            this.AsyncPath.Clear();

            this.Visit(node);
        }

        protected override void VisitMethodCall(Context context, MethodCallExpression node)
        {
            if (ExpressionInjectionHelper.CanInjectMethodCall(node, this.translationContext))
            {
                base.VisitMethodCall(context, node);
                return;
            }

            var objectType = node.Object?.Type ?? node.Method.DeclaringType;
            var context2 = this.translationContext.Clone<MethodTranslationContext>();

            context2.Expression = node;
            context2.WhatIfMode = true;
            context2.IsAsync = false;

            if (!this.methodCallTranslators
                    .Where(t => t.SupportsType(objectType))
                    .Any(t => t.Translate(context2)) ||
                !context2.IsAsync)
            {
                base.VisitMethodCall(context, node);
                return;
            }

            if (context.HasParent)
            {
                BinaryExpression binary;

                this.parents.Add(node, context.Parent.Expression);
                var parent = (Expression)node;

                while ((binary = parent as BinaryExpression) == null &&
                       this.parents.TryGetValue(parent, out parent))
                {
                    this.AsyncPath.Add(parent);
                }

                if (binary != null)
                {
                    this.AsyncPath.Add(binary);
                    this.AsyncBinaryExpressions.Add(binary);
                }
            }

            base.VisitMethodCall(context, node);
        }
    }
}