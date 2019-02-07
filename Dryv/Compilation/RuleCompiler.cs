using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dryv.Reflection;

namespace Dryv.Compilation
{
    internal static class RuleCompiler
    {
        public static DryvRuleDefinition Compile(DryvRuleDefinition rule)
        {
            if (rule.CompiledValidationExpression != null)
            {
                return rule;
            }

            rule.CompiledValidationExpression = CompileValidationExpression(rule);
            rule.CompiledEnablingExpression = CompileEnablingExpression(rule);

            return rule;
        }

        public static bool IsEnabled(DryvRuleDefinition rule, Func<Type, object> objectFactory)
        {
            Compile(rule);
            var options = GetPreevaluationOptions(rule, objectFactory);

            return rule.CompiledEnablingExpression(options);
        }

        public static DryvResult Validate(DryvRuleDefinition rule, object model, Func<Type, object> objectFactory)
        {
            if (typeof(Task).IsAssignableFrom(rule.ValidationExpression.ReturnType))
            {
                return DryvResult.Success;
            }

            Compile(rule);
            var options = GetPreevaluationOptions(rule, objectFactory);

            return (DryvResult)rule.CompiledValidationExpression(model, options);
        }

        public static Task<DryvResult> ValidateAsync(DryvRuleDefinition rule, object model, Func<Type, object> objectFactory)
        {
            Compile(rule);
            var options = GetPreevaluationOptions(rule, objectFactory);

            var result = rule.CompiledValidationExpression(model, options);

            switch (result)
            {
                case DryvResult dryvResult: return Task.FromResult(dryvResult);
                case Task<DryvResult> task: return task;
                default: throw new InvalidOperationException($"Compiled validation expression for property {rule.Property.DeclaringType.FullName}.{rule.Property.Name} should return '{result.GetType().FullName}'. Only DryvResult and Task<DryvResult> are allowed.");
            }
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

        private static Func<object[], bool> CompileEnablingExpression(DryvRuleDefinition rule)
        {
            var lambdaExpression = rule.EnablingExpression;
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

        private static Func<object, object[], object> CompileValidationExpression(DryvRuleDefinition rule)
        {
            var lambdaExpression = rule.ValidationExpression;
            EnsurePreevaluationOptionTypes(rule);

            var modelParameter = Expression.Parameter(typeof(object), "model");
            var optionsParameter = Expression.Parameter(typeof(object[]), "options");
            var invokeArguments = new List<Expression>
            {
                Expression.Convert(modelParameter, lambdaExpression.Parameters.First().Type)
            };

            AddOptionParameters(invokeArguments, lambdaExpression, optionsParameter, 1);

            var invokeExpression = Expression.Invoke(lambdaExpression, invokeArguments);
            var resultLambda = Expression.Lambda<Func<object, object[], object>>(invokeExpression, modelParameter, optionsParameter);

            return resultLambda.Compile();
        }

        private static void EnsurePreevaluationOptionTypes(DryvRuleDefinition rule)
        {
            if (rule.PreevaluationOptionTypes?.Any() != true)
            {
                rule.PreevaluationOptionTypes = (from p in rule.ValidationExpression.Parameters.Skip(1)
                                                 select p.Type).ToArray();
            }
        }

        private static object[] GetPreevaluationOptions(DryvRuleDefinition rule, Func<Type, object> objectFactory)
        {
            return (from t in rule.PreevaluationOptionTypes
                    select objectFactory(t)).ToArray();
        }
    }
}