using System.Linq;
using Dryv.Translation;

namespace Dryv.Translators
{
    internal class EnumerableTranslator : MethodCallTranslator
    {
        public EnumerableTranslator()
        {
            this.Supports(typeof(Enumerable));

            this.AddMethodTranslator(nameof(Enumerable.Any), Any);
            this.AddMethodTranslator(nameof(Enumerable.All), All);
            this.AddMethodTranslator(nameof(Enumerable.Sum), Sum);
            this.AddMethodTranslator(nameof(Enumerable.Min), Min);
            this.AddMethodTranslator(nameof(Enumerable.Max), Max);
            this.AddMethodTranslator(nameof(Enumerable.Average), Average);
            this.AddMethodTranslator(nameof(Enumerable.Select), Select);
            this.AddMethodTranslator(nameof(Enumerable.Where), Where);
            this.AddMethodTranslator(nameof(Enumerable.First), First);
            this.AddMethodTranslator(nameof(Enumerable.Last), Last);
            this.AddMethodTranslator(nameof(Enumerable.Single), Single);
            this.AddMethodTranslator(nameof(Enumerable.SingleOrDefault), SingleOrDefault);
            this.AddMethodTranslator(nameof(Enumerable.FirstOrDefault), FirstOrDefault);
            this.AddMethodTranslator(nameof(Enumerable.LastOrDefault), LastOrDefault);
            this.AddMethodTranslator(nameof(Enumerable.Contains), Contains);
            this.AddMethodTranslator(nameof(Enumerable.ElementAt), ElementAt);
            this.AddMethodTranslator(nameof(Enumerable.ElementAtOrDefault), ElementAtOrDefault);
            this.AddMethodTranslator(nameof(Enumerable.Count), Count);
            this.AddMethodTranslator(nameof(Enumerable.DefaultIfEmpty), DefaultIfEmpty);
        }

        protected static void All(MethodTranslationContext context)
        {
            Translate(context, "every");
        }

        protected static void Any(MethodTranslationContext context)
        {
            Translate(context, "some");
        }

        protected static void Contains(MethodTranslationContext context)
        {
            var array = context.Expression.Arguments.First();
            var item = context.Expression.Arguments.Skip(1).FirstOrDefault();

            context.Translator.VisitWithBrackets(array, context);
            context.Writer.Write(".indexOf(");
            context.Translator.Visit(item, context);
            context.Writer.Write(") >= 0");
        }

        protected static void Count(MethodTranslationContext context)
        {
            TranslateSelect(context, "return arr.length;");
        }

        protected static void DefaultIfEmpty(MethodTranslationContext context)
        {
            var array = context.Expression.Arguments.First();
            var defaultValueExpression = context.Expression.Arguments.Skip(1).FirstOrDefault();


            context.Writer.Write("(function(arr){ return arr.length ? arr : [");
            if (defaultValueExpression != null)
            {
                context.Translator.VisitWithBrackets(defaultValueExpression, context);
            }
            else
            {
                var enumType = array.GetExpressionType();
                var defaultValue = (enumType.GetGenericArguments().FirstOrDefault() ?? typeof(object)).GetDefaultValue();
                context.Writer.Write(context.Translator.TranslateValue(defaultValue));
            }
            context.Writer.Write("];})(");
            context.Writer.Write("(");
            context.Translator.Visit(array, context);
            context.Writer.Write(")) ");

        }

        protected static void ElementAt(MethodTranslationContext context)
        {
            var array = context.Expression.Arguments.First();
            var index = context.Expression.Arguments.Skip(1).FirstOrDefault();

            context.Translator.VisitWithBrackets(array, context);
            context.Writer.Write("[");
            context.Translator.Visit(index, context);
            context.Writer.Write("]");
        }

        protected static void ElementAtOrDefault(MethodTranslationContext context)
        {
            var array = context.Expression.Arguments.First();
            var index = context.Expression.Arguments.Skip(1).FirstOrDefault();

            context.Writer.Write("(function(arr){");
            context.Writer.Write("return arr.length > ");
            context.Translator.VisitWithBrackets(index, context);
            context.Writer.Write("? arr[");
            context.Translator.Visit(index, context);
            context.Writer.Write("] : null;})(");
            context.Translator.VisitWithBrackets(array, context);
            context.Writer.Write(")");
        }

        protected static void First(MethodTranslationContext context)
        {
            var array = context.Expression.Arguments.First();
            var func = context.Expression.Arguments.Skip(1).FirstOrDefault();

            context.Writer.Write("(");
            if (func != null)
            {
                Translate(context, "filter");
            }
            else
            {
                context.Translator.Visit(array, context);
            }
            context.Writer.Write(")[0]");
        }

        protected static void FirstOrDefault(MethodTranslationContext context)
        {
            TranslateSelect(context, "return arr.length ? arr[0] : null;");
        }

        protected static void Last(MethodTranslationContext context)
        {
            TranslateSelect(context, "return arr[arr.length - 1];");
        }

        protected static void LastOrDefault(MethodTranslationContext context)
        {
            TranslateSelect(context, "return arr.length ? arr[arr.length - 1] : null;");
        }

        protected static void Average(MethodTranslationContext context)
        {
            context.Writer.Write("(function(){var _c = 0; return ");
            Reduce(context, "++_c; return a + b");
            context.Writer.Write(" / (_c > 0 ? _c + 1 : 0);})()");
        }

        protected static void Max(MethodTranslationContext context)
        {
            Reduce(context, "return a < b ? b : a");
        }

        protected static void Min(MethodTranslationContext context)
        {
            Reduce(context, "return a > b ? b : a");
        }

        protected static void Select(MethodTranslationContext context)
        {
            Translate(context, "map");
        }

        protected static void Single(MethodTranslationContext context)
        {
            TranslateSelect(context, "return arr[0];");
        }

        protected static void SingleOrDefault(MethodTranslationContext context)
        {
            TranslateSelect(context, "return arr.length == 1 ? arr[0] : null;");
        }

        protected static void Sum(MethodTranslationContext context)
        {
            Reduce(context, "return a+b");
        }

        protected static void Where(MethodTranslationContext context)
        {
            Translate(context, "filter");
        }

        private static void Reduce(MethodTranslationContext context, string reducer)
        {
            var array = context.Expression.Arguments.First();
            var func = context.Expression.Arguments.Skip(1).FirstOrDefault();

            context.Translator.VisitWithBrackets(array, context);
            if (func != null)
            {
                context.Writer.Write(".map(");
                context.Translator.Visit(func, context);
                context.Writer.Write(")");
            }

            context.Writer.Write($".reduce(function(a,b){{{reducer}}})");
        }

        private static void Translate(MethodTranslationContext context, string funcName)
        {
            var array = context.Expression.Arguments.First();
            var func = context.Expression.Arguments.Skip(1).FirstOrDefault();

            context.Translator.VisitWithBrackets(array, context);
            context.Writer.Write($".{funcName}(");
            context.Translator.Visit(func, context);
            context.Writer.Write(")");
        }

        private static void TranslateSelect(MethodTranslationContext context, string selector)
        {
            var array = context.Expression.Arguments.First();
            var func = context.Expression.Arguments.Skip(1).FirstOrDefault();

            context.Writer.Write($"(function(arr){{{selector}}})(");
            context.Writer.Write("(");
            if (func != null)
            {
                Translate(context, "filter");
            }
            else
            {
                context.Translator.Visit(array, context);
            }

            context.Writer.Write(")) ");
        }
    }
}