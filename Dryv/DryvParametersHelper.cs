using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dryv.Extensions;
using Dryv.Rules;

namespace Dryv
{
    public class DryvParametersHelper
    {
        public static async Task<Dictionary<IReadOnlyList<DryvCompiledRule>, DryvParameters>> GetDryvParameters(IEnumerable<DryvCompiledRule> rules, Func<Type, object> serviceProvider, IReadOnlyDictionary<string, object> parameters = null)
        {
            var result = new Dictionary<IReadOnlyList<DryvCompiledRule>, DryvParameters>();

            foreach (var rule in rules)
            {
                var parameterValues = new Dictionary<string, object>();

                foreach (var parameter in rule.Parameters)
                {
                    if (parameters == null || !parameters.TryGetValue(parameter.Name, out var value))
                    {
                        var services = serviceProvider.GetServices(parameter.ServiceTypes);
                        var possiblyAsyncValue = parameter.CompiledValidationExpression(null, services);
                        
                        value = await TaskValueHelper.GetPossiblyAsyncValue(possiblyAsyncValue);
                    }

                    parameterValues.Add(parameter.Name, value);
                }

                result[rule.Parameters] = new DryvParameters(parameterValues);
            }

            return result;
        }
    }
}