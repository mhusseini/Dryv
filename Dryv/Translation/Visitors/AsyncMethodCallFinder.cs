using System.Linq.Expressions;

namespace Dryv.Translation.Visitors
{
    public class AsyncMethodCallFinder : ExpressionVisitor
    {
        private readonly TranslationContext context;
        private bool isAsync;

        public AsyncMethodCallFinder(TranslationContext context)
        {
            this.context = context;
        }

        public static bool ContainsAsyncCalls(TranslationContext context, Expression expression)
        {
            return new AsyncMethodCallFinder(context).IsAsync(expression);
        }

        public bool IsAsync(Expression expression)
        {
            this.isAsync = false;
            this.Visit(expression);
            return this.isAsync;
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

            if (!ctx.IsAsync)
            {
                return base.VisitMethodCall(node);
            }

            this.isAsync = true;

            return node;
        }
    }
}