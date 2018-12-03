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

        public TranslationResult GenerateTranslationDelegate(string code, List<LambdaExpression> optionDelegates, List<Type> optionTypes)
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
                OptionTypes = optionTypes.ToArray()
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
            IEnumerable<LambdaExpression> optionDelegates,
            IList<Type> optionTypes,
            Expression parameter,
            ref string code)
        {
            var arrayItems = new List<Expression>();
            var arrayIndexes = new ConcurrentDictionary<Type, int>();

            {
                var index = 0;// arrayIndexes.Any() ? arrayIndexes.Values.Max() + 1 : 0;
                var modelPathParameter = Expression.ArrayAccess(parameter, Expression.Constant(index));
                // Put that item into the formatting array
                arrayItems.Add(modelPathParameter);
                // Replace all occurences of $$MODELPATH$$ with the appropriate formatting placeholder
                code = code.Replace("$$MODELPATH$$", $"{{{index}}}");
            }

            foreach (var lambda in optionDelegates)
            {
                var optionType = GetTypeChain(lambda.Body).Last();
                var index = arrayIndexes.GetOrAdd(optionType, t =>
                {
                    var idx = optionTypes.IndexOf(optionType) + 1;
                    // get item from input array (properly casted)
                    var p = Expression.Convert(Expression.ArrayAccess(parameter, Expression.Constant(idx)), optionType);
                    // invoke lambda with item as argument
                    var optionValue = Expression.Convert(Expression.Invoke(lambda, p), typeof(object));
                    // translate result of lambda to JavaScript
                    var translatedOptionsValue = Expression.Call(Expression.Constant(this.translator), TranslateValueMethod, optionValue);
                    // Add the whole expression to the formatting array
                    arrayItems.Add(Expression.Convert(translatedOptionsValue, typeof(object)));

                    return idx;
                });

                code = code.Replace($"$${lambda.GetHashCode()}$$", $"{{{index}}}");
            }

            return arrayItems;
        }
    }
}