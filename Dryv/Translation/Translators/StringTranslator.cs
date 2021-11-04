using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dryv.Translation.Translators
{
    public class StringTranslator : MethodCallTranslator
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
            //this.AddMethodTranslator(nameof(string.Normalize), Normalize);
            this.AddMethodTranslator("Normalize", Normalize);
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
                select (StringComparison?) constExp.Value).FirstOrDefault();

            if (stringComparison == null)
            {
                return false;
            }

            switch ((int) stringComparison.Value)
            {
                case (int) StringComparison.CurrentCultureIgnoreCase:
                case (int) StringComparison.OrdinalIgnoreCase:
                case 3: // StringComparison.InvariantCultureIgnoreCase:
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

        private static void Compare(MethodTranslationContext context)
        {
            /*
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length, bool ignoreCase);
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length, StringComparison comparisonType);
             √ public static int Compare(String strA, String strB);
             √ public static int Compare(String strA, String strB, bool ignoreCase);
             √! public static int Compare(String strA, String strB, bool ignoreCase, CultureInfo culture);
             √ public static int Compare(String strA, String strB, CultureInfo culture, CompareOptions options);
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length, bool ignoreCase, CultureInfo culture);
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length, CultureInfo culture, CompareOptions context);
             √ public static int Compare(String strA, String strB, StringComparison comparisonType);
             Х public static int Compare(String strA, int indexA, String strB, int indexB, int length);
             */
            if (context.Expression.Method.GetParameters().Any(p => p.ParameterType == typeof(int)))
            {
                throw new DryvMethodNotSupportedException(context.Expression, "Only override without any indexes can be translated to JavaScript");
            }

            var arguments = context.Expression.Arguments;
            context.Translator.Translate(arguments.FirstOrDefault(), context);
            var isCaseInsensitive = ArgumentIs(context, 2, true) || GetIsCaseInsensitive(context.Expression);
            if (isCaseInsensitive)
            {
                context.Writer.Write(".toLowerCase()");
            }

            context.Writer.Write(".localeCompare(");
            context.Translator.Translate(arguments[1], context);
            if (isCaseInsensitive)
            {
                context.Writer.Write(".toLowerCase()");
            }

            context.Writer.Write(")");
        }

        private static void CompareTo(MethodTranslationContext context)
        {
            /*
             √ public int CompareTo(String strB);
             √ public int CompareTo(object value);
             */
            var arguments = context.Expression.Arguments;
            context.Translator.Translate(context.Expression.Object, context);
            context.Writer.Write(".localeCompare(");
            context.Translator.Translate(arguments[0], context);
            context.Writer.Write(")");
        }

        private static void Contains(MethodTranslationContext context)
        {
            /*
             √ public bool Contains(String value)
             */
            context.Translator.Translate(context.Expression.Object, context);

            var isCaseInsensitive = GetIsCaseInsensitive(context.Expression);
            if (isCaseInsensitive)
            {
                context.Writer.Write(".toLowerCase()");
            }

            context.Writer.Write(".indexOf(");
            WriteArguments(context.Translator, context.Expression.Arguments, context);
            if (isCaseInsensitive)
            {
                context.Writer.Write(".toLowerCase()");
            }

            context.Writer.Write(context.Negated ? ") < 0" : ") >= 0");
        }

        private static void EndsWith(MethodTranslationContext context)
        {
            /*
             √ public bool EndsWith(String value);
             √ public bool EndsWith(String value, bool ignoreCase, CultureInfo culture);
             √ public bool EndsWith(String value, StringComparison comparisonType);
             √ public bool EndsWith(char value);
             */
            context.Writer.Write("(");
            WriteIndexOf(context);

            context.Writer.Write(context.Negated ? "< 0 || " : ">= 0 && ");

            WriteIndexOf(context);

            context.Writer.Write(context.Negated ? " !== (" : " === (");
            context.Translator.Translate(context.Expression.Object, context);
            context.Writer.Write(".length - ");
            context.Translator.Translate(context.Expression.Arguments[0], context);
            context.Writer.Write(".length)");
            context.Writer.Write(")");
        }

        private static void Equals(MethodTranslationContext context)
        {
            /*
             √ public static bool Equals(String a, String b, StringComparison comparisonType);
             √ public static bool Equals(String a, String b);
             √ public bool Equals(String b, StringComparison comparisonType);
             √ public bool Equals(String b);
             */
            Expression value1;
            Expression value2;

            if (context.Expression.Method.IsStatic)
            {
                value1 = context.Expression.Arguments.First();
                value2 = context.Expression.Arguments.Skip(1).First();
            }
            else
            {
                value1 = context.Expression.Object;
                value2 = context.Expression.Arguments.First();
            }

            context.Translator.Translate(value1, context);
            var isCaseInsensitive = GetIsCaseInsensitive(context.Expression);
            if (isCaseInsensitive)
            {
                context.Writer.Write(".toLowerCase()");
            }

            context.Writer.Write(context.Negated ? "!==" : " === ");


            WriteArguments(context.Translator, new[] {value2}, context);
            if (isCaseInsensitive)
            {
                context.Writer.Write(".toLowerCase()");
            }
        }

        private static void Format(MethodTranslationContext context)
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

            var patternIndex = context.Expression.Method.GetParameters().First().ParameterType == typeof(IFormatProvider)
                ? 1
                : 0;

            if (!(context.Expression.Arguments.Skip(patternIndex).FirstOrDefault() is ConstantExpression pattern))
            {
                throw new DryvExpressionNotSupportedException(context.Expression, "Calls to string.Format with non-constant pattern strings are not supported.");
            }

            var skipped = context.Expression.Arguments.Skip(patternIndex + 1);
            var arguments = skipped.FirstOrDefault() is NewArrayExpression nar
                ? nar.Expressions.Cast<object>().ToArray()
                : skipped.Cast<object>().ToArray();

            var parts = StringFormatDissector.Recombine(pattern.Value.ToString(), arguments);

            for (var index = 0; index < parts.Count; index++)
            {
                var part = parts[index];
                switch (part)
                {
                    case string text:
                        if (index > 0)
                        {
                            context.Writer.Write(" + ");
                        }

                        context.Writer.Write(JavaScriptHelper.TranslateValue(text));

                        if (index < parts.Count - 1)
                        {
                            context.Writer.Write(" + ");
                        }

                        break;

                    case Expression exp:
                        context.Translator.Translate(exp, context);
                        break;
                }
            }
        }

        private static void IndexOf(MethodTranslationContext context)
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
            if (context.Expression.Method.GetParameters().Any(p => p.ParameterType == typeof(int)))
            {
                throw new DryvMethodNotSupportedException(context.Expression, "Only override without any indexes can be translated to JavaScript");
            }

            context.Translator.Translate(context.Expression.Object, context);
            var isCaseInsensitive = GetIsCaseInsensitive(context.Expression);
            if (isCaseInsensitive)
            {
                context.Writer.Write(".toLowerCase()");
            }

            context.Writer.Write(".indexOf(");
            context.Translator.Translate(context.Expression.Arguments[0], context);
            if (isCaseInsensitive)
            {
                context.Writer.Write(".toLowerCase()");
            }

            context.Writer.Write(")");
        }

        private static void IsNullOrEmpty(MethodTranslationContext context)
        {
            /*
             √ public static bool IsNullOrEmpty(String value);
             */
            if (!context.Negated)
            {
                context.Writer.Write("!");
            }

            context.Translator.Translate(context.Expression.Arguments.First(), context);
        }

        private static void IsNullOrWhiteSpace(MethodTranslationContext context)
        {
            /*
             √ public static bool IsNullOrWhiteSpace(String value);
             */
            if (!context.Negated)
            {
                context.Writer.Write("!");
            }

            context.Writer.Write(@"/\S/.test(");
            context.Translator.Translate(context.Expression.Arguments.First(), context);
            context.Writer.Write(@" || """")");
        }

        private static void Normalize(MethodTranslationContext context)
        {
            /*
             √ public String Normalize();
             √! public String Normalize(NormalizationForm normalizationForm);
             */
            context.Translator.Translate(context.Expression.Object, context);
            context.Writer.Write(".normalize()");
        }

        private static void StartsWith(MethodTranslationContext context)
        {
            /*
             √ public bool StartsWith(char value);
             √ public bool StartsWith(String value);
             √! public bool StartsWith(String value, bool ignoreCase, CultureInfo culture);
             √ public bool StartsWith(String value, StringComparison comparisonType);
             */
            context.Translator.Translate(context.Expression.Object, context);
            var isCaseInsensitive = ArgumentIs(context, 1, true) || GetIsCaseInsensitive(context.Expression);
            if (isCaseInsensitive)
            {
                context.Writer.Write(".toLowerCase()");
            }

            context.Writer.Write(".indexOf(");
            context.Translator.Translate(context.Expression.Arguments[0], context);
            
            if (isCaseInsensitive)
            {
                context.Writer.Write(".toLowerCase()");
            }

            context.Writer.Write(context.Negated ? ") !== 0" : ") === 0");
        }

        private static void ToLower(MethodTranslationContext context)
        {
            /*
             √ public String ToLower();
             √! public String ToLower(CultureInfo culture);
             */
            context.Translator.Translate(context.Expression.Object, context);
            context.Writer.Write(".toLowerCase()");
        }

        private static void ToUpper(MethodTranslationContext context)
        {
            /*
             √ public String ToUpper();
             √! public String ToUpper(CultureInfo culture);
             */
            context.Translator.Translate(context.Expression.Object, context);
            context.Writer.Write(".toUpperCase()");
        }

        private static void Trim(MethodTranslationContext context)
        {
            /*
             Х public String Trim(char trimChar);
             Х public String Trim(params char[] trimChars);
             √ public String Trim();
             */
            if (context.Expression.Arguments.Any())
            {
                throw new DryvMethodNotSupportedException(context.Expression, "Only override without any arguments can be translated to JavaScript");
            }

            context.Translator.Translate(context.Expression.Object, context);
            context.Writer.Write(".trim()");
        }

        private static void TrimEnd(MethodTranslationContext context)
        {
            /*
             √ public String TrimEnd();
             Х public String TrimEnd(char trimChar);
             Х public String TrimEnd(params char[] trimChars);
             */
            if (context.Expression.Arguments.Any())
            {
                throw new DryvMethodNotSupportedException(context.Expression, "Only override without any arguments can be translated to JavaScript");
            }

            context.Translator.Translate(context.Expression.Object, context);
            context.Writer.Write(".trimRight()");
        }

        private static void TrimStart(MethodTranslationContext context)
        {
            /*
             √ public String TrimStart();
             Х public String TrimStart(char trimChar);
             Х public String TrimStart(params char[] trimChars);
             */
            if (context.Expression.Arguments.Any())
            {
                throw new DryvMethodNotSupportedException(context.Expression, "Only override without any arguments can be translated to JavaScript");
            }

            context.Translator.Translate(context.Expression.Object, context);
            context.Writer.Write(".trimLeft()");
        }

        private static void WriteIndexOf(MethodTranslationContext context)
        {
            context.Translator.Translate(context.Expression.Object, context);

            var isCaseInsensitive = ArgumentIs(context, 1, true) || GetIsCaseInsensitive(context.Expression);
            if (isCaseInsensitive)
            {
                context.Writer.Write(".toLowerCase()");
            }

            context.Writer.Write(".indexOf(");
            context.Translator.Translate(context.Expression.Arguments[0], context);
            if (isCaseInsensitive)
            {
                context.Writer.Write(".toLowerCase()");
            }

            context.Writer.Write(")");
        }
    }
}