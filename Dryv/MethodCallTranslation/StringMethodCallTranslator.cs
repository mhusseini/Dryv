using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dryv.MethodCallTranslation
{
    internal class StringMethodCallTranslator : MethodCallTranslatorBase
    {
        public override bool Translate(MethodTranslationOptions options)
        {
            switch (options.Expression.Method.Name)
            {
                case nameof(string.Equals):
                    {
                        options.Translator.Visit((dynamic)options.Expression.Object, options.Writer);

                        var value = options.Expression.Arguments.First();
                        var isCaseInsensitive = GetIsCaseInsensitive(options.Expression);
                        if (isCaseInsensitive)
                        {
                            options.Writer.Write(".toLowerCase()");
                        }

                        options.Writer.Write(options.Negated ? "!==" : " === ");
                        this.WriteArguments(options.Translator, new[] { value }, options.Writer);
                        if (isCaseInsensitive)
                        {
                            options.Writer.Write(".toLowerCase()");
                        }

                        return true;
                    }

                case nameof(string.Contains):
                    {
                        options.Translator.Visit((dynamic)options.Expression.Object, options.Writer);

                        var isCaseInsensitive = GetIsCaseInsensitive(options.Expression);
                        if (isCaseInsensitive)
                        {
                            options.Writer.Write(".toLowerCase()");
                        }

                        options.Writer.Write(".indexOf(");
                        this.WriteArguments(options.Translator, options.Expression.Arguments, options.Writer);
                        if (isCaseInsensitive)
                        {
                            options.Writer.Write(".toLowerCase()");
                        }

                        options.Writer.Write(options.Negated ? ") < 0" : ") >= 0");
                        return true;
                    }

                case nameof(string.StartsWith):
                    {
                        options.Translator.Visit((dynamic)options.Expression.Object, options.Writer);

                        var isCaseInsensitive = GetIsCaseInsensitive(options.Expression);
                        if (isCaseInsensitive)
                        {
                            options.Writer.Write(".toLowerCase()");
                        }
                        options.Writer.Write(".indexOf(");
                        this.WriteArguments(options.Translator, options.Expression.Arguments, options.Writer);
                        if (isCaseInsensitive)
                        {
                            options.Writer.Write(".toLowerCase()");
                        }
                        options.Writer.Write(options.Negated ? ") !== 0" : ") === 0");
                        return true;
                    }

                case nameof(string.EndsWith):
                    {
                        options.Translator.Visit((dynamic)options.Expression.Object, options.Writer);

                        var isCaseInsensitive = GetIsCaseInsensitive(options.Expression);
                        if (isCaseInsensitive)
                        {
                            options.Writer.Write(".toLowerCase()");
                        }
                        options.Writer.Write(".indexOf(");
                        options.Translator.Visit((dynamic)options.Expression.Arguments[0], options.Writer);
                        if (isCaseInsensitive)
                        {
                            options.Writer.Write(".toLowerCase()");
                        }
                        options.Writer.Write(options.Negated ? ") !== (" : ") === (");
                        options.Translator.VisitWithBrackets((dynamic)options.Expression.Object, options.Writer);
                        options.Writer.Write(".length - ");
                        options.Translator.VisitWithBrackets((dynamic)options.Expression.Arguments[0], options.Writer);
                        options.Writer.Write(".length)");
                        return true;
                    }

                case nameof(string.ToLower):
                    {
                        if (!options.Negated)
                        {
                            options.Writer.Write("!");
                        }

                        options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
                        options.Writer.Write(".toLowerCase()");

                        return true;
                    }

                case nameof(string.ToUpper):
                    {
                        if (!options.Negated)
                        {
                            options.Writer.Write("!");
                        }

                        options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
                        options.Writer.Write(".toUpperCase()");

                        return true;
                    }

                case nameof(string.IsNullOrEmpty):
                    {
                        if (!options.Negated)
                        {
                            options.Writer.Write("!");
                        }

                        options.Translator.VisitWithBrackets(options.Expression.Arguments.First(), options.Writer);
                        return true;
                    }

                case nameof(string.IsNullOrWhiteSpace):
                    {
                        if (!options.Negated)
                        {
                            options.Writer.Write("!");
                        }

                        options.Writer.Write(@"/\S/.test(");
                        options.Translator.VisitWithBrackets(options.Expression.Arguments.First(), options.Writer);
                        options.Writer.Write(@" || """")");
                        return true;
                    }

                case nameof(string.Trim):
                    {
                        options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
                        options.Writer.Write(".trim()");
                        return true;
                    }

                case nameof(string.TrimEnd):
                    {
                        options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
                        options.Writer.Write(".trimRight()");
                        return true;
                    }

                case nameof(string.Normalize):
                    {
                        options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
                        options.Writer.Write(".normalize()");
                        return true;
                    }

                case nameof(string.Compare):
                    {
                        var arguments = options.Expression.Arguments;
                        options.Translator.VisitWithBrackets(arguments[0], options.Writer);
                        var isCaseInsensitive = GetIsCaseInsensitive(options.Expression);
                        if (isCaseInsensitive)
                        {
                            options.Writer.Write(".toLowerCase()");
                        }
                        options.Writer.Write(".localeCompare(");
                        options.Translator.Visit((dynamic)arguments[1], options.Writer);
                        if (isCaseInsensitive)
                        {
                            options.Writer.Write(".toLowerCase()");
                        }
                        options.Writer.Write(")");

                        return true;
                    }

                case nameof(string.CompareTo):
                    {
                        var arguments = options.Expression.Arguments;
                        options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
                        options.Writer.Write(".localeCompare(");
                        options.Translator.Visit((dynamic)arguments[0], options.Writer);
                        options.Writer.Write(")");

                        return true;
                    }

                case nameof(string.IndexOf):
                    {
                        var arguments = options.Expression.Arguments;
                        options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
                        var isCaseInsensitive = GetIsCaseInsensitive(options.Expression);
                        if (isCaseInsensitive)
                        {
                            options.Writer.Write(".toLowerCase()");
                        }
                        options.Writer.Write(".indexOf(");
                        options.Translator.VisitWithBrackets((dynamic)arguments[1], options.Writer);
                        if (isCaseInsensitive)
                        {
                            options.Writer.Write(".toLowerCase()");
                        }
                        options.Writer.Write(")");

                        return true;
                    }

                case nameof(string.TrimStart):
                    {
                        options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
                        options.Writer.Write(".trimLeft()");
                        return true;
                    }

                case nameof(string.Format):
                    {
                        if (!(options.Expression.Arguments.First() is ConstantExpression pattern))
                        {
                            throw new ExpressionNotSupportedException("Calls to string.Format with non-constant pattern strings are not supported.");
                        }

                        var arguments = options.Expression.Arguments.Skip(1).Cast<object>().ToArray();
                        var parts = StringFormatDissector.Recombine(pattern.Value.ToString(), arguments);

                        for (var index = 0; index < parts.Count; index++)
                        {
                            var part = parts[index];
                            switch (part)
                            {
                                case string text:
                                    if (index > 0)
                                    {
                                        options.Writer.Write(" + ");
                                    }

                                    options.Writer.Write(this.QuoteValue(text));

                                    if (index < parts.Count - 1)
                                    {
                                        options.Writer.Write(" + ");
                                    }

                                    break;

                                case ConstantExpression exp:
                                    options.Translator.VisitWithBrackets(exp, options.Writer);
                                    break;
                            }
                        }

                        return true;
                    }

                default:
                    throw new MethodCallNotAllowedException(options.Expression);
            }
        }

        protected static bool GetIsCaseInsensitive(MethodCallExpression expression)
        {
            var stringComparison = (from exp in expression.Arguments
                                    let constExp = exp as ConstantExpression
                                    where constExp?.Value is StringComparison
                                    select (StringComparison?)constExp.Value).FirstOrDefault();

            if (stringComparison == null)
            {
                return false;
            }

            switch (stringComparison.Value)
            {
                case StringComparison.CurrentCultureIgnoreCase:
                case StringComparison.InvariantCultureIgnoreCase:
                case StringComparison.OrdinalIgnoreCase:
                    // As stated above, locale hadling isn't very easy on the client side, so we'll just
                    // strick to simple case-insensitive sring comparison here.
                    return true;

                default:
                    // compareLocale is currently not implemented in all browsers (especially not on Android),
                    // also, locale handling is still wuite cumbersome. To keep things simple, we'll just
                    // fall back to simple string comparison here.
                    return false;
            }
        }
    }
}