using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dryv.Configuration;
using Dryv.Rules;
using Dryv.Translation.Visitors;

namespace Dryv.Translation
{
    public abstract class Translator : ITranslator
    {
        private readonly TranslationCompiler translationCompiler;

        protected Translator(DryvOptions options)
        {
            this.Options = options;
            this.translationCompiler = new TranslationCompiler(this);
        }

        protected DryvOptions Options { get; }

        public virtual TranslationResult Translate(Expression expression, MemberExpression propertyExpression, DryvCompiledRule rule)
        {
            var result = this.GenerateJavaScriptCode(expression, propertyExpression, rule);
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

        private GeneratedJavaScriptCode GenerateJavaScriptCode(
            Expression expression,
            MemberExpression propertyExpression,
            DryvCompiledRule rule)
        {
            // Find all option types used in the validation expression.
            var optionTypes = ((LambdaExpression)expression).GetOptionTypes();
            // Collect delegates that use options from withing the validation expression.
            var optionDelegates = new Dictionary<int, OptionDelegate>();
            var sb = new StringBuilder();

            using var writer = new IndentingStringWriter(sb);
            var context = new TranslationContext
            {
                OptionsTypes = optionTypes,
                Writer = writer,
                OptionDelegates = optionDelegates,
                ModelType = propertyExpression?.Expression.GetExpressionType(),
                PropertyExpression = propertyExpression?.Expression,
                GroupName = rule.GroupName,
                StringBuilder = sb,
                Rule = rule,
            };

            expression = new EnumComparisionModifier().Visit(expression);
            expression = new ConditionConversionModifier().Visit(expression);
            this.Translate(expression, context);

            return new GeneratedJavaScriptCode
            (
                sb.ToString(),
                optionTypes,
                context.OptionDelegates.Values.ToList()
            );
        }

        private struct GeneratedJavaScriptCode
        {
            public GeneratedJavaScriptCode(string code, IList<Type> optionTypes, IList<OptionDelegate> optionDelegates)
            {
                this.Code = code;
                this.OptionTypes = optionTypes;
                this.OptionDelegates = optionDelegates;
            }

            public string Code { get; }
            public IList<OptionDelegate> OptionDelegates { get; }
            public IList<Type> OptionTypes { get; }
        }
    }
}