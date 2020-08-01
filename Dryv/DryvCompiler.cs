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
            var resultLambda = lambdaExpression != null
                ? CreateInvokingLambda(lambdaExpression, optionsParameter)
                : CreateAlwaysTrueLambda(optionsParameter);

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
            var resultLambda = rule.RuleType switch
            {
                RuleType.Disabling => Expression.Lambda<Func<object, object[], object>>(Expression.Convert(invokeExpression, typeof(object)), modelParameter, optionsParameter),
                RuleType.Validation => Expression.Lambda<Func<object, object[], object>>(invokeExpression, modelParameter, optionsParameter),
                _ => throw new NotSupportedException("Cannot compile parameter.")
            };

            return resultLambda.Compile();
        }

        private static void AddOptionParameters(List<Expression> invokeArguments, LambdaExpression lambdaExpression, Expression optionsParameters, int skip = 0)
        {
            var index = 0;

            invokeArguments.AddRange(from options in lambdaExpression.Parameters.Skip(skip)
                                     let indexConstant = Expression.Constant(index++)
                                     let arrayAccess = Expression.ArrayAccess(optionsParameters, indexConstant)
                                     select Expression.Convert(arrayAccess, options.Type));
        }

        private static Expression<Func<object[], bool>> CreateAlwaysTrueLambda(ParameterExpression optionsParameter)
        {
            return Expression.Lambda<Func<object[], bool>>(Expression.Constant(true), optionsParameter);
        }

        private static Expression<Func<object[], bool>> CreateInvokingLambda(LambdaExpression lambdaExpression, ParameterExpression optionsParameter)
        {
            var invokeArguments = new List<Expression>();
            AddOptionParameters(invokeArguments, lambdaExpression, optionsParameter);
            var invokeExpression = Expression.Invoke(lambdaExpression, invokeArguments);

            return Expression.Lambda<Func<object[], bool>>(invokeExpression, optionsParameter);
        }

        private static void EnsurePreevaluationOptionTypes(DryvCompiledRule rule)
        {
            if (rule.ServiceTypes?.Any() != true)
            {
                rule.ServiceTypes = (from p in rule.ValidationExpression.Parameters.Skip(1)
                                                 select p.Type).ToArray();
            }
        }
    }
}