using Dryv.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dryv.MethodCallTranslation
{
    public abstract class MethodCallTranslator : IMethodCallTranslator
    {
        private List<(Regex Method, Action<MethodTranslationParameters> Translator)> methodTranslatorsByRegex;
        protected abstract List<(string Method, Action<MethodTranslationParameters> Translator)> MethodTranslators { get; }

        public static string QuoteValue(object value)
        {
            return value == null
                ? "null"
                : (value.GetType().IsPrimitive
                    ? value.ToString()
                    : $@"""{value}""");
        }

        public static void WriteArguments(Translator translator, IEnumerable<Expression> arguments, IndentingStringWriter writer)
        {
            var sep = string.Empty;
            foreach (var argument in arguments)
            {
                writer.Write(sep);
                translator.Visit(argument, writer);
                sep = ", ";
            }
        }

        public abstract IList<Regex> TypeMatches { get; }

        public virtual bool Translate(MethodTranslationParameters options)
        {
            if (this.methodTranslatorsByRegex == null)
            {
                this.methodTranslatorsByRegex = this.MethodTranslators
                    .Select(i => (
                        Method: new Regex($"^{i.Method}$", RegexOptions.Compiled),
                        Translator: i.Translator
                        ))
                    .ToList();
            }

            var translator = this.methodTranslatorsByRegex.Where(i => i.Method.IsMatch(options.Expression.Method.Name)).Select(i => i.Translator).FirstOrDefault();

            if (translator == null)
            {
                throw new MethodCallNotAllowedException(options.Expression);
            }

            translator(options);
            return true;
        }

        protected static bool ArgumentIs<T>(MethodTranslationParameters options, int index, T value)
        {
            return Equals(options.Expression.Arguments
                .Skip(index)
                .Take(1)
                .OfType<ConstantExpression>()
                .Select(i => i.Value)
                .FirstOrDefault(), value);
        }

        protected static T FindValue<T>(params Expression[] expressions) =>
                            FindValue<T>((IList<Expression>)expressions);

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
    }
}