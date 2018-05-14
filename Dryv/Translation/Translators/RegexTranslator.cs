using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Dryv.Translation.Translators
{
    public class RegexTranslator : MethodCallTranslator, ICustomTranslator
    {
        private static readonly PropertyInfo SuccessProperty = typeof(Group).GetTypeInfo().GetDeclaredProperty(nameof(Group.Success));

        public RegexTranslator()
        {
            this.Supports<Regex>();
            this.AddMethodTranslator(nameof(Regex.IsMatch), IsMatch);
        }

        public bool TryTranslate(CustomTranslationContext context)
        {
            if (!(context.Expression is MemberExpression memberExpression))
            {
                return false;
            }

            if (!Equals(memberExpression.Member, SuccessProperty))
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
                throw new MethodNotSupportedException(methodCallExpression, "Could not determine regular context.Expression.");
            }

            var clientRegexp = $"/{result.Pattern}/{TranslateRegexOptions(result.Options)}";
            context.Writer.Write(clientRegexp);
            context.Writer.Write(".test(");
            WriteArguments(context.Translator, new[] { result.Test }, context);
            context.Writer.Write(")");

            return true;
        }

        private static FindRegularExpressionResult FindRegularExpression(MethodCallExpression methodCallExpression)
        {
            FindRegularExpressionResult result = null;
            var exp = methodCallExpression.Object;

            switch (exp)
            {
                case NewExpression newExpression:
                    result = new FindRegularExpressionResult(
                        FindValue<string>(newExpression.Arguments),
                        methodCallExpression.Arguments.First(),
                        FindValue<RegexOptions>(newExpression.Arguments)
                    );
                    break;

                case MemberExpression memberExpression:
                    var regex = FindValue<Regex>(memberExpression.Expression);
                    if (regex != null)
                    {
                        result = new FindRegularExpressionResult(
                            regex.ToString(),
                            methodCallExpression.Arguments.First(),
                            regex.Options
                        );
                    }

                    break;

                case null:
                    result = new FindRegularExpressionResult(
                        FindValue<string>(methodCallExpression.Arguments),
                        methodCallExpression.Arguments.Skip(1).First(),
                        FindValue<RegexOptions>(methodCallExpression.Arguments)
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
                throw new MethodNotSupportedException(context.Expression, "Could not determine regular context.Expression.");
            }

            var clientRegexp = $"/{result.Pattern}/{TranslateRegexOptions(result.Options)}";
            context.Writer.Write(clientRegexp);
            context.Writer.Write(".test(");
            WriteArguments(context.Translator, new[] { result.Test }, context);
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
                throw new ExpressionNotSupportedException($"{RegexOptions.IgnorePatternWhitespace} not translatable to JavaScript.");
            }

            if (context.HasFlag(RegexOptions.RightToLeft))
            {
                throw new ExpressionNotSupportedException($"{RegexOptions.RightToLeft} not translatable to JavaScript.");
            }

            return sb.ToString();
        }

        private class FindRegularExpressionResult
        {
            public FindRegularExpressionResult(string pattern, Expression test, RegexOptions options)
            {
                this.Pattern = pattern;
                this.Test = test;
                this.Options = options;
            }

            public RegexOptions Options { get; }
            public string Pattern { get; }
            public Expression Test { get; }
        }
    }
}