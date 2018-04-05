using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dryv.Translation
{
    public abstract class Translator : ITranslator
    {
        private static readonly MethodInfo TranslateValueMethod = typeof(Translator).GetMethod(nameof(TranslateValue));

        public virtual TranslationResult Translate(Expression expression, string modelName)
        {
            var sb = new StringBuilder();
            var optionDelegates = new List<LambdaExpression>();
            var optionTypes = GetOptionTypes(expression);

            using (var writer = new IndentingStringWriter(sb))
            {
                var context = new TranslationContext
                {
                    OptionsTypes = optionTypes,
                    Writer = writer,
                    OptionDelegates = optionDelegates,
                    ModelName = modelName
                };

                this.Translate(expression, context);
            }

            if (!optionDelegates.Any())
            {
                var lambda = Expression.Lambda<Func<object[], string>>(
                    Expression.Constant(sb.ToString()),
                    Expression.Parameter(typeof(object[])));

                return new TranslationResult
                {
                    Factory = lambda.Compile(),
                    OptionTypes = optionTypes.ToArray()
                };
            }

            var code = sb.ToString()
                .Replace("{", "{{")
                .Replace("}", "}}");
            var parameter = Expression.Parameter(typeof(object[]));
            var arrayItems = new List<Expression>();

            foreach (var lambda in optionDelegates)
            {
                var optionType = this.GetTypeChain(lambda.Body).Last();
                var idx = optionTypes.IndexOf(optionType);
                code = code.Replace($"$${lambda.GetHashCode()}$$", $"{{{idx}}}");
                var p = Expression.Convert(Expression.ArrayAccess(parameter, Expression.Constant(idx)), optionType);
                var optionValue = Expression.Convert(Expression.Invoke(lambda, p), typeof(object));
                var translatedOptionsValue = Expression.Call(Expression.Constant(this), TranslateValueMethod, optionValue);
                arrayItems.Add(Expression.Convert(translatedOptionsValue, typeof(object)));
            }

            var formatMethod = typeof(string).GetMethod(nameof(string.Format), new[] { typeof(string), typeof(object[]) });
            var pattern = Expression.Constant(code);
            var array = Expression.NewArrayInit(typeof(object), arrayItems);
            var format = Expression.Call(null, formatMethod, pattern, array);
            var result = Expression.Lambda<Func<object[], string>>(format, parameter);

            return new TranslationResult
            {
                Factory = result.Compile(),
                OptionTypes = optionTypes.ToArray()
            };
        }

        private IEnumerable<Type> GetTypeChain(Expression expression)
        {
            if (!(expression is MemberExpression memberExpression))
            {
                yield break;
            }

            while (memberExpression != null)
            {
                switch (memberExpression.Expression)
                {
                    case MemberExpression mex:
                        memberExpression = mex;
                        yield return mex.Type;
                        break;
                    case ParameterExpression pex:
                        memberExpression = null;
                        yield return pex.Type;
                        break;
                }
            }
        }

        public virtual void Translate(Expression expression, TranslationContext context)
        {
            this.Visit(expression, context);
        }

        public virtual object TranslateValue(object value)
        {
            return value;
        }

        public virtual void Visit(Expression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(BinaryExpression expression, TranslationContext context, bool negated = false)
        {
        }

        public virtual void Visit(BlockExpression expression, TranslationContext context, bool negated = false)
        {
            foreach (dynamic child in expression.Expressions)
            {
                this.Visit(child, context);
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

        public virtual void Visit(IDynamicExpression expression, TranslationContext context, bool negated = false)
        {
        }

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

        public virtual void VisitWithBrackets(Expression expression, TranslationContext context)
        {
        }

        protected object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        private static List<Type> GetOptionTypes(Expression expression)
        {
            var lambda = (LambdaExpression)expression;
            var args = lambda.Type.GetGenericArguments().Skip(1).ToList();
            args = args.Take(args.Count - 1).ToList();
            return args;
        }
    }
}