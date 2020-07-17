using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dryv.Rules;

namespace Dryv
{
    public class DryvCompiler
    {
        public Func<object[], bool> CompileEnablingExpression(DryvCompiledRule rule, LambdaExpression lambdaExpression)
        {
            EnsurePreevaluationOptionTypes(rule);

            var optionsParameter = Expression.Parameter(typeof(object[]), "options");
            Expression<Func<object[], bool>> resultLambda;

            if (lambdaExpression != null)
            {
                var invokeArguments = new List<Expression>();
                AddOptionParameters(invokeArguments, lambdaExpression, optionsParameter);
                var invokeExpression = Expression.Invoke(lambdaExpression, invokeArguments);
                resultLambda = Expression.Lambda<Func<object[], bool>>(invokeExpression, optionsParameter);
            }
            else
            {
                resultLambda = Expression.Lambda<Func<object[], bool>>(Expression.Constant(true), optionsParameter);
            }

            return resultLambda.Compile();
        }

        public Func<object, object[], object> CompileValidationExpression(DryvCompiledRule rule, LambdaExpression lambdaExpression)
        {
            EnsurePreevaluationOptionTypes(rule);

            var modelParameter = Expression.Parameter(typeof(object), "model");
            var optionsParameter = Expression.Parameter(typeof(object[]), "options");
            var invokeArguments = new List<Expression>
            {
                Expression.Convert(modelParameter, lambdaExpression.Parameters.First().Type)
            };

            AddOptionParameters(invokeArguments, lambdaExpression, optionsParameter, 1);

            var invokeExpression = Expression.Invoke(lambdaExpression, invokeArguments);
            var resultLambda = rule.IsDisablingRule
                ? Expression.Lambda<Func<object, object[], object>>(Expression.Convert(invokeExpression, typeof(object)), modelParameter, optionsParameter)
                : Expression.Lambda<Func<object, object[], object>>(invokeExpression, modelParameter, optionsParameter);

            return resultLambda.Compile();
        }

        private static void AddOptionParameters(List<Expression> invokeArguments,
                    LambdaExpression lambdaExpression,
            Expression optionsParameters,
            int skip = 0)
        {
            var index = 0;

            invokeArguments.AddRange(from options in lambdaExpression.Parameters.Skip(skip)
                                     let indexConstant = Expression.Constant(index++)
                                     let arrayAccess = Expression.ArrayAccess(optionsParameters, indexConstant)
                                     select Expression.Convert(arrayAccess, options.Type));
        }

        private static void EnsurePreevaluationOptionTypes(DryvCompiledRule rule)
        {
            if (rule.PreevaluationOptionTypes?.Any() != true)
            {
                rule.PreevaluationOptionTypes = (from p in rule.ValidationExpression.Parameters.Skip(1)
                                                 select p.Type).ToArray();
            }
        }
    }
}