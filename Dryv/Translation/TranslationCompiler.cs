using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Configuration;
using Dryv.Extensions;
using Dryv.Reflection;

namespace Dryv.Translation
{
    internal class TranslationCompiler
    {
        private static readonly MethodInfo FormatMethod = typeof(string).GetMethod(nameof(string.Format), typeof(string), typeof(object[]));

        private static readonly MethodInfo TranslateValueMethod = typeof(JavaScriptTranslator).GetMethod(nameof(JavaScriptTranslator.TranslateValue), typeof(object), typeof(DryvOptions));

        public TranslationResult GenerateTranslationDelegate(string code, IEnumerable<InjectedExpression> optionDelegates, IList<Type> serviceTypes)
        {
            // Escape curly braces for usage within string.Format().
            code = code
                .Replace("{", "{{")
                .Replace("}", "}}");

            var servicesParameter = Expression.Parameter(typeof(Func<Type, object>));
            var parameter = Expression.Parameter(typeof(object[]));
            var optionsParameter = Expression.Parameter(typeof(DryvOptions));

            // Create an array that will be used as parameters for string.Format().
            var arrayItems = this.GenerateFormatArgumentExpressions(optionDelegates, serviceTypes, parameter, optionsParameter, ref code);

            // create the following code: string.Format("...", new[]{ ... })
            var pattern = Expression.Constant(code);
            var arguments = Expression.NewArrayInit(typeof(object), arrayItems);
            var blockExpressions = new List<Expression>();
            var resultVariable = Expression.Variable(typeof(string));

            var format = Expression.Call(null, FormatMethod, pattern, arguments);
            blockExpressions.Add(Expression.Assign(resultVariable, format));

            var block = Expression.Block(new[] {resultVariable}, blockExpressions);
            var result = Expression.Lambda<Func<Func<Type, object>, object[], DryvOptions, string>>
                (block, servicesParameter, parameter, optionsParameter);

            return new TranslationResult
            {
                Factory = result.Compile(),
                InjectedServiceTypes = serviceTypes.ToArray(),
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

                    default:
                        yield break;
                }
            }
        }

        private IEnumerable<Expression> GenerateFormatArgumentExpressions(IEnumerable<InjectedExpression> injectedExpressions,
            IList<Type> serviceTypes,
            Expression parameter,
            ParameterExpression parameterExpression,
            ref string code)
        {
            var arrayItems = new List<Expression>();
            var arrayIndexes = new ConcurrentDictionary<int, int>();

            // The first item is the model path.
            arrayItems.Add(Expression.ArrayAccess(parameter, Expression.Constant(0)));

            // Replace all occurrences of $$MODELPATH$$ with the appropriate formatting placeholder
            // TODO: remove "$$MODELPATH$$" from all code.
            code = code.Replace("$$MODELPATH$$", string.Empty);

            foreach (var injectedExpression in injectedExpressions)
            {
                var index = arrayIndexes.GetOrAdd(injectedExpression.Index, t =>
                {
                    // get item from input array (properly casted)
                    var arguments = from p2 in injectedExpression.LambdaExpression.Parameters
                        let idx2 = Expression.Constant(serviceTypes.IndexOf(p2.Type) + 1)
                        let arrayAccess = Expression.ArrayAccess(parameter, idx2)
                        select Expression.Convert(arrayAccess, p2.Type);

                    // invoke lambda with item as argument
                    Expression value = Expression.Convert(Expression.Invoke(injectedExpression.LambdaExpression, arguments), typeof(object));

                    if (!injectedExpression.IsRawOutput)
                    {
                        // translate result of lambda to JavaScript
                        value = Expression.Call(null, TranslateValueMethod, value, parameterExpression);
                    }

                    // Add the whole expression to the formatting array
                    arrayItems.Add(Expression.Convert(value, typeof(object)));

                    return arrayIndexes.Count + 1;
                });

                // Replace all occurrences of $$...$$ with the appropriate formatting placeholder
                code = code.Replace($"$${injectedExpression.Index}$$", $"{{{index}}}");
            }

            return arrayItems;
        }
    }
}