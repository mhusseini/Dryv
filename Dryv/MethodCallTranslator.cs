using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Dryv
{
    internal class MethodCallTranslator
    {
        private static readonly MemberInfo ErrorMember = typeof(DryvResult).GetMember("Error").First();
        private static readonly StringFormatDissector StringFormatDissector = new StringFormatDissector();

        public string QuoteValue(object value)
        {
            return value == null
                ? "null"
                : (value.GetType().IsPrimitive
                    ? value.ToString()
                    : $@"""{value}""");
        }

        public string TranslateMethodCall(JavaScriptTranslator translator, MethodCallExpression expression, IndentingStringWriter writer, bool negated)
        {
            switch (expression.Method.Name)
            {
                case nameof(string.Equals):
                    translator.Visit((dynamic)expression.Object, writer);

                    var isCaseInsensitive = false;

                    var value = expression.Arguments.First();
                    if (expression.Arguments.Skip(1).FirstOrDefault() is ConstantExpression comparisonType
                        && comparisonType.Value is StringComparison stringComparison)
                    {
                        switch (stringComparison)
                        {
                            case StringComparison.CurrentCulture:
                            case StringComparison.InvariantCulture:
                            case StringComparison.Ordinal:
                                // compareLocale is currently not implemented in all browsers (especially not on Android),
                                // also, locale handling is still wuite cumbersome. To keep things simple, we'll just
                                // fall back to simple string comparison here.
                                break;

                            case StringComparison.CurrentCultureIgnoreCase:
                            case StringComparison.InvariantCultureIgnoreCase:
                            case StringComparison.OrdinalIgnoreCase:
                                // As stated above, locale hadling isn't very easy on the client side, so we'll just
                                // strick to simple case-insensitive sring comparison here.
                                isCaseInsensitive = true;
                                writer.Write(".toLowerCase()");
                                break;
                        }
                    }

                    writer.Write(negated ? "!==" : " === ");
                    this.WriteArguments(translator, new[] { value }, writer);
                    if (isCaseInsensitive)
                    {
                        writer.Write(".toLowerCase()");
                    }
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
                    {
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
                    }

                case nameof(Regex.IsMatch):
                    {
                        var result = FindRegularExpression(expression.Object);

                        if (result == null)
                        {
                            throw new MethodCallNotAllowedException(expression, "Could not determine regular expression.");
                        }

                        var clientRegexp = $"/{result.Value.Pattern}/{TranslateRegexOptions(result.Value.Options)}";
                        writer.Write(clientRegexp);
                        writer.Write(".test(");
                        this.WriteArguments(translator, expression.Arguments, writer);
                        writer.Write(")");
                        break;
                    }

                case nameof(Regex.Match):
                    {
                        var result = FindRegularExpression(expression.Object);

                        if (result == null)
                        {
                            throw new MethodCallNotAllowedException(expression, "Could not determine regular expression.");
                        }

                        var clientRegexp = $"/{result.Value.Pattern}/{TranslateRegexOptions(result.Value.Options)}";
                        writer.Write(clientRegexp);
                        writer.Write(".test(");
                        this.WriteArguments(translator, expression.Arguments, writer);
                        writer.Write(")");
                        return nameof(Match.Success);
                    }

                default:
                    throw new MethodCallNotAllowedException(expression);
            }

            return null;
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

        private static (string Pattern, RegexOptions Options)? FindRegularExpression(Expression exp)
        {
            (string Pattern, RegexOptions Options)? result = null;

            switch (exp)
            {
                case NewExpression newExpression:
                    result = (
                        Pattern: FindValue<string>(newExpression.Arguments),
                        Options: FindValue<RegexOptions>(newExpression.Arguments)
                    );
                    break;

                case MemberExpression memberExpression:
                    var regex = FindValue<Regex>(memberExpression.Expression);
                    if (regex != null)
                    {
                        result = (
                            Pattern: regex.ToString(),
                            Options: regex.Options
                        );
                    }

                    break;
            }

            return result;
        }

        private static T FindValue<T>(params Expression[] expressions) =>
                            FindValue<T>((IList<Expression>)expressions);

        private static T FindValue<T>(IList<Expression> expressions)
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

        private static string TranslateRegexOptions(RegexOptions options)
        {
            var sb = new StringBuilder();
            if (options.HasFlag(RegexOptions.IgnoreCase))
            {
                sb.Append("i");
            }

            if (options.HasFlag(RegexOptions.Multiline))
            {
                sb.Append("m");
            }

            if (options.HasFlag(RegexOptions.IgnorePatternWhitespace))
            {
                throw new ExpressionNotTranslatableException($"JavaScript regular expressions don't support {RegexOptions.IgnorePatternWhitespace}.");
            }

            if (options.HasFlag(RegexOptions.RightToLeft))
            {
                throw new ExpressionNotTranslatableException($"JavaScript regular expressions don't support {RegexOptions.RightToLeft}.");
            }

            var modifiers = sb.ToString();
            return modifiers;
        }
    }
}