using System;
using System.Collections.Generic;
using System.Linq;
using Dryv.Configuration;

namespace Dryv.Translation
{
    internal static class RuleTranslationExtensions
    {
        public static IEnumerable<string> Translate(this IEnumerable<DryvRuleNode> rules, Func<Type, object> objectProvider, DryvOptions options, string modelPath, Type modelType)
        {
            var translator = objectProvider(typeof(ITranslator)) as ITranslator;

            return (from r in rules
                    let rule = r.Rule.Translate(translator, options)
                    where rule.TranslationError == null
                    let path = string.IsNullOrWhiteSpace(r.Path) ? r.Path : $".{r.Path}"
                    let preevaluationOptions = new[] { path }.Union(rule.PreevaluationOptionTypes.Select(objectProvider)).ToArray()
                    select rule.TranslatedValidationExpression(preevaluationOptions)).ToList();
        }

        private static DryvRuleDefinition Translate(this DryvRuleDefinition rule, ITranslator translator, DryvOptions options)
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
                rule.CodeTemplate = translatedRule.CodeTemplate;
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
    }
}