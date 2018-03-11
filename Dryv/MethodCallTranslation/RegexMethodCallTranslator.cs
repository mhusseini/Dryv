using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Dryv.MethodCallTranslation
{
    internal class RegexMethodCallTranslator : MethodCallTranslatorBase
    {
        public override bool Translate(MethodTranslationOptions options)
        {
            switch (options.Expression.Method.Name)
            {
                case nameof(Regex.IsMatch):
                    {
                        var result = FindRegularExpression(options.Expression.Object);

                        if (result == null)
                        {
                            throw new MethodCallNotAllowedException(options.Expression, "Could not determine regular options.Expression.");
                        }

                        var clientRegexp = $"/{result.Value.Pattern}/{TranslateRegexOptions(result.Value.Options)}";
                        options.Writer.Write(clientRegexp);
                        options.Writer.Write(".test(");
                        this.WriteArguments(options.Translator, options.Expression.Arguments, options.Writer);
                        options.Writer.Write(")");
                        return true;
                    }

                case nameof(Regex.Match):
                    {
                        var result = FindRegularExpression(options.Expression.Object);

                        if (result == null)
                        {
                            throw new MethodCallNotAllowedException(options.Expression, "Could not determine regular options.Expression.");
                        }

                        var clientRegexp = $"/{result.Value.Pattern}/{TranslateRegexOptions(result.Value.Options)}";
                        options.Writer.Write(clientRegexp);
                        options.Writer.Write(".test(");
                        this.WriteArguments(options.Translator, options.Expression.Arguments, options.Writer);
                        options.Writer.Write(")");
                        options.Result = nameof(Match.Success);
                        return true;
                    }

                default:
                    throw new MethodCallNotAllowedException(options.Expression);
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