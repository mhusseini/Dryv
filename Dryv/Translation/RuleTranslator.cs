using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public IDictionary<DryvRuleTreeNode, Func<string>> Translate(IEnumerable<DryvRuleTreeNode> rules, string modelPath, Type modelType)
        {
            var translator = this.serviceProvider(typeof(ITranslator)) as ITranslator;
            return (from r in rules
                    let rule = this.Translate(r.Rule, translator)
                    where rule.TranslationError == null
                    let p1 = string.IsNullOrWhiteSpace(r.Path) ? r.Path : $".{r.Path}"
                    let path = rule.IsDisablingRule && p1.Contains(".") ? p1.Substring(0, p1.LastIndexOf(".", StringComparison.Ordinal)) : p1
                    let preevaluationOptions = new[] { path }.Union(rule.PreevaluationOptionTypes.Select(this.serviceProvider)).ToArray()
                    select new
                    {
                        Rule = r,
                        Translate = (Func<string>)(() => this.GetTranslationFromRule(rule, preevaluationOptions))
                    })
                .ToDictionary(x => x.Rule, x => x.Translate);
        }

        private static DryvTranslationException CreateException(string msg, Exception ex, DryvCompiledRule rule)
        {
            var sb = new StringBuilder(msg);

            sb.AppendLine();
            sb.Append("The error occurred while translating the following rule for property ");
            sb.Append(rule.Property.Name);
            sb.Append(" on type ");
            sb.Append(rule.ModelType.FullName);
            sb.AppendLine(":");
            sb.Append(rule.ValidationExpression);

            return new DryvTranslationException(sb.ToString(), ex);
        }

        private string GetTranslationFromRule(DryvCompiledRule rule, object[] preevaluationOptions)
        {
            try
            {
                return rule.TranslatedValidationExpression(this.serviceProvider, preevaluationOptions);
            }
            catch (NullReferenceException ex)
            {
                var p = rule.ValidationExpression.Parameters;
                var parameters = preevaluationOptions.Select((o, i) => new { o, i }).Where(x => x.o == null).Select(x => $"'{p[x.i].Name}'").ToList();

                var msg = parameters.Count switch
                {
                    0 => "An error occurred while translating the validation rule.",
                    1 => $"The injected rule parameter {parameters.First()} is null.",
                    _ => $"An injected rule parameter is null. Possible candidates are {string.Join(", ", parameters)}."
                };

                throw CreateException(msg, ex, rule);
            }
            catch (Exception ex)
            {
                throw CreateException("An error occurred while translating validation rule.", ex, rule);
            }
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
                var translatedRule = translator.Translate(rule.ValidationExpression, rule.PropertyExpression, rule.GroupName, this.serviceProvider);

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