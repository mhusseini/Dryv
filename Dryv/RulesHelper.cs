using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dryv.Translation;

namespace Dryv
{
    internal class RulesHelper
    {
        private static readonly ConcurrentDictionary<Expression, string> ClientRules = new ConcurrentDictionary<Expression, string>();
        private static readonly ConcurrentDictionary<LambdaExpression, Func<object, DryvResult>> CompiledRules = new ConcurrentDictionary<LambdaExpression, Func<object, DryvResult>>();
        private static readonly ConcurrentDictionary<Type, IList<DryvRules>> TypeRules = new ConcurrentDictionary<Type, IList<DryvRules>>();

        public static string GetClientRulesForProperty(Type objectType, string propertyName, ITranslator translator) =>
            TranslateRules(
                translator,
                GetRulesForProperty(GetRulesForType(objectType), propertyName));

        public static IEnumerable<Func<object, DryvResult>> GetCompiledRulesForProperty(Type objectType, string propertyName) =>
            CompileRules(GetRulesForProperty(GetRulesForType(objectType), propertyName));

        private static IEnumerable<Func<object, DryvResult>> CompileRules(IEnumerable<Expression> rulesForProperty) =>
            from rule in rulesForProperty.OfType<LambdaExpression>()
            select CompiledRules.GetOrAdd(rule, lambdaExpression =>
            {
                var inputParameter = Expression.Parameter(typeof(object), "input");
                var convertExpression = Expression.Convert(inputParameter, lambdaExpression.Parameters.First().Type);
                var invokeExpression = Expression.Invoke(lambdaExpression, convertExpression);
                var resultLambda = Expression.Lambda<Func<object, DryvResult>>(invokeExpression, inputParameter);
                return resultLambda.Compile();
            });

        private static IEnumerable<Expression> GetRulesForProperty(IEnumerable<DryvRules> allRulesForType, string propertyName) =>
            from rules in allRulesForType
            where rules.PropertyRules.ContainsKey(propertyName)
            from expression in rules.PropertyRules[propertyName]
            select expression;

        private static IEnumerable<DryvRules> GetRulesForType(Type objectType) => TypeRules.GetOrAdd(objectType, t =>
            (from p in t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
             where typeof(DryvRules).IsAssignableFrom(p.FieldType)
             select p.GetValue(null) as DryvRules)
            .Union(
                from p in objectType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                where typeof(DryvRules).IsAssignableFrom(p.PropertyType)
                select p.GetValue(null) as DryvRules)
            .ToList());

        private static string TranslateRules(ITranslator translator, IEnumerable<Expression> rulesForProperty) =>
            $@"[{
                    string.Join(",",
                        from rule in rulesForProperty
                        select ClientRules.GetOrAdd(rule, r =>
                        {
                            var translated = translator.Translate(rule);
                            return Cleanup(translated);
                        }))
                }]";

        private static string Cleanup(string translated) =>
#if DEBUG
            translated;
#else
            RegexWhiteSpace.Replace(RegexNewLine.Replace(translated, " "), string.Empty);
#endif
    }
}