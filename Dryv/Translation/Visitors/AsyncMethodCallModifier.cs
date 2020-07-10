﻿using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dryv.Translation.Visitors
{
    internal class AsyncMethodCallModifier : ExpressionVisitor
    {
        private readonly TranslationContext translationContext;
        private readonly JavaScriptTranslator translator;
        private bool disabled;

        public AsyncMethodCallModifier(JavaScriptTranslator translator, TranslationContext translationContext)
        {
            this.translator = translator;
            this.translationContext = translationContext;
        }

        public Dictionary<StringBuilder, ParameterExpression> AsyncCalls { get; set; } = new Dictionary<StringBuilder, ParameterExpression>();

        public Expression ApplyPromises(Expression expression)
        {
            return this.Visit(expression);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            this.disabled = true;
            return base.VisitConditional(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (this.disabled)
            {
                return expression;
            }

            var sb = new StringBuilder();
            var context = this.translationContext.Clone<TranslationContext>(sb);
            context.IsAsync = false;

            this.translator.Translate(expression, context);

            if (!context.IsAsync)
            {
                return expression;
            }

            var parameter = Expression.Parameter(expression.Type, context.GetVirtualParameter());
            this.AsyncCalls.Add(sb, parameter);

            return parameter;
        }
    }
}