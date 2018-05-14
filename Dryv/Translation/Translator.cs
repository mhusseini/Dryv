using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dryv.Reflection;

namespace Dryv.Translation
{
    public abstract class Translator : ITranslator
    {
        private readonly TranslationCompiler translationCompiler;

        protected Translator()
        {
            this.translationCompiler = new TranslationCompiler(this);
        }

        public virtual TranslationResult Translate(Expression expression, MemberExpression propertyExpression)
        {
            var result = this.GenerateJavaScriptCode(expression, propertyExpression);
            return this.translationCompiler.GenerateTranslationDelegate(result.Code, result.OptionDelegates, result.OptionTypes);
        }

        public virtual void Translate(Expression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual string TranslateValue(object value)
        {
            return value?.ToString();
        }

        public virtual void Visit(BinaryExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(BlockExpression expression, TranslationContext context, bool negated = false)
        {
            foreach (dynamic child in expression.Expressions)
            {
                this.Translate(child, context);
            }
        }

        public virtual void Visit(ConditionalExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(ConstantExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(DebugInfoExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(DefaultExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(DynamicExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(GotoExpression expression, TranslationContext context, bool negated = false)
        {
        }

        //public virtual void Visit(IDynamicExpression expression, TranslationContext context, bool negated = false)
        //{
        //}

        public virtual void Visit(IndexExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(InvocationExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(LabelExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(LambdaExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(ListInitExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(LoopExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(MemberExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(MemberInitExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(MethodCallExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(NewArrayExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(NewExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(ParameterExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(RuntimeVariablesExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(SwitchExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(TryExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(TypeBinaryExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(UnaryExpression expression, TranslationContext context, bool negated = false)
        {
        }

        protected object GetDefaultValue(Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

        private static List<Type> GetOptionTypes(Expression expression)
        {
            var lambda = (LambdaExpression)expression;
            var genericArguments = lambda.Type.GetGenericArguments();
            return genericArguments
                .Skip(1)
                .Take(genericArguments.Count - 2)
                .ToList();
        }

        private GeneratedJavaScriptCode GenerateJavaScriptCode(
            Expression expression,
            MemberExpression propertyExpression)
        {
            // Find all option types used in the validation expression.
            var optionTypes = GetOptionTypes(expression);
            // Collect delegates that use options from withing the validation expression.
            var optionDelegates = new List<LambdaExpression>();
            var sb = new StringBuilder();

            using (var writer = new IndentingStringWriter(sb))
            {
                var context = new TranslationContext
                {
                    OptionsTypes = optionTypes,
                    Writer = writer,
                    OptionDelegates = optionDelegates,
                    ModelType = propertyExpression?.Expression.GetExpressionType(),
                    PropertyExpression = propertyExpression?.Expression
                };

                this.Translate(expression, context);
            }

            return new GeneratedJavaScriptCode
            (
                sb.ToString(),
                optionTypes,
                optionDelegates
            );
        }

        private struct GeneratedJavaScriptCode
        {
            public GeneratedJavaScriptCode(string code, List<Type> optionTypes, List<LambdaExpression> optionDelegates)
            {
                this.Code = code;
                this.OptionTypes = optionTypes;
                this.OptionDelegates = optionDelegates;
            }

            public string Code { get; }
            public List<LambdaExpression> OptionDelegates { get; }
            public List<Type> OptionTypes { get; }
        }
    }
}