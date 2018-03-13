using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Dryv.MethodCallTranslation
{
    internal class RegexMethodCallTranslator : MethodCallTranslator, IGenericTranslator
    {
        private static readonly PropertyInfo SuccessProperty = typeof(Group).GetProperty(nameof(Group.Success));

        public RegexMethodCallTranslator()
        {
            this.AddMethodTranslator(nameof(Regex.IsMatch), IsMatch);
        }

        public override IList<Regex> TypeMatches { get; } = new[] { new Regex(typeof(Regex).FullName, RegexOptions.Compiled) };

        public bool TryTranslate(TranslationParameters parameters)
        {
            if (!(parameters.Expression is MemberExpression memberExpression))
            {
                return false;
            }

            if (memberExpression.Member != SuccessProperty)
            {
                return false;
            }

            if (!(memberExpression.Expression is MethodCallExpression methodCallExpression))
            {
                return false;
            }

            var result = FindRegularExpression(methodCallExpression.Object);

            if (result == null)
            {
                throw new MethodCallNotAllowedException(methodCallExpression, "Could not determine regular parameters.Expression.");
            }

            var clientRegexp = $"/{result.Value.Pattern}/{TranslateRegexOptions(result.Value.Options)}";
            parameters.Writer.Write(clientRegexp);
            parameters.Writer.Write(".test(");
            WriteArguments(parameters.Translator, methodCallExpression.Arguments, parameters.Writer);
            parameters.Writer.Write(")");

            return true;
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

        private static void IsMatch(MethodTranslationParameters parameters)
        {
            var result = FindRegularExpression(parameters.Expression.Object);

            if (result == null)
            {
                throw new MethodCallNotAllowedException(parameters.Expression, "Could not determine regular parameters.Expression.");
            }

            var clientRegexp = $"/{result.Value.Pattern}/{TranslateRegexOptions(result.Value.Options)}";
            parameters.Writer.Write(clientRegexp);
            parameters.Writer.Write(".test(");
            WriteArguments(parameters.Translator, parameters.Expression.Arguments, parameters.Writer);
            parameters.Writer.Write(")");
        }

        private static string TranslateRegexOptions(RegexOptions parameters)
        {
            var sb = new StringBuilder();
            if (parameters.HasFlag(RegexOptions.IgnoreCase))
            {
                sb.Append("i");
            }

            if (parameters.HasFlag(RegexOptions.Multiline))
            {
                sb.Append("m");
            }

            if (parameters.HasFlag(RegexOptions.IgnorePatternWhitespace))
            {
                throw new ExpressionNotTranslatableException($"{RegexOptions.IgnorePatternWhitespace} not translatable to JavaScript.");
            }

            if (parameters.HasFlag(RegexOptions.RightToLeft))
            {
                throw new ExpressionNotTranslatableException($"{RegexOptions.RightToLeft} not translatable to JavaScript.");
            }

            var modifiers = sb.ToString();
            return modifiers;
        }
    }
}