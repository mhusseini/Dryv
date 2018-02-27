using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dryv
{
    internal class MethodCallTranslator
    {
        private static readonly MemberInfo ErrorMember = typeof(DryvResult).GetMember("Error").First();
        private static readonly StringFormatDissector StringFormatDissector = new StringFormatDissector();

        public void TranslateMethodCall(JavaScriptTranslator translator, MethodCallExpression expression, IndentingStringWriter writer, bool negated)
        {
            switch (expression.Method.Name)
            {
                case nameof(object.Equals):
                    translator.Visit((dynamic)expression.Object, writer);
                    writer.Write(negated ? "!==" : " === ");
                    this.WriteArguments(translator, expression.Arguments, writer);
                    break;

                case nameof(string.Contains):
                    translator.Visit((dynamic)expression.Object, writer);
                    writer.Write(".indexOf(");
                    this.WriteArguments(translator, expression.Arguments, writer);
                    writer.Write(negated ? ") < 0" : ") >= 0");
                    break;

                case nameof(string.StartsWith):
                    translator.Visit((dynamic)expression.Object, writer);
                    writer.Write(".indexOf(");
                    this.WriteArguments(translator, expression.Arguments, writer);
                    writer.Write(negated ? ") !== 0" : ") === 0");
                    break;

                case nameof(string.ToLower):
                    if (!negated)
                    {
                        writer.Write("!");
                    }
                    translator.VisitWithBrackets(expression.Object, writer);
                    writer.Write(".toLowerCase()");

                    break;

                case nameof(string.ToUpper):
                    if (!negated)
                    {
                        writer.Write("!");
                    }

                    translator.VisitWithBrackets(expression.Object, writer);
                    writer.Write(".toUpperCase()");

                    break;

                case nameof(DryvResult.Error):
                    if (expression.Method != ErrorMember)
                    {
                        throw new MethodCallNotAllowedException(expression);
                    }

                    writer.Write(expression.Arguments.First());
                    break;

                case nameof(string.IsNullOrEmpty):
                    if (!negated)
                    {
                        writer.Write("!");
                    }

                    translator.VisitWithBrackets(expression.Arguments.First(), writer);
                    break;

                case nameof(string.IsNullOrWhiteSpace):
                    if (!negated)
                    {
                        writer.Write("!");
                    }

                    writer.Write(@"/\S/.test(");
                    translator.VisitWithBrackets(expression.Arguments.First(), writer);
                    writer.Write(@" || """")");
                    break;

                case nameof(string.Format):
                    if (!(expression.Arguments.First() is ConstantExpression pattern))
                    {
                        throw new ExpressionNotSupportedException("Calls to string.Format with non-constant pattern strings are not supported.");
                    }

                    var arguments = expression.Arguments.Skip(1).Cast<object>().ToArray();
                    var parts = StringFormatDissector.Recombine(pattern.Value.ToString(), arguments);

                    for (var index = 0; index < parts.Count; index++)
                    {
                        var part = parts[index];
                        switch (part)
                        {
                            case string text:
                                if (index > 0)
                                {
                                    writer.Write(" + ");
                                }

                                writer.Write(this.QuoteValue(text));

                                if (index < parts.Count - 1)
                                {
                                    writer.Write(" + ");
                                }

                                break;

                            case Expression exp:
                                translator.VisitWithBrackets(exp, writer);
                                break;
                        }
                    }

                    break;

                default:
                    throw new MethodCallNotAllowedException(expression);
            }
        }

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
    }
}