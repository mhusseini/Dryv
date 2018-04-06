using System;
using System.Collections.Generic;
using System.Linq;
using Dryv.Configuration;

namespace Dryv.Translation
{
    internal static class RuleTranslationExtensions
    {
        public static DryvRule Translate(this DryvRule rule, ITranslator translator, DryvOptions options, Type modelType)
        {
            if (rule.TranslatedValidationExpression != null ||
                rule.TranslationError != null)
            {
                return rule;
            }

            try
            {
                var translatedRule = translator.Translate(rule.ValidationExpression, rule.PropertyExpression);

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

        public static IEnumerable<string> Translate(this IEnumerable<(string Path, DryvRule Rule)> rules, Func<Type, object> objectProvider, DryvOptions options, string modelPath, Type modelType)
        {
            var translator = objectProvider(typeof(ITranslator)) as ITranslator;

            return (from r in rules
                    let rule = r.Rule.Translate(translator, options, modelType)
                    where rule.TranslationError == null
                    let path = string.IsNullOrWhiteSpace(r.Path) ? r.Path : $".{r.Path}"
                    let preevaluationOptions = rule.PreevaluationOptionTypes.Select(objectProvider).Union(new[] { path }).ToArray()
                    select rule.TranslatedValidationExpression(preevaluationOptions)).ToList();
        }
    }
}