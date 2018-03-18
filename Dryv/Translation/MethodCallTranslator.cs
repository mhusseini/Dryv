using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Dryv.Translation;

namespace Dryv.MethodCallTranslation
{
    public abstract class MethodCallTranslator : IMethodCallTranslator
    {
        private readonly List<(Regex Method, Action<MethodTranslationContext> Translator)> methodTranslatorsByRegex = new List<(Regex Method, Action<MethodTranslationContext> Translator)>();

        private readonly List<Type> supportedTypes = new List<Type>();

        public static string QuoteValue(object value)
            => value == null
            ? "null"
            : (value.GetType().IsPrimitive
                ? value.ToString()
                : $@"""{value}""");

        public static void WriteArguments(Translator translator, IEnumerable<Expression> arguments, TranslationContext context)
        {
            var sep = string.Empty;
            foreach (var argument in arguments)
            {
                context.Writer.Write(sep);
                translator.Visit(argument, context);
                sep = ", ";
            }
        }

        public virtual bool SupportsType(Type type) => this.supportedTypes.Contains(type);

        public virtual bool Translate(MethodTranslationContext options)
        {
            var translator = this.methodTranslatorsByRegex.Where(i => i.Method.IsMatch(options.Expression.Method.Name)).Select(i => i.Translator).FirstOrDefault();

            if (translator == null)
            {
                return false;
            }

            translator(options);
            return true;
        }

        protected static bool ArgumentIs<T>(MethodTranslationContext options, int index, T value)
            => Equals(options.Expression.Arguments
            .Skip(index)
            .Take(1)
            .OfType<ConstantExpression>()
            .Select(i => i.Value)
            .FirstOrDefault(), value);

        protected static T FindValue<T>(params Expression[] expressions)
            => FindValue<T>((IList<Expression>)expressions);

        protected static T FindValue<T>(IList<Expression> expressions)
        {
            var contsantExpressions = expressions.OfType<ConstantExpression>().ToList();
            return contsantExpressions.Select(e => e.Value)
                .Union(from exp in contsantExpressions
                       let v = exp.Value
                       from f in v?.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static |
                                                        BindingFlags.Public | BindingFlags.NonPublic |
                                                        BindingFlags.FlattenHierarchy)
                       select f.GetValue(v))
                .Union(from exp in contsantExpressions
                       let v = exp.Value
                       from f in v?.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static |
                                                            BindingFlags.Public | BindingFlags.NonPublic |
                                                            BindingFlags.FlattenHierarchy)
                       select f.GetValue(v))
                .Union(from exp in expressions.OfType<MemberExpression>()
                       let obj = (exp.Expression as ConstantExpression)?.Value
                       where obj != null
                       let field = exp.Member as FieldInfo
                       let property = exp.Member as PropertyInfo
                       select field?.GetValue(obj) ?? property?.GetValue(obj))
                .OfType<T>()
                .FirstOrDefault();
        }

        protected void AddMethodTranslator(string methodName, Action<MethodTranslationContext> translator)
            => this.AddMethodTranslator(new Regex($"^{methodName}$", RegexOptions.Compiled), translator);

        protected void AddMethodTranslator(Regex regex, Action<MethodTranslationContext> translator)
            => this.methodTranslatorsByRegex.Add((regex, translator));

        protected void Supports(Type type) => this.supportedTypes.Add(type);

        protected void Supports<T>() => this.Supports(typeof(T));
    }
}