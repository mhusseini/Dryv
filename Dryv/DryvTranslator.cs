
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dryv.Rework.RuleDetection;
using Dryv.RuleDetection;
using Dryv.Rules;
using Dryv.Translation;

namespace Dryv.Rework
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
            var rules = this.ruleFinder
                .FindValidationRulesInTree(modelType, RuleType.Default)
                .Where(rule => IsRuleEnabled(rule, serviceProvider))
                .ToList();

            var validationRules = rules.Where(r => !r.IsDisablingRule);
            var clientValidation = validationRules.Select(r => Translate(serviceProvider, r));

            var disablingRules = rules.Where(r => r.IsDisablingRule);
            var clientDisablers = disablingRules.Select(r => Translate(serviceProvider, r));

            return new TranslatedExpressions
            {
                ValidationFunctions = clientValidation.GroupBy(c => c.Key, c => c.Value).ToDictionary(i => i.Key, i => i.ToList()),
                DisablingFunctions = clientDisablers.GroupBy(c => c.Key, c => c.Value).ToDictionary(i => i.Key, i => i.ToList()),
            };
        }

        private static bool IsRuleEnabled(DryvCompiledRule rule, Func<Type, object> serviceProvider)
        {
            var arguments = rule.PreevaluationOptionTypes.Select(serviceProvider).ToArray();
            return rule.CompiledEnablingExpression(arguments);
        }

        private static KeyValuePair<string, string> Translate(Func<Type, object> serviceProvider, DryvCompiledRule rule)
        {
            var services = rule.PreevaluationOptionTypes.Select(serviceProvider);
            var arguments = new object[] { null }.Union(services).ToArray();
            try
            {
                var code = rule.TranslatedValidationExpression(_ => null, arguments);

                return new KeyValuePair<string, string>(rule.ModelPath, code);
            }
            catch (NullReferenceException ex)
            {
                var p = rule.ValidationExpression.Parameters;
                var parameters = arguments.Select((o, i) => new { o, i }).Where(x => x.o == null).Select(x => $"'{p[x.i].Name}'").ToList();

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