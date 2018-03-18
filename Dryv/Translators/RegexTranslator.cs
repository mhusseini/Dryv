using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Dryv.MethodCallTranslation
{
    internal class RegexTranslator : MethodCallTranslator, IGenericTranslator
    {
        private static readonly PropertyInfo SuccessProperty = typeof(Group).GetProperty(nameof(Group.Success));

        public RegexTranslator()
        {
            this.Supports<Regex>();
            this.AddMethodTranslator(nameof(Regex.IsMatch), IsMatch);
        }

        public bool TryTranslate(GenericTranslationContext context)
        {
            if (!(context.Expression is MemberExpression memberExpression))
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

            var result = FindRegularExpression(methodCallExpression);

            if (result == null)
            {
                throw new MethodCallNotAllowedException(methodCallExpression, "Could not determine regular context.Expression.");
            }

            var clientRegexp = $"/{result.Value.Pattern}/{TranslateRegexOptions(result.Value.Options)}";
            context.Writer.Write(clientRegexp);
            context.Writer.Write(".test(");
            WriteArguments(context.Translator, new[] { result.Value.Test }, context);
            context.Writer.Write(")");

            return true;
        }

        private static (string Pattern, Expression Test, RegexOptions Options)? FindRegularExpression(MethodCallExpression methodCallExpression)
        {
            (string Pattern, Expression Test, RegexOptions Options)? result = null;
            var exp = methodCallExpression.Object;

            switch (exp)
            {
                case NewExpression newExpression:
                    result = (
                        Pattern: FindValue<string>(newExpression.Arguments),
                        Test: methodCallExpression.Arguments.First(),
                        Options: FindValue<RegexOptions>(newExpression.Arguments)
                    );
                    break;

                case MemberExpression memberExpression:
                    var regex = FindValue<Regex>(memberExpression.Expression);
                    if (regex != null)
                    {
                        result = (
                            Pattern: regex.ToString(),
                            Test: methodCallExpression.Arguments.First(),
                            Options: regex.Options
                        );
                    }

                    break;

                case null:
                    result = (
                        Pattern: FindValue<string>(methodCallExpression.Arguments),
                        Test: methodCallExpression.Arguments.Skip(1).First(),
                        Options: FindValue<RegexOptions>(methodCallExpression.Arguments)
                    );
                    break;
            }

            return result;
        }

        private static void IsMatch(MethodTranslationContext context)
        {
            var result = FindRegularExpression(context.Expression);

            if (result == null)
            {
                throw new MethodCallNotAllowedException(context.Expression, "Could not determine regular context.Expression.");
            }

            var clientRegexp = $"/{result.Value.Pattern}/{TranslateRegexOptions(result.Value.Options)}";
            context.Writer.Write(clientRegexp);
            context.Writer.Write(".test(");
            WriteArguments(context.Translator, new[] { result.Value.Test }, context);
            context.Writer.Write(")");
        }

        private static string TranslateRegexOptions(RegexOptions context)
        {
            var sb = new StringBuilder();
            if (context.HasFlag(RegexOptions.IgnoreCase))
            {
                sb.Append("i");
            }

            if (context.HasFlag(RegexOptions.Multiline))
            {
                sb.Append("m");
            }

            if (context.HasFlag(RegexOptions.IgnorePatternWhitespace))
            {
                throw new ExpressionNotTranslatableException($"{RegexOptions.IgnorePatternWhitespace} not translatable to JavaScript.");
            }

            if (context.HasFlag(RegexOptions.RightToLeft))
            {
                throw new ExpressionNotTranslatableException($"{RegexOptions.RightToLeft} not translatable to JavaScript.");
            }

            return sb.ToString();
        }
    }
}