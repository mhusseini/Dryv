using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Reflection;

namespace Dryv.Translation
{
    internal class TranslationCompiler
    {
        private static readonly MethodInfo FormatMethod = typeof(string).GetMethod(nameof(string.Format), typeof(string), typeof(object[]));

        private static readonly MethodInfo TranslateValueMethod = typeof(Translator).GetMethod(nameof(Translator.TranslateValue));
        private readonly object translator;

        public TranslationCompiler(object translator)
        {
            this.translator = translator;
        }

        public TranslationResult GenerateTranslationDelegate(string code, IEnumerable<OptionDelegate> optionDelegates, IList<Type> optionTypes)
        {
            // Escape curly braces for usage within string.Format().
            code = code
                .Replace("{", "{{")
                .Replace("}", "}}");

            // We will produce a delegate that has a single parameter:
            var parameter = Expression.Parameter(typeof(object[]));

            // Create an array that will be used as parameters for string.Format().
            var arrayItems = this.GenerateFormatArgumentExpressions(optionDelegates, optionTypes, parameter, ref code);

            // create the following code: string.Format("...", new[]{ ... })
            var pattern = Expression.Constant(code);
            var arguments = Expression.NewArrayInit(typeof(object), arrayItems);
            var format = Expression.Call(null, FormatMethod, pattern, arguments);
            var result = Expression.Lambda<Func<object[], string>>(format, parameter);

            return new TranslationResult
            {
                Factory = result.Compile(),
                OptionTypes = optionTypes.ToArray(),
                CodeTemplate = code,
            };
        }

        private static IEnumerable<Type> GetTypeChain(Expression expression)
        {
            if (!(expression is MemberExpression memberExpression))
            {
                yield break;
            }

            while (memberExpression != null)
            {
                switch (memberExpression.Expression)
                {
                    case MemberExpression mex:
                        memberExpression = mex;
                        yield return mex.Type;
                        break;

                    case ParameterExpression pex:
                        memberExpression = null;
                        yield return pex.Type;
                        break;
                }
            }
        }

        private IEnumerable<Expression> GenerateFormatArgumentExpressions(
            IEnumerable<OptionDelegate> optionDelegates,
            IList<Type> optionTypes,
            Expression parameter,
            ref string code)
        {
            var arrayItems = new List<Expression>();
            var arrayIndexes = new ConcurrentDictionary<string, int>();

            // The first item is the model path.
            arrayItems.Add(Expression.ArrayAccess(parameter, Expression.Constant(0)));

            // Replace all occurrences of $$MODELPATH$$ with the appropriate formatting placeholder
            code = code.Replace("$$MODELPATH$$", "{0}");

            foreach (var optionDelegate in optionDelegates)
            {
                var optionType = GetTypeChain(optionDelegate.LambdaExpression.Body).LastOrDefault();
                var key = optionType?.Name ?? optionDelegate.Index.ToString();

                var index = arrayIndexes.GetOrAdd(key, t =>
                {
                    // get item from input array (properly casted)
                    var arguments = from p2 in optionDelegate.LambdaExpression.Parameters
                                    let idx2 = Expression.Constant(optionTypes.IndexOf(p2.Type) + 1)
                                    let arrayAccess = Expression.ArrayAccess(parameter, idx2)
                                    select Expression.Convert(arrayAccess, p2.Type);

                    // invoke lambda with item as argument
                    Expression optionValue = Expression.Convert(Expression.Invoke(optionDelegate.LambdaExpression, arguments), typeof(object));

                    if (!optionDelegate.IsRawOutput)
                    {
                        // translate result of lambda to JavaScript
                        optionValue = Expression.Call(Expression.Constant(this.translator), TranslateValueMethod, optionValue);
                    }

                    // Add the whole expression to the formatting array
                    arrayItems.Add(Expression.Convert(optionValue, typeof(object)));

                    return arrayIndexes.Count + 1;
                });

                // Replace all occurrences of $$...$$ with the appropriate formatting placeholder
                code = code.Replace($"$${optionDelegate.Index}$$", $"{{{index}}}");
            }

            return arrayItems;
        }
    }
}