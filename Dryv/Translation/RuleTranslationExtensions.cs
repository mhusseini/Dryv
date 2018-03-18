using System;
using System.Collections.Generic;
using System.Linq;
using Dryv.Translation;

namespace Dryv
{
    internal static class RuleTranslationExtensions
    {
        public static DryvRule Translate(this DryvRule rule, ITranslator translator)
        {
            if (rule.TranslatedValidationExpression != null)
            {
                return rule;
            }

            var translatedRule = translator.Translate(rule.ValidationExpression);

            rule.TranslatedValidationExpression = translatedRule.Factory;
            rule.PreevaluationOptionTypes = translatedRule.OptionTypes;

            return rule;
        }

        public static string Translate(this IEnumerable<DryvRule> rules, Func<Type, object> objectProvider)
        {
            var translator = objectProvider(typeof(ITranslator)) as ITranslator;

            return $@"[{
                    string.Join(",",
                        from r in rules
                        let rule = Translate(r, translator)
                        let preevaluationOptions = rule.PreevaluationOptionTypes.Select(objectProvider).ToArray()
                        select rule.TranslatedValidationExpression(preevaluationOptions))
                }]";
        }
    }
}