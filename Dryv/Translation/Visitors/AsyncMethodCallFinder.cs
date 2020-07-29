using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dryv.Translation.Visitors
{
    public class AsyncMethodCallFinder : ExpressionVisitor<object>
    {
        private readonly IReadOnlyCollection<IDryvMethodCallTranslator> methodCallTranslators;
        private readonly TranslationContext translationContext;

        public AsyncMethodCallFinder(IReadOnlyCollection<IDryvMethodCallTranslator> methodCallTranslators, TranslationContext translationContext)
        {
            this.methodCallTranslators = methodCallTranslators;
            this.translationContext = translationContext;
        }

        public List<MethodCallExpression> AsyncMethodCallsExpressions { get; } = new List<MethodCallExpression>();

        public void FindAsyncMethodCalls(Expression node)
        {
            this.AsyncMethodCallsExpressions.Clear();
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

            if (this.methodCallTranslators
                    .Where(t => t.SupportsType(objectType))
                    .Any(t => t.Translate(context2)) &&
                context2.IsAsync)
            {
                this.AsyncMethodCallsExpressions.Add(node);
            }

            base.VisitMethodCall(context, node);
        }
    }
}