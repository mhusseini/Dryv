using System.Linq.Expressions;

namespace Dryv.Translation.Visitors
{
    public class AsyncMethodCallFinder : ExpressionVisitor
    {
        private readonly TranslationContext context;
        private int count;

        public AsyncMethodCallFinder(TranslationContext context)
        {
            this.context = context;
        }

        public static bool ContainsAsyncCalls(TranslationContext context, Expression expression)
        {
            return new AsyncMethodCallFinder(context).IsAsync(expression);
        }

        public static int GetAsyncCallCount(TranslationContext context, Expression expression)
        {
            return new AsyncMethodCallFinder(context).GetAsyncCallCount(expression);
        }

        public int GetAsyncCallCount(Expression expression)
        {
            this.Visit(expression);
            return this.count;
        }

        public bool IsAsync(Expression expression)
        {
            this.Visit(expression);
            return this.count > 0;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            this.Visit(node.Left);
            this.Visit(node.Right);
            return node;
            // return base.VisitBinary(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var ctx = this.context.Clone<TranslationContext>();
            ctx.WhatIfMode = true;
            ctx.IsAsync = false;

            this.context.Translator.Translate(node, ctx);

            if (ctx.IsAsync)
            {
                this.count++;
            }

            ctx.IsAsync = false;

            return base.VisitMethodCall(node);

            return node;
        }
    }
}