﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dryv.Configuration;
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

        public async Task<TranslationResult> TranslateValidationRules(Type modelType, Func<Type, object> serviceProvider, IReadOnlyDictionary<string, object> parameters = null)
        {
            var validationRules = this.ruleFinder
                .FindValidationRulesInTree(modelType, RuleType.Validation)
                .Where(r => r.EvaluationLocation.HasFlag(DryvEvaluationLocation.Client))
                .ToList();

            var disablingRules = this.ruleFinder
                .FindValidationRulesInTree(modelType, RuleType.Disabling)
                .Where(r => r.EvaluationLocation.HasFlag(DryvEvaluationLocation.Client))
                .ToList();

            var ruleParameters = await DryvParametersHelper.GetDryvParameters(validationRules.Union(disablingRules), serviceProvider, parameters);

            var validationTasks = validationRules.Select(rule =>
                IsRuleEnabled(rule, serviceProvider, ruleParameters)
                    .ContinueWith(task => task.Result ? rule : null));
            
            var clientValidation = from rule in await Task.WhenAll(validationTasks)
                where rule != null
                select Translate(rule, serviceProvider, ruleParameters);
            
            var disablingTasks = disablingRules.Select(rule =>
                IsRuleEnabled(rule, serviceProvider, ruleParameters)
                    .ContinueWith(task => task.Result ? rule : null));
            
            var clientDisablers = from rule in await Task.WhenAll(disablingTasks)
                where rule != null
                select Translate(rule, serviceProvider, ruleParameters);

            return new TranslationResult
            {
                ValidationFunctions = clientValidation.GroupBy(c => c.Rule.ModelPath, c => c).ToDictionary(i => i.Key, i => i.ToList()),
                DisablingFunctions = clientDisablers.GroupBy(c => c.Rule.ModelPath, c => c).ToDictionary(i => i.Key, i => i.ToList()),
                Parameters = ruleParameters.Values.Select(p => p.Values).Merge()
            };
        }

        private static async Task<bool> IsRuleEnabled(DryvCompiledRule rule, Func<Type, object> serviceProvider, IReadOnlyDictionary<IReadOnlyList<DryvCompiledRule>, DryvParameters> parameters)
        {
            var arguments = serviceProvider.GetServices(rule, parameters);
            return true.Equals(await TaskValueHelper.GetPossiblyAsyncValue(rule.CompiledEnablingExpression(arguments)));
        }

        private static TranslatedRule Translate(DryvCompiledRule rule, Func<Type, object> serviceProvider, IReadOnlyDictionary<IReadOnlyList<DryvCompiledRule>, DryvParameters> parameters)
        {
            var services = serviceProvider.GetServices(rule, parameters);
            var options = serviceProvider.GetService<DryvOptions>();
            
            // index 0 was used to transpose the path. It isn't used anymore,
            // but it was too cumbersome to update all that code :-)
            // TODO: Update all that code.
            var arguments = new object[services.Length + 1];
            services.CopyTo(arguments, 1);

            try
            {
                var code = rule.TranslatedValidationExpression(_ => null, arguments, options);

                return new TranslatedRule {Rule = rule, ClientCode = code};
            }
            catch (Exception ex)
            {
                throw CreateException("An error occurred while translating validation rule.", ex, rule);
            }
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