using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Dryv.MethodCallTranslation
{
    internal class RegexMethodCallTranslator : MethodCallTranslator
    {
        protected override List<(string Method, Action<MethodTranslationParameters> Translator)> MethodTranslators { get; } = new List<(string Method, Action<MethodTranslationParameters> Translator)>
        {
            (nameof(Regex.IsMatch), IsMatch),
            (nameof(Regex.Match), Match)
        };

        public override IList<Regex> TypeMatches { get; } = new[] { new Regex(typeof(Regex).FullName, RegexOptions.Compiled) };

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

        private static void IsMatch(MethodTranslationParameters options)
        {
            var result = FindRegularExpression(options.Expression.Object);

            if (result == null)
            {
                throw new MethodCallNotAllowedException(options.Expression, "Could not determine regular options.Expression.");
            }

            var clientRegexp = $"/{result.Value.Pattern}/{TranslateRegexOptions(result.Value.Options)}";
            options.Writer.Write(clientRegexp);
            options.Writer.Write(".test(");
            WriteArguments(options.Translator, options.Expression.Arguments, options.Writer);
            options.Writer.Write(")");
        }

        private static void Match(MethodTranslationParameters options)
        {
            var result = FindRegularExpression(options.Expression.Object);

            if (result == null)
            {
                throw new MethodCallNotAllowedException(options.Expression, "Could not determine regular options.Expression.");
            }

            var clientRegexp = $"/{result.Value.Pattern}/{TranslateRegexOptions(result.Value.Options)}";
            options.Writer.Write(clientRegexp);
            options.Writer.Write(".test(");
            WriteArguments(options.Translator, options.Expression.Arguments, options.Writer);
            options.Writer.Write(")");
            options.Result = nameof(System.Text.RegularExpressions.Match.Success);
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
                throw new ExpressionNotTranslatableException($"{RegexOptions.IgnorePatternWhitespace} not translatable to JavaScript.");
            }

            if (options.HasFlag(RegexOptions.RightToLeft))
            {
                throw new ExpressionNotTranslatableException($"{RegexOptions.RightToLeft} not translatable to JavaScript.");
            }

            var modifiers = sb.ToString();
            return modifiers;
        }
    }
}