using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dryv.MethodCallTranslation
{
    internal class StringTranslator : MethodCallTranslator
    {
        private static readonly StringFormatDissector StringFormatDissector = new StringFormatDissector();

        public StringTranslator()
        {
            this.Supports<string>();

            this.AddMethodTranslator(nameof(string.Equals), Equals);
            this.AddMethodTranslator(nameof(string.Contains), Contains);
            this.AddMethodTranslator(nameof(string.StartsWith), StartsWith);
            this.AddMethodTranslator(nameof(string.EndsWith), EndsWith);
            this.AddMethodTranslator(nameof(string.ToLower), ToLower);
            this.AddMethodTranslator(nameof(string.ToUpper), ToUpper);
            this.AddMethodTranslator(nameof(string.IsNullOrEmpty), IsNullOrEmpty);
            this.AddMethodTranslator(nameof(string.IsNullOrWhiteSpace), IsNullOrWhiteSpace);
            this.AddMethodTranslator(nameof(string.Trim), Trim);
            this.AddMethodTranslator(nameof(string.TrimEnd), TrimEnd);
            this.AddMethodTranslator(nameof(string.Normalize), Normalize);
            this.AddMethodTranslator(nameof(string.Compare), Compare);
            this.AddMethodTranslator(nameof(string.CompareTo), CompareTo);
            this.AddMethodTranslator(nameof(string.IndexOf), IndexOf);
            this.AddMethodTranslator(nameof(string.TrimStart), TrimStart);
            this.AddMethodTranslator(nameof(string.Format), Format);
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

        private static void Compare(MethodTranslationParameters parameters)
        {
            /*
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length, bool ignoreCase);
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length, StringComparison comparisonType);
             √ public static int Compare(String strA, String strB);
             √ public static int Compare(String strA, String strB, bool ignoreCase);
             √! public static int Compare(String strA, String strB, bool ignoreCase, CultureInfo culture);
             √ public static int Compare(String strA, String strB, CultureInfo culture, CompareOptions options);
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length, bool ignoreCase, CultureInfo culture);
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length, CultureInfo culture, CompareOptions parameters);
             √ public static int Compare(String strA, String strB, StringComparison comparisonType);
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length);
             */
            if (parameters.Expression.Method.GetParameters().Any(p => p.ParameterType == typeof(int)))
            {
                throw new MethodCallNotAllowedException(parameters.Expression, "Only override without any indexes can be translated to JavaScript");
            }
            var arguments = parameters.Expression.Arguments;
            parameters.Translator.VisitWithBrackets(arguments.FirstOrDefault(), parameters.Writer);
            var isCaseInsensitive = ArgumentIs(parameters, 2, true) || GetIsCaseInsensitive(parameters.Expression);
            if (isCaseInsensitive)
            {
                parameters.Writer.Write(".toLowerCase()");
            }

            parameters.Writer.Write(".localeCompare(");
            parameters.Translator.Visit(arguments[1], parameters.Writer);
            if (isCaseInsensitive)
            {
                parameters.Writer.Write(".toLowerCase()");
            }

            parameters.Writer.Write(")");
        }

        private static void CompareTo(MethodTranslationParameters parameters)
        {
            /*
             √ public int CompareTo(String strB);
             √ public int CompareTo(object value);
             */
            var arguments = parameters.Expression.Arguments;
            parameters.Translator.VisitWithBrackets(parameters.Expression.Object, parameters.Writer);
            parameters.Writer.Write(".localeCompare(");
            parameters.Translator.Visit(arguments[0], parameters.Writer);
            parameters.Writer.Write(")");
        }

        private static void Contains(MethodTranslationParameters parameters)
        {
            /*
             √ public bool Contains(String value)
             */
            parameters.Translator.Visit(parameters.Expression.Object, parameters.Writer);

            var isCaseInsensitive = GetIsCaseInsensitive(parameters.Expression);
            if (isCaseInsensitive)
            {
                parameters.Writer.Write(".toLowerCase()");
            }

            parameters.Writer.Write(".indexOf(");
            WriteArguments(parameters.Translator, parameters.Expression.Arguments, parameters.Writer);
            if (isCaseInsensitive)
            {
                parameters.Writer.Write(".toLowerCase()");
            }

            parameters.Writer.Write(parameters.Negated ? ") < 0" : ") >= 0");
        }

        private static void EndsWith(MethodTranslationParameters parameters)
        {
            /*
             √ public bool EndsWith(String value);
             √ public bool EndsWith(String value, bool ignoreCase, CultureInfo culture);
             √ public bool EndsWith(String value, StringComparison comparisonType);
             √ public bool EndsWith(char value);
             */
            parameters.Translator.Visit(parameters.Expression.Object, parameters.Writer);

            var isCaseInsensitive = ArgumentIs(parameters, 1, true) || GetIsCaseInsensitive(parameters.Expression);
            if (isCaseInsensitive)
            {
                parameters.Writer.Write(".toLowerCase()");
            }

            parameters.Writer.Write(".indexOf(");
            parameters.Translator.Visit(parameters.Expression.Arguments[0], parameters.Writer);
            if (isCaseInsensitive)
            {
                parameters.Writer.Write(".toLowerCase()");
            }

            parameters.Writer.Write(parameters.Negated ? ") !== (" : ") === (");
            parameters.Translator.VisitWithBrackets(parameters.Expression.Object, parameters.Writer);
            parameters.Writer.Write(".length - ");
            parameters.Translator.VisitWithBrackets(parameters.Expression.Arguments[0], parameters.Writer);
            parameters.Writer.Write(".length)");
        }

        private static void Equals(MethodTranslationParameters parameters)
        {
            /*
             √ public static bool Equals(String a, String b, StringComparison comparisonType);
             √ public static bool Equals(String a, String b);
             */
            parameters.Translator.Visit(parameters.Expression.Object, parameters.Writer);

            var value = parameters.Expression.Arguments.First();
            var isCaseInsensitive = GetIsCaseInsensitive(parameters.Expression);
            if (isCaseInsensitive)
            {
                parameters.Writer.Write(".toLowerCase()");
            }

            parameters.Writer.Write(parameters.Negated ? "!==" : " === ");
            WriteArguments(parameters.Translator, new[] { value }, parameters.Writer);
            if (isCaseInsensitive)
            {
                parameters.Writer.Write(".toLowerCase()");
            }
        }

        private static void Format(MethodTranslationParameters parameters)
        {
            /*
             Х public static String Format(IFormatProvider provider, String format, object arg0);
             Х public static String Format(IFormatProvider provider, String format, object arg0, object arg1, object arg2);
             Х public static String Format(IFormatProvider provider, String format, params object[] args);
             √ public static String Format(String format, object arg0);
             X public static String Format(String format, params object[] args);
             √ public static String Format(String format, object arg0, object arg1, object arg2);
             √ public static String Format(String format, object arg0, object arg1);
             Х public static String Format(IFormatProvider provider, String format, object arg0, object arg1);
             */
            if (!(parameters.Expression.Arguments.First() is ConstantExpression pattern))
            {
                throw new ExpressionNotSupportedException("Calls to string.Format with non-constant pattern strings are not supported.");
            }

            if (parameters.Expression.Method.GetParameters().First().ParameterType == typeof(IFormatProvider))
            {
                throw new MethodCallNotAllowedException(parameters.Expression, "Only override with first parameter being a string can be translated to JavaScript");
            }

            if (parameters.Expression.Arguments.OfType<NewArrayExpression>().Any())
            {
                throw new ExpressionNotSupportedException("Calls to string.Format with arguments being an array are not supported.");
            }

            var arguments = parameters.Expression.Arguments.Skip(1).Cast<object>().ToArray();
            var parts = StringFormatDissector.Recombine(pattern.Value.ToString(), arguments);

            for (var index = 0; index < parts.Count; index++)
            {
                var part = parts[index];
                switch (part)
                {
                    case string text:
                        if (index > 0)
                        {
                            parameters.Writer.Write(" + ");
                        }

                        parameters.Writer.Write(QuoteValue(text));

                        if (index < parts.Count - 1)
                        {
                            parameters.Writer.Write(" + ");
                        }

                        break;

                    case ConstantExpression exp:
                        parameters.Translator.VisitWithBrackets(exp, parameters.Writer);
                        break;
                }
            }
        }

        private static void IndexOf(MethodTranslationParameters parameters)
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
            if (parameters.Expression.Method.GetParameters().Any(p => p.ParameterType == typeof(int)))
            {
                throw new MethodCallNotAllowedException(parameters.Expression, "Only override without any indexes can be translated to JavaScript");
            }

            var arguments = parameters.Expression.Arguments;
            parameters.Translator.VisitWithBrackets(parameters.Expression.Object, parameters.Writer);
            var isCaseInsensitive = GetIsCaseInsensitive(parameters.Expression);
            if (isCaseInsensitive)
            {
                parameters.Writer.Write(".toLowerCase()");
            }

            parameters.Writer.Write(".indexOf(");
            parameters.Translator.VisitWithBrackets(arguments[1], parameters.Writer);
            if (isCaseInsensitive)
            {
                parameters.Writer.Write(".toLowerCase()");
            }

            parameters.Writer.Write(")");
        }

        private static void IsNullOrEmpty(MethodTranslationParameters parameters)
        {
            /*
             √ public static bool IsNullOrEmpty(String value);
             */
            if (!parameters.Negated)
            {
                parameters.Writer.Write("!");
            }

            parameters.Translator.VisitWithBrackets(parameters.Expression.Arguments.First(), parameters.Writer);
        }

        private static void IsNullOrWhiteSpace(MethodTranslationParameters parameters)
        {
            /*
             √ public static bool IsNullOrWhiteSpace(String value);
             */
            if (!parameters.Negated)
            {
                parameters.Writer.Write("!");
            }

            parameters.Writer.Write(@"/\S/.test(");
            parameters.Translator.VisitWithBrackets(parameters.Expression.Arguments.First(), parameters.Writer);
            parameters.Writer.Write(@" || """")");
        }

        private static void Normalize(MethodTranslationParameters parameters)
        {
            /*
             √ public String Normalize();
             √! public String Normalize(NormalizationForm normalizationForm);
             */
            parameters.Translator.VisitWithBrackets(parameters.Expression.Object, parameters.Writer);
            parameters.Writer.Write(".normalize()");
        }

        private static void StartsWith(MethodTranslationParameters parameters)
        {
            /*
             √ public bool StartsWith(char value);
             √ public bool StartsWith(String value);
             √! public bool StartsWith(String value, bool ignoreCase, CultureInfo culture);
             √ public bool StartsWith(String value, StringComparison comparisonType);
             */
            parameters.Translator.Visit(parameters.Expression.Object, parameters.Writer);
            var isCaseInsensitive = ArgumentIs(parameters, 1, true) || GetIsCaseInsensitive(parameters.Expression);
            if (isCaseInsensitive)
            {
                parameters.Writer.Write(".toLowerCase()");
            }

            parameters.Writer.Write(".indexOf(");
            WriteArguments(parameters.Translator, parameters.Expression.Arguments, parameters.Writer);
            if (isCaseInsensitive)
            {
                parameters.Writer.Write(".toLowerCase()");
            }

            parameters.Writer.Write(parameters.Negated ? ") !== 0" : ") === 0");
        }

        private static void ToLower(MethodTranslationParameters parameters)
        {
            /*
             √ public String ToLower();
             √! public String ToLower(CultureInfo culture);
             */
            if (!parameters.Negated)
            {
                parameters.Writer.Write("!");
            }

            parameters.Translator.VisitWithBrackets(parameters.Expression.Object, parameters.Writer);
            parameters.Writer.Write(".toLowerCase()");
        }

        private static void ToUpper(MethodTranslationParameters parameters)
        {
            /*
             √ public String ToUpper();
             √! public String ToUpper(CultureInfo culture);
             */
            if (!parameters.Negated)
            {
                parameters.Writer.Write("!");
            }

            parameters.Translator.VisitWithBrackets(parameters.Expression.Object, parameters.Writer);
            parameters.Writer.Write(".toUpperCase()");
        }

        private static void Trim(MethodTranslationParameters parameters)
        {
            /*
             Х public String Trim(char trimChar);
             Х public String Trim(params char[] trimChars);
             √ public String Trim();
             */
            if (parameters.Expression.Arguments.Any())
            {
                throw new MethodCallNotAllowedException(parameters.Expression, "Only override without any arguments can be translated to JavaScript");
            }

            parameters.Translator.VisitWithBrackets(parameters.Expression.Object, parameters.Writer);
            parameters.Writer.Write(".trim()");
        }

        private static void TrimEnd(MethodTranslationParameters parameters)
        {
            /*
             √ public String TrimEnd();
             Х public String TrimEnd(char trimChar);
             Х public String TrimEnd(params char[] trimChars);
             */
            if (parameters.Expression.Arguments.Any())
            {
                throw new MethodCallNotAllowedException(parameters.Expression, "Only override without any arguments can be translated to JavaScript");
            }

            parameters.Translator.VisitWithBrackets(parameters.Expression.Object, parameters.Writer);
            parameters.Writer.Write(".trimRight()");
        }

        private static void TrimStart(MethodTranslationParameters parameters)
        {
            /*
             √ public String TrimStart();
             Х public String TrimStart(char trimChar);
             Х public String TrimStart(params char[] trimChars);
             */
            if (parameters.Expression.Arguments.Any())
            {
                throw new MethodCallNotAllowedException(parameters.Expression, "Only override without any arguments can be translated to JavaScript");
            }

            parameters.Translator.VisitWithBrackets(parameters.Expression.Object, parameters.Writer);
            parameters.Writer.Write(".trimLeft()");
        }
    }
}