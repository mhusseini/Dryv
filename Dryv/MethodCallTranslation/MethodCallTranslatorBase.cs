using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Translation;

namespace Dryv.MethodCallTranslation
{
    internal abstract class MethodCallTranslatorBase
    {
        protected static readonly StringFormatDissector StringFormatDissector = new StringFormatDissector();

        public abstract bool Translate(MethodTranslationOptions options);

        public string QuoteValue(object value)
        {
            return value == null
                ? "null"
                : (value.GetType().IsPrimitive
                    ? value.ToString()
                    : $@"""{value}""");
        }
        public void WriteArguments(Translator translator, IEnumerable<Expression> arguments, IndentingStringWriter writer)
        {
            var sep = string.Empty;
            foreach (var argument in arguments)
            {
                writer.Write(sep);
                translator.Visit((dynamic)argument, writer);
                sep = ", ";
            }
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