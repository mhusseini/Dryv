using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dryv.Compilation;
using Dryv.Configuration;
using Dryv.RuleDetection;
using Dryv.Rules;

namespace Dryv.Translation
{
    public sealed class DryvRuleTranslator
    {
        private readonly ITranslator translator;
        private readonly DryvOptions options;

        public DryvRuleTranslator(ITranslator translator, DryvOptions options)
        {
            this.translator = translator;
            this.options = options;
        }

        public IDictionary<DryvRuleTreeNode, Func<Func<Type, object>, string>> Translate(IEnumerable<DryvRuleTreeNode> rules, string modelPath, Type modelType)
        {
            return (from r in rules
                    let rule = this.Translate(r.Rule)
                    where rule.TranslationError == null
                    let p1 = string.IsNullOrWhiteSpace(r.Path) ? r.Path : $".{r.Path}"
                    let path = rule.IsDisablingRule && p1.Contains(".") ? p1.Substring(0, p1.LastIndexOf(".", StringComparison.Ordinal)) : p1
                    select new
                    {
                        Rule = r,
                        Translate = (Func<Func<Type, object>, string>)(serviceProvider => GetTranslationFromRule(rule, serviceProvider, path))
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

        private static string GetTranslationFromRule(DryvCompiledRule rule, Func<Type, object> serviceProvider, string path)
        {
            var preevaluationServices = rule.PreevaluationOptionTypes.Select(serviceProvider).ToArray();

            if (!rule.CompiledEnablingExpression(preevaluationServices))
            {
                return null;
            }

            var preevaluationOptions = new[] { path }.Union(preevaluationServices).ToArray();

            try
            {
                return rule.TranslatedValidationExpression(serviceProvider, preevaluationOptions);
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

        private DryvCompiledRule Translate(DryvCompiledRule rule)
        {
            if (rule.TranslatedValidationExpression != null ||
                rule.TranslationError != null)
            {
                return rule;
            }

            try
            {
                var translatedRule = this.translator.Translate(rule.ValidationExpression, rule.PropertyExpression, rule.GroupName);

                rule.CompiledEnablingExpression = DryvServerRuleEvaluator.CompileEnablingExpression(rule);
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