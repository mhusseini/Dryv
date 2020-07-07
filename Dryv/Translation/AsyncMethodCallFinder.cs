using System.Linq.Expressions;

namespace Dryv.Translation
{
    internal class AsyncMethodCallFinder : ExpressionVisitor
    {
        private readonly JavaScriptTranslator translator;
        private readonly TranslationContext context;
        private bool isAsync;

        public bool IsAsync(Expression expression)
        {
            this.isAsync = false;
            this.Visit(expression);
            return this.isAsync;
        }

        public AsyncMethodCallFinder(JavaScriptTranslator translator, TranslationContext context)
        {
            this.translator = translator;
            this.context = context;
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

            this.translator.Translate(node, ctx);

            if (!ctx.IsAsync)
            {
                return base.VisitMethodCall(node);
            }

            this.isAsync = true;

            return node;
        }
    }
}