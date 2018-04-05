using System;
using System.Collections.Generic;
using System.Linq;
using Dryv.Configuration;

namespace Dryv.Translation
{
    internal static class RuleTranslationExtensions
    {
        public static DryvRule Translate(this DryvRule rule, ITranslator translator, DryvOptions options, string modelName)
        {
            if (rule.TranslatedValidationExpression != null ||
                rule.TranslationError != null)
            {
                return rule;
            }

            try
            {
                var translatedRule = translator.Translate(rule.ValidationExpression, modelName);

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

        public static IEnumerable<string> Translate(this IEnumerable<DryvRule> rules, Func<Type, object> objectProvider, DryvOptions options, string modelName)
        {
            var translator = objectProvider(typeof(ITranslator)) as ITranslator;

            return (from r in rules
                    let rule = r.Translate(translator, options, modelName)
                    where rule.TranslationError == null
                    let preevaluationOptions = rule.PreevaluationOptionTypes.Select(objectProvider).ToArray()
                    select rule.TranslatedValidationExpression(preevaluationOptions)).ToList();
        }
    }
}