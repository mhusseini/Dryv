using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dryv.Extensions;
using Dryv.RuleDetection;
using Dryv.Rules;
using Dryv.Translation;

namespace Dryv
{
    public class DryvTranslator
    {
        private readonly DryvRuleFinder ruleFinder;

        public DryvTranslator(DryvRuleFinder ruleFinder)
        {
            this.ruleFinder = ruleFinder;
        }

        public TranslatedExpressions TranslateValidationRules(Type modelType, Func<Type, object> serviceProvider)
        {
            var validationRules = this.ruleFinder
                .FindValidationRulesInTree(modelType, RuleType.Validation)
                .Where(rule => IsRuleEnabled(rule, serviceProvider))
                .ToList();
            var clientValidation = validationRules.Select(r => Translate(serviceProvider, r));

            var disablingRules = this.ruleFinder
                .FindValidationRulesInTree(modelType, RuleType.Disabling)
                .Where(rule => IsRuleEnabled(rule, serviceProvider))
                .ToList();

            var clientDisablers = disablingRules.Select(r => Translate(serviceProvider, r));

            return new TranslatedExpressions
            {
                ValidationFunctions = clientValidation.GroupBy(c => c.Key, c => c.Value).ToDictionary(i => i.Key, i => i.ToList()),
                DisablingFunctions = clientDisablers.GroupBy(c => c.Key, c => c.Value).ToDictionary(i => i.Key, i => i.ToList()),
            };
        }

        private static bool IsRuleEnabled(DryvCompiledRule rule, Func<Type, object> serviceProvider)
        {
            var arguments = serviceProvider.GetServices(rule.PreevaluationOptionTypes);
            return rule.CompiledEnablingExpression(arguments);
        }

        private static KeyValuePair<string, string> Translate(Func<Type, object> serviceProvider, DryvCompiledRule rule)
        {
            var services = serviceProvider.GetServices(rule.PreevaluationOptionTypes);

            // index 0 was used to transpose the path. It isn't used anymore,
            // but it was too cumbersome to update all that code :-)
            // TODO: Update all that code.
            var arguments = new object[services.Length + 1];
            services.CopyTo(arguments, 1);

            try
            {
                var code = rule.TranslatedValidationExpression(_ => null, arguments);

                return new KeyValuePair<string, string>(rule.ModelPath, code);
            }
            catch (Exception ex)
            {
                throw CreateException("An error occurred while translating validation rule.", ex, rule);
            }
        }

        public class TranslatedExpressions
        {
            public Dictionary<string, List<string>> ValidationFunctions { get; internal set; }
            public Dictionary<string, List<string>> DisablingFunctions { get; internal set; }
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
    }
}