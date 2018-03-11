using Dryv.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dryv.MethodCallTranslation
{
    internal abstract class MethodCallTranslatorBase
    {
        protected static readonly StringFormatDissector StringFormatDissector = new StringFormatDissector();

        protected virtual Dictionary<string, Action<MethodTranslationOptions>> MethodTranslators { get; }

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
                translator.Visit((dynamic)argument, writer);
                sep = ", ";
            }
        }

        public virtual bool Translate(MethodTranslationOptions options)
        {
            if (!this.MethodTranslators.TryGetValue(options.Expression.Method.Name, out var translator))
            {
                throw new MethodCallNotAllowedException(options.Expression);
            }

            translator(options);
            return true;
        }

        protected static bool ArgumentIs<T>(MethodTranslationOptions options, int index, T value)
        {
            return Equals((options.Expression.Arguments[index] as ConstantExpression)?.Value, value);
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