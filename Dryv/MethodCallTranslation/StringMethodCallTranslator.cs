using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Dryv.MethodCallTranslation
{
    internal class StringMethodCallTranslator : MethodCallTranslator
    {
        private static readonly StringFormatDissector StringFormatDissector = new StringFormatDissector();
        public override IList<Regex> TypeMatches { get; } = new[] { new Regex(typeof(String).FullName, RegexOptions.Compiled) };

        protected override List<(string Method, Action<MethodTranslationParameters> Translator)> MethodTranslators { get; } = new List<(string Method, Action<MethodTranslationParameters> Translator)>
        {
            (nameof(string.Equals), Equals),
            (nameof(string.Contains), Contains),
            (nameof(string.StartsWith), StartsWith),
            (nameof(string.EndsWith), EndsWith),
            (nameof(string.ToLower), ToLower),
            (nameof(string.ToUpper), ToUpper),
            (nameof(string.IsNullOrEmpty), IsNullOrEmpty),
            (nameof(string.IsNullOrWhiteSpace), IsNullOrWhiteSpace),
            (nameof(string.Trim), Trim),
            (nameof(string.TrimEnd), TrimEnd),
            (nameof(string.Normalize), Normalize),
            (nameof(string.Compare), Compare),
            (nameof(string.CompareTo), CompareTo),
            (nameof(string.IndexOf), IndexOf),
            (nameof(string.TrimStart), TrimStart),
            (nameof(string.Format), Format)
        };

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

        private static void Compare(MethodTranslationParameters options)
        {
            /*
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length, bool ignoreCase);
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length, StringComparison comparisonType);
             √ public static int Compare(String strA, String strB);
             √ public static int Compare(String strA, String strB, bool ignoreCase);
             √! public static int Compare(String strA, String strB, bool ignoreCase, CultureInfo culture);
             √ public static int Compare(String strA, String strB, CultureInfo culture, CompareOptions options);
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length, bool ignoreCase, CultureInfo culture);
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length, CultureInfo culture, CompareOptions options);
             √ public static int Compare(String strA, String strB, StringComparison comparisonType);
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length);
             */
            if (options.Expression.Method.GetParameters().Any(p => p.ParameterType == typeof(int)))
            {
                throw new MethodCallNotAllowedException(options.Expression, "Only override without any indexes can be translated to JavaScript");
            }
            var arguments = options.Expression.Arguments;
            options.Translator.VisitWithBrackets(arguments.FirstOrDefault(), options.Writer);
            var isCaseInsensitive = ArgumentIs(options, 2, true) || GetIsCaseInsensitive(options.Expression);
            if (isCaseInsensitive)
            {
                options.Writer.Write(".toLowerCase()");
            }

            options.Writer.Write(".localeCompare(");
            options.Translator.Visit(arguments[1], options.Writer);
            if (isCaseInsensitive)
            {
                options.Writer.Write(".toLowerCase()");
            }

            options.Writer.Write(")");
        }

        private static void CompareTo(MethodTranslationParameters options)
        {
            /*
             √ public int CompareTo(String strB);
             √ public int CompareTo(object value);
             */
            var arguments = options.Expression.Arguments;
            options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
            options.Writer.Write(".localeCompare(");
            options.Translator.Visit(arguments[0], options.Writer);
            options.Writer.Write(")");
        }

        private static void Contains(MethodTranslationParameters options)
        {
            /*
             √ public bool Contains(String value)
             */
            options.Translator.Visit(options.Expression.Object, options.Writer);

            var isCaseInsensitive = GetIsCaseInsensitive(options.Expression);
            if (isCaseInsensitive)
            {
                options.Writer.Write(".toLowerCase()");
            }

            options.Writer.Write(".indexOf(");
            WriteArguments(options.Translator, options.Expression.Arguments, options.Writer);
            if (isCaseInsensitive)
            {
                options.Writer.Write(".toLowerCase()");
            }

            options.Writer.Write(options.Negated ? ") < 0" : ") >= 0");
        }

        private static void EndsWith(MethodTranslationParameters options)
        {
            /*
             √ public bool EndsWith(String value);
             √ public bool EndsWith(String value, bool ignoreCase, CultureInfo culture);
             √ public bool EndsWith(String value, StringComparison comparisonType);
             √ public bool EndsWith(char value);
             */
            options.Translator.Visit(options.Expression.Object, options.Writer);

            var isCaseInsensitive = ArgumentIs(options, 1, true) || GetIsCaseInsensitive(options.Expression);
            if (isCaseInsensitive)
            {
                options.Writer.Write(".toLowerCase()");
            }

            options.Writer.Write(".indexOf(");
            options.Translator.Visit(options.Expression.Arguments[0], options.Writer);
            if (isCaseInsensitive)
            {
                options.Writer.Write(".toLowerCase()");
            }

            options.Writer.Write(options.Negated ? ") !== (" : ") === (");
            options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
            options.Writer.Write(".length - ");
            options.Translator.VisitWithBrackets(options.Expression.Arguments[0], options.Writer);
            options.Writer.Write(".length)");
        }

        private static void Equals(MethodTranslationParameters options)
        {
            /*
             √ public static bool Equals(String a, String b, StringComparison comparisonType);
             √ public static bool Equals(String a, String b);
             */
            options.Translator.Visit(options.Expression.Object, options.Writer);

            var value = options.Expression.Arguments.First();
            var isCaseInsensitive = GetIsCaseInsensitive(options.Expression);
            if (isCaseInsensitive)
            {
                options.Writer.Write(".toLowerCase()");
            }

            options.Writer.Write(options.Negated ? "!==" : " === ");
            WriteArguments(options.Translator, new[] { value }, options.Writer);
            if (isCaseInsensitive)
            {
                options.Writer.Write(".toLowerCase()");
            }
        }

        private static void Format(MethodTranslationParameters options)
        {
            /*
             Х public static String Format(IFormatProvider provider, String format, object arg0);
             Х public static String Format(IFormatProvider provider, String format, object arg0, object arg1, object arg2);
             Х public static String Format(IFormatProvider provider, String format, params object[] args);
             √ public static String Format(String format, object arg0);
             √! public static String Format(String format, params object[] args);
             √ public static String Format(String format, object arg0, object arg1, object arg2);
             √ public static String Format(String format, object arg0, object arg1);
             Х public static String Format(IFormatProvider provider, String format, object arg0, object arg1);
             */
            if (!(options.Expression.Arguments.First() is ConstantExpression pattern))
            {
                throw new ExpressionNotSupportedException("Calls to string.Format with non-constant pattern strings are not supported.");
            }

            if (options.Expression.Method.GetParameters().First().ParameterType == typeof(IFormatProvider))
            {
                throw new MethodCallNotAllowedException(options.Expression, "Only override with first parameter being a string can be translated to JavaScript");
            }

            if (options.Expression.Arguments.OfType<NewArrayExpression>().Any())
            {
                throw new ExpressionNotSupportedException("Calls to string.Format with arguments being an array are not supported.");
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

                        options.Writer.Write(QuoteValue(text));

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
        }

        private static void IndexOf(MethodTranslationParameters options)
        {
            /*
             √ public int IndexOf(char value);
             Х public int IndexOf(char value, int startIndex);
             Х public int IndexOf(char value, int startIndex, int count);
             √ public int IndexOf(String value);
             Х public int IndexOf(String value, int startIndex, int count);
             Х public int IndexOf(String value, int startIndex, int count, StringComparison comparisonType);
             Х public int IndexOf(String value, int startIndex, StringComparison comparisonType);
             √ public int IndexOf(String value, StringComparison comparisonType);
             Х public int IndexOf(String value, int startIndex);
             */
            if (options.Expression.Method.GetParameters().Any(p => p.ParameterType == typeof(int)))
            {
                throw new MethodCallNotAllowedException(options.Expression, "Only override without any indexes can be translated to JavaScript");
            }

            var arguments = options.Expression.Arguments;
            options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
            var isCaseInsensitive = GetIsCaseInsensitive(options.Expression);
            if (isCaseInsensitive)
            {
                options.Writer.Write(".toLowerCase()");
            }

            options.Writer.Write(".indexOf(");
            options.Translator.VisitWithBrackets(arguments[1], options.Writer);
            if (isCaseInsensitive)
            {
                options.Writer.Write(".toLowerCase()");
            }

            options.Writer.Write(")");
        }

        private static void IsNullOrEmpty(MethodTranslationParameters options)
        {
            /*
             √ public static bool IsNullOrEmpty(String value);
             */
            if (!options.Negated)
            {
                options.Writer.Write("!");
            }

            options.Translator.VisitWithBrackets(options.Expression.Arguments.First(), options.Writer);
        }

        private static void IsNullOrWhiteSpace(MethodTranslationParameters options)
        {
            /*
             √ public static bool IsNullOrWhiteSpace(String value);
             */
            if (!options.Negated)
            {
                options.Writer.Write("!");
            }

            options.Writer.Write(@"/\S/.test(");
            options.Translator.VisitWithBrackets(options.Expression.Arguments.First(), options.Writer);
            options.Writer.Write(@" || """")");
        }

        private static void Normalize(MethodTranslationParameters options)
        {
            /*
             √ public String Normalize();
             √! public String Normalize(NormalizationForm normalizationForm);
             */
            options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
            options.Writer.Write(".normalize()");
        }

        private static void StartsWith(MethodTranslationParameters options)
        {
            /*
             √ public bool StartsWith(char value);
             √ public bool StartsWith(String value);
             √! public bool StartsWith(String value, bool ignoreCase, CultureInfo culture);
             √ public bool StartsWith(String value, StringComparison comparisonType);
             */
            options.Translator.Visit(options.Expression.Object, options.Writer);
            var isCaseInsensitive = ArgumentIs(options, 1, true) || GetIsCaseInsensitive(options.Expression);
            if (isCaseInsensitive)
            {
                options.Writer.Write(".toLowerCase()");
            }

            options.Writer.Write(".indexOf(");
            WriteArguments(options.Translator, options.Expression.Arguments, options.Writer);
            if (isCaseInsensitive)
            {
                options.Writer.Write(".toLowerCase()");
            }

            options.Writer.Write(options.Negated ? ") !== 0" : ") === 0");
        }

        private static void ToLower(MethodTranslationParameters options)
        {
            /*
             √ public String ToLower();
             √! public String ToLower(CultureInfo culture);
             */
            if (!options.Negated)
            {
                options.Writer.Write("!");
            }

            options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
            options.Writer.Write(".toLowerCase()");
        }

        private static void ToUpper(MethodTranslationParameters options)
        {
            /*
             √ public String ToUpper();
             √! public String ToUpper(CultureInfo culture);
             */
            if (!options.Negated)
            {
                options.Writer.Write("!");
            }

            options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
            options.Writer.Write(".toUpperCase()");
        }

        private static void Trim(MethodTranslationParameters options)
        {
            /*
             √ public String Trim(char trimChar);
             Х public String Trim(params char[] trimChars);
             Х public String Trim();
             */
            if (options.Expression.Arguments.Any())
            {
                throw new MethodCallNotAllowedException(options.Expression, "Only override without any arguments can be translated to JavaScript");
            }

            options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
            options.Writer.Write(".trim()");
        }

        private static void TrimEnd(MethodTranslationParameters options)
        {
            /*
             √ public String TrimEnd();
             Х public String TrimEnd(char trimChar);
             Х public String TrimEnd(params char[] trimChars);
             */
            if (options.Expression.Arguments.Any())
            {
                throw new MethodCallNotAllowedException(options.Expression, "Only override without any arguments can be translated to JavaScript");
            }

            options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
            options.Writer.Write(".trimRight()");
        }

        private static void TrimStart(MethodTranslationParameters options)
        {
            /*
             √ public String TrimStart();
             Х public String TrimStart(char trimChar);
             Х public String TrimStart(params char[] trimChars);
             */
            if (options.Expression.Arguments.Any())
            {
                throw new MethodCallNotAllowedException(options.Expression, "Only override without any arguments can be translated to JavaScript");
            }

            options.Translator.VisitWithBrackets(options.Expression.Object, options.Writer);
            options.Writer.Write(".trimLeft()");
        }
    }
}