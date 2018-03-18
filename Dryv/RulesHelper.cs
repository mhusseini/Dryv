using Dryv.DependencyInjection;
using Dryv.Translation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dryv
{
    internal class RulesHelper
    {
        private static readonly ConcurrentDictionary<Expression, TranslationResult> ClientRules = new ConcurrentDictionary<Expression, TranslationResult>();
        private static readonly ConcurrentDictionary<LambdaExpression, Func<object, DryvResult>> CompiledRules = new ConcurrentDictionary<LambdaExpression, Func<object, DryvResult>>();
        private static readonly ConcurrentDictionary<Type, IList<DryvRules>> TypeRules = new ConcurrentDictionary<Type, IList<DryvRules>>();

        public static string GetClientRulesForProperty(
            Type objectType,
            string propertyName,
            ITranslator translator,
            DryvOptions options,
            Func<Type, object> objectProvider)
        {
            var typeRules = GetRulesForType(objectType);
            var property = objectType.GetProperty(propertyName);
            var propertyRules = GetRulesForProperty(typeRules, property);

            return TranslateRules(
                translator,
                propertyRules,
                options,
                objectProvider);
        }

        public static IEnumerable<Func<object, DryvResult>> GetCompiledRulesForProperty(
            Type objectType,
            string propertyName)
        {
            var property = objectType.GetProperty(propertyName);
            return CompileRules(GetRulesForProperty(GetRulesForType(objectType), property));
        }

        private static IEnumerable<Func<object, DryvResult>> CompileRules(IEnumerable<Expression> rulesForProperty)
        {
            return from rule in rulesForProperty.OfType<LambdaExpression>()
                   select CompiledRules.GetOrAdd(rule, lambdaExpression =>
                   {
                       var inputParameter = Expression.Parameter(typeof(object), "input");
                       var convertExpression = Expression.Convert(inputParameter, lambdaExpression.Parameters.First().Type);
                       var invokeExpression = Expression.Invoke(lambdaExpression, convertExpression);
                       var resultLambda = Expression.Lambda<Func<object, DryvResult>>(invokeExpression, inputParameter);

                       return resultLambda.Compile();
                   });
        }

        private static IEnumerable<Expression> GetRulesForProperty(
            IEnumerable<DryvRules> allRulesForType,
            PropertyInfo property)
        {
            return from rules in allRulesForType
                   where rules.PropertyRules.ContainsKey(property)
                   from expression in rules.PropertyRules[property]
                   select expression;
        }

        private static IEnumerable<DryvRules> GetRulesForType(Type objectType)
        {
            return TypeRules.GetOrAdd(objectType, t
                => (from p in t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    where typeof(DryvRules).IsAssignableFrom(p.FieldType)
                    select p.GetValue(null) as DryvRules)
                    .Union(
                        from p in objectType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where typeof(DryvRules).IsAssignableFrom(p.PropertyType)
                        select p.GetValue(null) as DryvRules)
                    .ToList());
        }

        private static string TranslateRules(
            ITranslator translator,
            IEnumerable<Expression> rulesForProperty,
            DryvOptions options,
            Func<Type, object> objectProvider)
        {
            return $@"[{
                    string.Join(",",
                        from rule in rulesForProperty
                        let result = ClientRules.GetOrAdd(rule, r => translator.Translate(rule))
                        let preevaluationOptions = result.OptionTypes.Select(objectProvider).ToArray()
                        select result.Factory(preevaluationOptions))
                }]";
        }
    }
}