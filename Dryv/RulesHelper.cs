using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dryv
{
    public class RulesHelper
    {
        private static readonly ConcurrentDictionary<Expression, string> ClientRules = new ConcurrentDictionary<Expression, string>();

        private static readonly ConcurrentDictionary<Type, IList<Rules>> TypeRules = new ConcurrentDictionary<Type, IList<Rules>>();

        public static IEnumerable<Expression> GetRulesForProperty(IEnumerable<Rules> allRulesForType, Type objectType, string propertyName) =>
            GetRulesForProperty(GetRulesForType(objectType), propertyName);

        public static IEnumerable<Expression> GetRulesForProperty(IEnumerable<Rules> allRulesForType, string propertyName) =>
            from rules in allRulesForType
            where rules.PropertyRules.ContainsKey(propertyName)
            from expression in rules.PropertyRules[propertyName]
            select expression;

        public static IEnumerable<Rules> GetRulesForType(Type objectType) => TypeRules.GetOrAdd(objectType, t =>
            (from p in t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
             where typeof(Rules).IsAssignableFrom(p.FieldType)
             select p.GetValue(null) as Rules)
            .Union(
                from p in objectType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                where typeof(Rules).IsAssignableFrom(p.PropertyType)
                select p.GetValue(null) as Rules)
            .ToList());

        public static string GetClientRulesForProperty(Type objectType, string propertyName) =>
            TranslateRules(GetRulesForProperty(GetRulesForType(objectType), propertyName));

        private static readonly Regex RegexNewLine = new Regex(@"[\r\n]+", RegexOptions.Compiled);
        private static readonly Regex RegexWhiteSpace = new Regex(@"\t+|\s{2,}", RegexOptions.Compiled);

        public static string TranslateRules(IEnumerable<Expression> rulesForProperty) =>
            $@"[{
                    string.Join(",",
                        from rule in rulesForProperty
                        select ClientRules.GetOrAdd(rule, r =>
                        {
                            var translator = new JavaScriptTranslator();
                            return RegexWhiteSpace.Replace(RegexNewLine.Replace(translator.Translate(rule), " "), string.Empty);
                        }))
                }]";
    }
}