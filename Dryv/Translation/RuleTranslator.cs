using System;
using System.Collections.Generic;
using System.Linq;
using Dryv.Configuration;
using Dryv.RuleDetection;
using Dryv.Rules;

namespace Dryv.Translation
{
    public sealed class DryvRuleTranslator
    {
        private readonly DryvOptions options;
        private readonly Func<Type, object> serviceProvider;

        public DryvRuleTranslator(DryvOptions options, Func<Type, object> serviceProvider)
        {
            this.options = options;
            this.serviceProvider = serviceProvider;
        }

        public IDictionary<DryvRuleTreeNode, string> Translate(IEnumerable<DryvRuleTreeNode> rules, string modelPath, Type modelType)
        {
            var translator = this.serviceProvider(typeof(ITranslator)) as ITranslator;
            return (from r in rules
                    let rule = this.Translate(r.Rule, translator)
                    where rule.TranslationError == null
                    let p1 = string.IsNullOrWhiteSpace(r.Path) ? r.Path : $".{r.Path}"
                    let path = rule.IsDisablingRule && p1.Contains(".") ? p1.Substring(0, p1.LastIndexOf(".", StringComparison.Ordinal)) : p1
                    let preevaluationOptions = new[] { path }.Union(rule.PreevaluationOptionTypes.Select(this.serviceProvider)).ToArray()
                    select new { Rule = r, Translation = rule.TranslatedValidationExpression(this.serviceProvider, preevaluationOptions) })
                .ToDictionary(x => x.Rule, x => x.Translation);
        }

        private DryvCompiledRule Translate(DryvCompiledRule rule, ITranslator translator)
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
                switch (this.options.TranslationErrorBehavior)
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