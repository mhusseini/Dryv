using System;
using System.Collections.Generic;
using System.Linq;
using Dryv.Configuration;

namespace Dryv.Translation
{
    internal static class RuleTranslationExtensions
    {
        public static DryvRule Translate(this DryvRule rule, ITranslator translator, DryvOptions options)
        {
            if (rule.TranslatedValidationExpression != null ||
                rule.TranslationError != null)
            {
                return rule;
            }

            try
            {
                var translatedRule = translator.Translate(rule.ValidationExpression);

                rule.TranslatedValidationExpression = translatedRule.Factory;
                rule.PreevaluationOptionTypes = translatedRule.OptionTypes;
            }
            catch (DryvException ex)
            {
                switch (options.TranslationErrorBehavior)
                {
                    case TranslationErrorBehavior.ValidateOnServer:
                        rule.TranslatedValidationExpression = null;
                        rule.PreevaluationOptionTypes = null;
                        rule.TranslationError = ex;
                        break;

                    default:
                        throw;
                }
            }

            return rule;
        }

        public static string Translate(this IEnumerable<DryvRule> rules, Func<Type, object> objectProvider, DryvOptions options)
        {
            var translator = objectProvider(typeof(ITranslator)) as ITranslator;

            return $@"[{
                    string.Join(",",
                        from r in rules
                        let rule = r.Translate(translator, options)
                        where rule.TranslationError == null
                        let preevaluationOptions = rule.PreevaluationOptionTypes.Select(objectProvider).ToArray()
                        select rule.TranslatedValidationExpression(preevaluationOptions))
                }]";
        }
    }
}